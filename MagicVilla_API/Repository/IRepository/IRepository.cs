using MagicVilla_API.Data;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{

    //repositorio generico: todos los modelos que agreguemos pueden usar este repositorio
    public interface IRepository<T> where T : class
    {
        Task Create(T entity);

        Task<List<T>> GetAll(Expression<Func<T, bool>>? filtro = null);

        Task<T> Get(Expression<Func<T, bool>> filtro =null, bool tracked =true);

        Task Delete(T entity);

        Task Save();
    }
}
