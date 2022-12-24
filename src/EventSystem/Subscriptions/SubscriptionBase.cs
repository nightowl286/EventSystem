namespace TNO.EventSystem.Registrations
{
   internal abstract class SubscriptionBase
   {
      #region Methods
      public abstract bool Matches(object subscriber);
      #endregion
   }
}
