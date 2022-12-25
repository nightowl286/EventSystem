using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.Common.Extensions;
using TNO.EventSystem.Abstractions;
using TNO.EventSystem.Registrations;

namespace TNO.EventSystem
{
   /// <summary>
   /// A facade that encompasses an event system.
   /// </summary>
   public class EventSystemFacade : IEventSystem
   {
      #region Fields
      private readonly ReaderWriterLockSlim _subscriptionsLock = new ReaderWriterLockSlim();
      private readonly Dictionary<Type, List<SubscriptionBase>> _subscriptions = new Dictionary<Type, List<SubscriptionBase>>();
      #endregion

      #region Methods
      #region Subscribe
      /// <inheritdoc/>
      public bool Subscribe<T>(IEventHandler<T> handler) where T : notnull
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            if (TryGetSubscription<T>(handler, out _))
               return false;

            AddSubscription(typeof(T), new InstanceSubscription<T>(handler), true);
            return true;
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }

      /// <inheritdoc/>
      public bool Subscribe<T>(Func<T, Task> action) where T : notnull
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            if (TryGetSubscription<T>(action, out _))
               return false;

            AddSubscription(typeof(T), new DelegateSubscription<T>(action), true);
            return true;
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }

      /// <inheritdoc/>
      public bool SubscribeAll(object subscriber)
      {
         _subscriptionsLock.EnterWriteLock();
         try
         {
            Type type = subscriber.GetType();
            IEnumerable<Type> implementedHandlers = type.GetOpenInterfaceImplementations(typeof(IEventHandler<>));
            bool subscribed = false;

            foreach (Type handlerType in implementedHandlers)
            {
               Type eventDataType = handlerType.GetGenericArguments()[0];
               if (TryGetSubscription(eventDataType, subscriber, out _) == false)
               {
                  SubscriptionBase subscription = CreateSubscription(subscriber, eventDataType);
                  AddSubscription(eventDataType, subscription, false);

                  subscribed = true;
               }
            }
            return subscribed;
         }
         finally
         {
            _subscriptionsLock.ExitWriteLock();
         }
      }
      #endregion
      #region Unsubscribe
      /// <inheritdoc/>
      public bool Unsubscribe<T>(IEventHandler<T> subscriber) where T : notnull
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            if (TryGetSubscription(subscriber, out SubscriptionBase<T>? subscription))
            {
               _subscriptionsLock.EnterWriteLock();
               try
               {
                  Debug.Assert(_subscriptions.ContainsKey(typeof(T)));
                  RemoveSubscription(typeof(T), subscription);
               }
               finally
               {
                  _subscriptionsLock.ExitWriteLock();
               }

               return true;
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }

      /// <inheritdoc/>
      public bool Unsubscribe<T>(Func<T, Task> action) where T : notnull
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            if (TryGetSubscription(action, out SubscriptionBase<T>? subscription))
            {
               _subscriptionsLock.EnterWriteLock();
               try
               {
                  Debug.Assert(_subscriptions.ContainsKey(typeof(T)));
                  RemoveSubscription(typeof(T), subscription);
               }
               finally
               {
                  _subscriptionsLock.ExitWriteLock();
               }

               return true;
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }
      /// <inheritdoc/>
      public bool UnsubscribeAll(object subscriber)
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            Dictionary<Type, SubscriptionBase> toRemove = new Dictionary<Type, SubscriptionBase>();
            foreach (KeyValuePair<Type, List<SubscriptionBase>> pair in _subscriptions)
            {
               foreach (SubscriptionBase subscription in pair.Value)
               {
                  if (subscription.Matches(subscriber))
                  {
                     toRemove.Add(pair.Key, subscription);
                     break;
                  }
               }
            }

            _subscriptionsLock.EnterWriteLock();
            try
            {
               foreach (KeyValuePair<Type, SubscriptionBase> pair in toRemove)
                  RemoveSubscription(pair.Key, pair.Value);
            }
            finally
            {
               _subscriptionsLock.ExitWriteLock();
            }

            return toRemove.Count > 0;
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }
      #endregion
      #region Is Subscribed
      /// <inheritdoc/>
      public bool IsSubscribed<T>(IEventHandler<T> subscriber) where T : notnull
      {
         _subscriptionsLock.EnterReadLock();
         try
         {
            if (TryGetSubscription(subscriber, out SubscriptionBase<T>? subscription))
            {
               Debug.Assert(subscription is InstanceSubscription<T>);
               return !subscription.CanBeRemoved;
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitReadLock();
         }
      }

      /// <inheritdoc/>
      public bool IsSubscribed<T>(Func<T, Task> action) where T : notnull
      {
         _subscriptionsLock.EnterReadLock();
         try
         {
            if (TryGetSubscription(action, out SubscriptionBase<T>? subscription))
            {
               Debug.Assert(subscription is DelegateSubscription<T>);
               return !subscription.CanBeRemoved;
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitReadLock();
         }
      }

      /// <inheritdoc/>
      public bool IsSubscribedForAny(object subscriber)
      {
         _subscriptionsLock.EnterReadLock();
         try
         {
            foreach (List<SubscriptionBase> subscriptions in _subscriptions.Values)
            {
               foreach (SubscriptionBase subscription in subscriptions)
               {
                  if (subscription.Matches(subscriber) && !subscription.CanBeRemoved)
                     return true;
               }
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitReadLock();
         }
      }

      /// <inheritdoc/>
      public bool AnySubscribersForEvent<T>() where T : notnull
      {
         _subscriptionsLock.EnterReadLock();
         try
         {
            if (_subscriptions.TryGetValue(typeof(T), out List<SubscriptionBase>? subscriptions))
            {
               foreach (SubscriptionBase subscription in subscriptions)
               {
                  if (subscription.CanBeRemoved == false)
                     return true;
               }
            }

            return false;
         }
         finally
         {
            _subscriptionsLock.ExitReadLock();
         }
      }
      #endregion

      /// <inheritdoc/>
      public async Task<bool> PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : notnull
      {
         List<Task<bool>> tasks = new List<Task<bool>>();
         bool[] results;

         _subscriptionsLock.EnterReadLock();
         try
         {
            foreach (SubscriptionBase<T> subscription in GetSubscriptions<T>())
            {
               if (cancellationToken.IsCancellationRequested)
                  break;

               tasks.Add(subscription.PublishAsync(eventData, cancellationToken));
            }

            results = await Task.WhenAll(tasks);
         }
         finally
         {
            _subscriptionsLock.ExitReadLock();
         }

         Cleanup();

         foreach (bool result in results)
         {
            if (result)
               return true;
         }

         return false;
      }

      /// <inheritdoc/>
      public void Cleanup()
      {
         _subscriptionsLock.EnterUpgradeableReadLock();
         try
         {
            Dictionary<Type, List<SubscriptionBase>> toRemove = new Dictionary<Type, List<SubscriptionBase>>();
            foreach (KeyValuePair<Type, List<SubscriptionBase>> pair in _subscriptions)
            {
               List<SubscriptionBase>? removeForType = null;
               foreach (SubscriptionBase subscription in pair.Value)
               {
                  if (subscription.CanBeRemoved)
                  {
                     removeForType ??= new List<SubscriptionBase>();
                     removeForType.Add(subscription);
                  }
               }

               if (removeForType is not null)
                  toRemove.Add(pair.Key, removeForType);
            }

            _subscriptionsLock.EnterWriteLock();
            try
            {
               foreach (KeyValuePair<Type, List<SubscriptionBase>> pair in toRemove)
               {
                  foreach (SubscriptionBase subscription in pair.Value)
                     RemoveSubscription(pair.Key, subscription);
               }
            }
            finally
            {
               _subscriptionsLock.ExitWriteLock();
            }
         }
         finally
         {
            _subscriptionsLock.ExitUpgradeableReadLock();
         }
      }
      #endregion

      #region Helpers
      private void AddSubscription(Type type, SubscriptionBase subscription, bool enterLock)
      {
         if (enterLock)
            _subscriptionsLock.EnterWriteLock();

         try
         {
            if (_subscriptions.TryGetValue(type, out List<SubscriptionBase>? subscriptions) == false)
            {
               subscriptions = new List<SubscriptionBase>();
               _subscriptions.Add(type, subscriptions);
            }

            subscriptions.Add(subscription);
         }
         finally
         {
            if (enterLock)
               _subscriptionsLock.ExitWriteLock();
         }
      }
      private void RemoveSubscription(Type type, SubscriptionBase subscription)
      {
         List<SubscriptionBase> subscriptions = _subscriptions[type];

         Debug.Assert(subscriptions.Contains(subscription));
         if (subscriptions.Count == 1)
            _subscriptions.Remove(type);
         else
            subscriptions.Remove(subscription);
      }
      private bool TryGetSubscription<T>(object subscriber, [NotNullWhen(true)] out SubscriptionBase<T>? subscription) where T : notnull
      {
         foreach (SubscriptionBase<T> possibleSubscription in GetSubscriptions<T>())
         {
            if (possibleSubscription.Matches(subscriber))
            {
               subscription = possibleSubscription;
               return true;
            }
         }

         subscription = null;
         return false;
      }
      private bool TryGetSubscription(Type type, object subscriber, [NotNullWhen(true)] out SubscriptionBase? subscription)
      {
         foreach (SubscriptionBase possibleSubscription in GetSubscriptions(type))
         {
            if (possibleSubscription.Matches(subscriber))
            {
               subscription = possibleSubscription;
               return true;
            }
         }

         subscription = null;
         return false;
      }
      private IEnumerable<SubscriptionBase<T>> GetSubscriptions<T>() where T : notnull
      {
         foreach (SubscriptionBase subscription in GetSubscriptions(typeof(T)))
         {
            if (subscription is SubscriptionBase<T> typedSubscription)
               yield return typedSubscription;
         }
      }
      private IEnumerable<SubscriptionBase> GetSubscriptions(Type type)
      {
         if (_subscriptions.TryGetValue(type, out List<SubscriptionBase>? subscriptions))
         {
            foreach (SubscriptionBase subscription in subscriptions)
               yield return subscription;
         }
      }
      private static SubscriptionBase CreateSubscription(object subscriber, Type eventType)
      {
         // Todo(Nightowl): This can be sped-up with expression trees;
         Type genericType = typeof(InstanceSubscription<>).MakeGenericType(eventType);
         object? instance = Activator.CreateInstance(genericType, args: subscriber);
         Debug.Assert(instance is not null);

         return (SubscriptionBase)instance;
      }
      #endregion
   }
}
