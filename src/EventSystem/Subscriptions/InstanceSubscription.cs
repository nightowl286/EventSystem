using TNO.EventSystem.Abstractions;

namespace TNO.EventSystem.Registrations
{
   internal sealed class InstanceSubscription<T> : SubscriptionBase<T> where T : notnull
   {
      #region Fields
      private readonly WeakReference<IEventHandler<T>> _instance;
      #endregion

      #region Properties
      public override bool CanBeRemoved => _instance.TryGetTarget(out _) == false;
      #endregion
      public InstanceSubscription(IEventHandler<T> handler)
      {
         _instance = new WeakReference<IEventHandler<T>>(handler);
      }

      #region Methods
      public override bool Matches(object subscriber)
      {
         if (subscriber is not IEventHandler<T> handler)
            return false;

         if (_instance.TryGetTarget(out IEventHandler<T>? storedHandler) == false)
            return false;

         return storedHandler == handler;
      }
      public override async Task<bool> PublishAsync(T eventData, CancellationToken cancellationToken = default)
      {
         if (_instance.TryGetTarget(out IEventHandler<T>? handler))
         {
            await handler.HandleAsync(eventData, cancellationToken);
            return true;
         }

         return false;
      }
      #endregion
   }
}