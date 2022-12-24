using System.Threading;
using System.Threading.Tasks;

namespace TNO.EventSystem.Abstractions
{
   public interface IEventPublisher
   {
      #region Methods
      Task<bool> PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : notnull;
      #endregion
   }
}
