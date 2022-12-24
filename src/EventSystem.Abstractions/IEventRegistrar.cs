using System;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   public interface IEventRegistrar
   {
      #region Methods
      bool SubscribeAll(object subscriber);
      bool Subscribe<T>(IEventHandler<T> handler) where T : notnull;
      bool Subscribe<T>(Func<T, Task> action) where T : notnull;

      bool UnsubscribeAll(object subscriber);
      bool Unsubscribe<T>(IEventHandler<T> subscriber) where T : notnull;
      bool Unsubscribe<T>(Func<T, Task> action) where T : notnull;

      bool IsSubscribedForAny(object subscriber);
      bool IsSubscribed<T>(IEventHandler<T> subscriber) where T : notnull;
      bool IsSubscribed<T>(Func<T, Task> action) where T : notnull;
      #endregion
   }
}
