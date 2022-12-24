namespace TNO.EventSystem.Registrations
{
   internal sealed class DelegateSubscription<T> : SubscriptionBase<T> where T : notnull
   {
      #region Fields
      private readonly WeakReference<Func<T, Task>> _instance;
      #endregion

      #region Properties
      public override bool CanBeRemoved => _instance.TryGetTarget(out _) == false;
      #endregion
      public DelegateSubscription(Func<T, Task> action)
      {
         _instance = new WeakReference<Func<T, Task>>(action);
      }

      #region Methods
      public override bool Matches(object subscriber)
      {
         if (subscriber is not Func<T, Task> action)
            return false;

         if (_instance.TryGetTarget(out Func<T, Task>? storedAction) == false)
            return false;

         return storedAction == action;
      }
      public override async Task<bool> PublishAsync(T eventData, CancellationToken cancellationToken = default)
      {
         if (cancellationToken.IsCancellationRequested)

            if (_instance.TryGetTarget(out Func<T, Task>? storedDelegate))
            {
               await storedDelegate.Invoke(eventData);
               return true;
            }

         return false;
      }
      #endregion
   }
}
