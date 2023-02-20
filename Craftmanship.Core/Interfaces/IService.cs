using System.Linq.Expressions;

namespace Craftmanship.Core.Interfaces
{
    public interface IService<T> where T : class
    {
        T GetFirstOrDefalut(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        //Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
