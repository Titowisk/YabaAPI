using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Abstracts
{
    public abstract class GenericAsyncRepository<TEntity> where TEntity : class
    {
        protected DataContext context;
        protected DbSet<TEntity> entityContext;

        public GenericAsyncRepository(DataContext context)
        {
            this.context = context;
            entityContext = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            return await entityContext.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await entityContext.ToListAsync();
        }

        public virtual void Insert(TEntity entity)
        {
            this.entityContext.Add(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            this.entityContext.AddRange(entities);
        }

        public virtual void Delete(TEntity entity)
        {
            if(context.Entry(entity).State == EntityState.Detached)
            {
                entityContext.Attach(entity);
            }
            entityContext.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            entityContext.RemoveRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            entityContext.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async ValueTask DisposeAsync()
        {
            await context.DisposeAsync();
        }
    }
}
