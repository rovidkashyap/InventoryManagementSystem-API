using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace IMS.DataAccess.Repositories.BaseRepository.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IMSContext _context;
        private readonly DbSet<TEntity> _entities;
        private ILogger<BaseRepository<TEntity>> _logger;
        public BaseRepository(IMSContext context, ILogger<BaseRepository<TEntity>> logger)
        {
            _context = context;
            _logger = logger;
            _entities = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _entities.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any unexpected error
                _logger.LogError(ex, "An error occurred while retrieving all entities.");
                throw new InvalidOperationException("An error occurred while retrieving entities. Please try again later.");
            }
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID.", nameof(id));
            }
            try
            {
                var entity = await _entities.FindAsync(id);

                if(entity == null)
                {
                    // Log that the entity was not found.
                    _logger.LogWarning($"Entity with ID {id} not found.");
                }

                return entity;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the entity with ID {id}");
                throw new InvalidOperationException("An error occurred while retrieving the entity. Please try again later.");
            }
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                // Log database-specific exceptions
                _logger.LogError(ex, "An database error occurred while creating the entity.");
                throw new InvalidOperationException("An error occurred while saving the entity. Please try again later.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the entity.");
                throw;
            }

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, Guid id)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if(id == Guid.Empty)
            {
                throw new ArgumentException($"Invalid entity ID: {nameof(id)}");
            }

            // check if entity is already tracked
            var existingEntity = await _entities.FindAsync(id);

            if(existingEntity == null)
            {
                throw new InvalidOperationException("Entity not found.");
            }

            // Update the existing entity with new values
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            // Ensure the entity is attached to the context if it's not tracked
            if (_context.Entry(existingEntity).State == EntityState.Detached)
            {
                _context.Set<TEntity>().Attach(existingEntity);
            }

            try
            {
                // Save Changes Asynchronously
                await _context.SaveChangesAsync();

                // Refresh the entity after save to get the latest values from the database
                await _context.Entry(existingEntity).ReloadAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("The entity was updates by another user. Please refresh and try again.");
            }
            catch(Exception ex)
            {
                // Log any unexpected error
                _logger.LogError(ex, "An error occurred during update.");
                throw;
            }

            return existingEntity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException("Invalid ID.", nameof(id));
            }
            try
            {
                // Ensure the entity is attached to the context
                var existingEntity = await _entities.FindAsync(id);

                if(existingEntity == null)
                {
                    _logger.LogWarning($"Entity with ID {id} not found for deletion.");
                    return false;
                }

                _entities.Remove(existingEntity);

                int result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch(DbUpdateException ex)
            {
                // Log the database=specific exceptions
                _logger.LogError(ex, $"An database error occurred while deleting the entity with ID {id}");
                throw new InvalidOperationException("An error occurred while deleting the entity. Plaese try again later.");
            }
            catch(Exception ex)
            {
                // Log any unexpected exceptions
                _logger.LogError(ex, $"An unexpected error occurred while deleting the entity with ID {id}");
                throw;
            }
        }

        public async Task<TEntity>? FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
    }
}
