namespace TNO.EventSystem.Registrations
{
   internal abstract class SubscriptionBase
   {
      #region Properties
      public abstract bool CanBeRemoved { get; }
      #endregion

      #region Methods
      public abstract bool Matches(object subscriber);
      #endregion
   }
}
