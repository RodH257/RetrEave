using Retreave.Domain.Models;

namespace Retreave.Domain.DataAccess
{
    /// <summary>
    /// Data access object interface for Users
    /// </summary>
    public interface IRegisteredUserDao: IDao<RegisteredUser, int>
    {

    }
}