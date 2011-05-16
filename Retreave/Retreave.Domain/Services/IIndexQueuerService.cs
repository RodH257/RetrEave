using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public interface IIndexQueuerService
    {
        void QueueUserStreamIndex(RegisteredUser user);
    }
}