using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_Project.Repositories
{
    public class Repository <T> : IRepository<T> where T : class 
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        private IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracking = true)
        {
            var entities = _dbSet.AsQueryable();
            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (!isTracking)
            {
                entities = entities.AsNoTracking();
            }

            return entities;
        }

        public async Task<T?> GetOneAsync(
           Expression<Func<T, bool>>? filter = null,
           Expression<Func<T, object>>[]? includes = null,
           bool isTracking = true
           )
        {
            var entities = Query(filter, includes, isTracking);
            return await entities.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracking = true
            )
        {
            var entities = Query(filter, includes, isTracking);
            return await entities.ToListAsync();
        }

        public async Task<EntityEntry<T>> CreateAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CommitAsync()
        {
            try
            {

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
