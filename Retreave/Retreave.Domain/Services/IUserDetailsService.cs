using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public interface IUserDetailsService
    {
        void CreateUser(RegisteredUser user);
        RegisteredUser AuthenticateTwitterAccount(TwitterAuthentication authentication, string userName);
    }
}