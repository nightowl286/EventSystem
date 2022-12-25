using System.Threading;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   /// <summary>
   /// Denotes an event system publisher.
   /// </summary>
   public interface IEventPublisher
   {
      #region Methods
      /// <summary>
      /// Publishes the given <paramref name="eventData"/> for an event of the type <typeparamref name="T"/>.
      /// </summary>
      /// <typeparam name="T">The type of the event to publish.</typeparam>
      /// <param name="eventData">The event data to publish.</param>
      /// <param name="cancellationToken">
      /// A <see cref="CancellationToken"/> that can be used to cancel the publishing, and handling of the published event.
      /// </param>
      /// <returns><see langword="true"/> if the published event was handled, <see langword="false"/> otherwise.</returns>
      /// <remarks>The published event can still be handled by some subscribers, if the cancellation occurs too late.</remarks>
      Task<bool> PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : notnull;
      #endregion
   }
}
