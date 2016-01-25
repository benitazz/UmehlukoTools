#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

#endregion

namespace Umehluko.Tools.DataModel
{
    /// <summary>
    /// The generic repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public class GenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     The _entities.
        /// </summary>
        private readonly UmehlukoEntities1 _entities = null;

        /// <summary>
        ///     The _object set.
        /// </summary>
        private readonly DbSet<TEntity> _objectSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public GenericRepository(UmehlukoEntities1 entities)
        {
            this._entities = entities;
            this._objectSet = entities.Set<TEntity>();
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<TEntity> GetAll(Func<TEntity, bool> predicate = null)
        {
            return predicate != null ? this._objectSet.Where(predicate) : this._objectSet;
        }

        /// <summary>
        /// Gets the i queryable data.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.
        /// </exception>
        public IQueryable<TEntity> GetIQueryableData(Func<TEntity, bool> predicate = null)
        {
            return predicate != null ? this._objectSet.Where(predicate).AsQueryable() : this._objectSet;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity Get(Func<TEntity, bool> predicate)
        {
            return predicate == null ? null : this._objectSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(TEntity entity)
        {
            this._objectSet.Add(entity);
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Attach(TEntity entity)
        {
            this._objectSet.Attach(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Delete(TEntity entity)
        {
            if (this._entities.Entry(entity).State == EntityState.Detached)
            {
                this._objectSet.Attach(entity);
            }

            this._objectSet.Remove(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public void Delete(object id)
        {
            var entityToDelete = this._objectSet.Find(id);
            this.Delete(entityToDelete);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Update(TEntity entity)
        {
            this._objectSet.Attach(entity);
            this._entities.Entry(entity).State = EntityState.Modified;
        }
    }
}