namespace TNO.EventSystem.Registrations
{
   internal abstract class SubscriptionBase<T> : SubscriptionBase where T : notnull
   {
      #region Properties
      public abstract bool CanBeRemoved { get; }
      #endregion

      #region Methods
      public abstract Task<bool> PublishAsync(T eventData, CancellationToken cancellationToken = default);
      #endregion
   }
}