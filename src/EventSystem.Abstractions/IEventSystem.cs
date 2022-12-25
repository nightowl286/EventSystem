namespace TNO.EventSystem.Abstractions
{
   public interface IEventSystem : IEventPublisher, IEventRegistrar
   {
      #region Methods
      void Cleanup();
      #endregion
   }
}
