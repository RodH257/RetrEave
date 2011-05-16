using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public interface IUserDetailsService
    {
        void CreateUser(RegisteredUser user);
    }
}