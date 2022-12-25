using System.Threading;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   public interface IEventHandler<in T> where T : notnull
   {
      #region Methods
      Task HandleAsync(T eventData, CancellationToken cancellationToken = default);
      #endregion
   }
}
