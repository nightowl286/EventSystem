using System.Threading;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   /// <summary>
   /// Denotes an event handler that handles data for events of the type <typeparamref name="T"/>.
   /// </summary>
   /// <typeparam name="T">The type of the event to handle.</typeparam>
   public interface IEventHandler<in T> where T : notnull
   {
      #region Methods
      /// <summary>Asynchronously handles the given <paramref name="eventData"/>.</summary>
      /// <param name="eventData">The event data to handle.</param>
      /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel this operation.</param>
      /// <returns>A task representing the asynchronous operation.</returns>
      Task HandleAsync(T eventData, CancellationToken cancellationToken = default);
      #endregion
   }
}
