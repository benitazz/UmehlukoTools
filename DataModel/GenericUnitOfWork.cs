#region

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

#endregion

namespace Umehluko.Tools.DataModel
{
    // <summary>
    ///     The generic unit of work.
    /// </summary>
    public class GenericUnitOfWork : IDisposable
    {
        /// <summary>
        ///     The _entities.
        /// </summary>
        private readonly UmehlukoEntities1 _entities = null;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericUnitOfWork" /> class.
        /// </summary>
        public GenericUnitOfWork()
        {
            this._entities = new UmehlukoEntities1();
        }

        /// <summary>
        ///     The repositories.
        /// </summary>
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        ///     The repository.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IRepository" />.
        /// </returns>
        public IRepository<T> Repository<T>() where T : class
        {
            if (this._repositories.Keys.Contains(typeof(T)) == true)
            {
                return this._repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo = new GenericRepository<T>(this._entities);
            this._repositories.Add(typeof(T), repo);

            return repo;
        }

        /// <summary>
        /// The save changes.
        /// </summary>
        /// <exception cref="DbUpdateException">An error occurred sending updates to the database.</exception>
        public void SaveChanges()
        {
            this._entities.SaveChanges();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._entities.Dispose();
            }

            this._disposed = true;
        }
    }
}