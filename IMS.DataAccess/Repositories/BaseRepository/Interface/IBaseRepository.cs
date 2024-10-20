using System.Linq.Expressions;

namespace IMS.DataAccess.Repositories.BaseRepository.Interface
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, Guid id);
        Task<bool> DeleteAsync(Guid id);

        Task<TEntity>? FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
