using System;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   /// <summary>
   /// Denotes an event system registrar.
   /// </summary>
   public interface IEventRegistrar
   {
      #region Methods
      /// <summary>
      /// Subscribes to all of the <see cref="IEventHandler{T}"/> interfaces 
      /// that the given <paramref name="subscriber"/> implements.
      /// </summary>
      /// <param name="subscriber">The subscriber to check for <see cref="IEventHandler{T}"/> implementations.</param>
      /// <returns>
      /// <see langword="true"/> if even one <see cref="IEventHandler{T}"/> was subscribed, <see langword="false"/> otherwise.
      /// </returns>
      bool SubscribeAll(object subscriber);

      /// <summary>Subscribes the given <paramref name="handler"/> to events of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the event to subscribe to.</typeparam>
      /// <param name="handler">The handler to subscribe to events of the type <typeparamref name="T"/>.</param>
      /// <returns><see langword="true"/> if the subscription was successful, <see langword="false"/> otherwise.</returns>
      bool Subscribe<T>(IEventHandler<T> handler) where T : notnull;

      /// <summary>Subscribes the given <paramref name="action"/> to events of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the event to subscribe to.</typeparam>
      /// <param name="action">The action to subscribe to events of the type <typeparamref name="T"/>.</param>
      /// <returns><see langword="true"/> if the subscription was successful, <see langword="false"/> otherwise.</returns>
      bool Subscribe<T>(Func<T, Task> action) where T : notnull;

      /// <summary>
      /// Unsubscribes to all of the <see cref="IEventHandler{T}"/> interfaces
      /// that the given <paramref name="subscriber"/> implements.
      /// </summary>
      /// <param name="subscriber">The subscriber to check for <see cref="IEventHandler{T}"/> implementations.</param>
      /// <returns>
      /// <see langword="true"/> if even one <see cref="IEventHandler{T}"/> was unsubscribed, <see langword="false"/> otherwise.
      /// </returns>
      bool UnsubscribeAll(object subscriber);

      /// <summary>Unsubscribes the given <paramref name="subscriber"/> from events of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the event to unsubscribe from.</typeparam>
      /// <param name="subscriber">The handler to unsubscribe from events of the type <typeparamref name="T"/>.</param>
      /// <returns><see langword="true"/> if the unsubscription was successful, <see langword="false"/> otherwise.</returns>
      bool Unsubscribe<T>(IEventHandler<T> subscriber) where T : notnull;

      /// <summary>Unsubscribes the given <paramref name="action"/> from events of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the event to unsubscribe from.</typeparam>
      /// <param name="action">The action to unsubscribe from events of the type <typeparamref name="T"/>.</param>
      /// <returns><see langword="true"/> if the unsubscription was successful, <see langword="false"/> otherwise.</returns>
      bool Unsubscribe<T>(Func<T, Task> action) where T : notnull;

      /// <summary>Checks whether the given <paramref name="subscriber"/> is subscribed to events of any type.</summary>
      /// <param name="subscriber">The subscriber to check.</param>
      /// <returns>
      /// <see langword="true"/> if the given <paramref name="subscriber"/> is
      /// subscribed to any type of events, <see langword="false"/> otherwise.
      /// </returns>
      bool IsSubscribedForAny(object subscriber);

      /// <summary>
      /// Checks whether the given <paramref name="subscriber"/> is subscribed to events of the type <typeparamref name="T"/>.
      /// </summary>
      /// <typeparam name="T">The type of the event to check for.</typeparam>
      /// <param name="subscriber">The subscriber to check.</param>
      /// <returns>
      /// <see langword="true"/> if the given <paramref name="subscriber"/>
      /// is subscribed to events of the type <typeparamref name="T"/>.
      /// </returns>
      bool IsSubscribed<T>(IEventHandler<T> subscriber) where T : notnull;

      /// <summary>
      /// Checks whether the given <paramref name="action"/> is subscribed to events of the type <typeparamref name="T"/>.
      /// </summary>
      /// <typeparam name="T">The type of the event to check for.</typeparam>
      /// <param name="action">The action to check.</param>
      /// <returns>
      /// <see langword="true"/> if the given <paramref name="action"/>
      /// is subscribed to events of the type <typeparamref name="T"/>.
      /// </returns>
      bool IsSubscribed<T>(Func<T, Task> action) where T : notnull;

      /// <summary>Checks whether there are any subscribers to events of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the event to check for.</typeparam>
      /// <returns>
      /// <see langword="true"/> if there are any subscribers for events of the
      /// type <typeparamref name="T"/>, <see langword="false"/> otherwise.
      /// </returns>
      bool AnySubscribersForEvent<T>() where T : notnull;
      #endregion
   }
}
