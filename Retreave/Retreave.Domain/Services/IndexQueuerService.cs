using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    /// <summary>
    /// Service Layer for queuing indexes 
    /// </summary>
    public class IndexQueuerService : IIndexQueuerService
    {


        public void QueueUserStreamIndex(RegisteredUser user)
        {
           Index streamIndex =  user.AddStreamIndex();
           
        }
    }
}
