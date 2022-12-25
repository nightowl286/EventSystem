namespace TNO.EventSystem.Abstractions
{
   /// <summary>
   /// Denotes an event system.
   /// </summary>
   public interface IEventSystem : IEventPublisher, IEventRegistrar
   {
      #region Methods
      /// <summary>Cleans up any remaining weak references to subscribers that no longer exist.</summary>
      void Cleanup();
      #endregion
   }
}
