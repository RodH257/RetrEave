using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retreave.Domain.DataAccess
{
    /// <summary>
    /// Base persistence object
    /// </summary>
    /// <typeparam name="T">Type of persisted object </typeparam>
    /// <typeparam name="IdT">Id Type</typeparam>
    public interface IDao<T, IdT>
    {
        T GetById(IdT id);
        T SaveOrUpdate(T entity);
        void Delete(T entity);
    }
}
