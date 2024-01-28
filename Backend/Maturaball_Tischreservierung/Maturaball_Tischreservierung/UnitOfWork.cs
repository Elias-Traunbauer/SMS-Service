using Maturaball_Tischreservierung.Models;
using System;

namespace Maturaball_Tischreservierung
{
    public class UnitOfWork
    {
        private readonly DatabaseContext _appDbContext;
        private readonly ApiConfig _config;

        public UnitOfWork(ApiConfig config)
        {
            _appDbContext = new DatabaseContext(config);
            _config = config;
        }

        Semaphore Semaphore { get; set; } = new Semaphore(1, 1);
        bool manualTransaction = false;
        public void StartTransaction()
        {
            manualTransaction = true;
            _appDbContext.Database.BeginTransaction();
        }

        public async Task<bool> DeleteAsync<T>(T entity) where T : BaseDBEntity
        {
            Semaphore.WaitOne();
            try
            {
                var entityFromDb = await _appDbContext.FindAsync<T>(entity.Id);
                return entityFromDb != null && _appDbContext.Remove(entityFromDb) != null;
            }
            catch (Exception)
            {
                throw;
            }
            finally { Semaphore.Release(); }
        }

        public async Task InsertAsync<T>(T entity) where T : BaseDBEntity
        {
            Semaphore.WaitOne();
            try
            {
                await _appDbContext.AddAsync<T>(entity);
            }
            catch (Exception)
            {
                throw;
            }
            finally { Semaphore.Release(); }
        }

        public void Update<T>(T entity) where T : BaseDBEntity
        {
            _appDbContext.Update(entity);
        }

        // Method to get access to the DBSets
        public IQueryable<T> Query<T>() where T : BaseDBEntity
        {
            return _appDbContext.Set<T>().AsQueryable();
        }

        public async Task SaveChangesAsync(bool commit = true)
        {
            Semaphore.WaitOne();
            try
            {
                var freshDb = new DatabaseContext(_config);
                if (!manualTransaction || (manualTransaction && commit))
                {
                    // go through all modified entities and check for concurrency
                    // if the modified versionid is not the same as the one in the database, throw an exception
                    var updatedEntities = _appDbContext.ChangeTracker.Entries().Where(x => x.State == Microsoft.EntityFrameworkCore.EntityState.Modified).ToList();


                    foreach (var updatedEntity in updatedEntities)
                    {
                        var entity = (updatedEntity.Entity as BaseDBEntity)!;
                        var dbEntity = await freshDb.FindAsync(entity.GetType(), entity.Id) as BaseDBEntity;
                        if (dbEntity != null && dbEntity.Version != entity.Version)
                        {
                            throw new ConcurrencyException("Concurrency check failed.");
                        }

                        entity.Version = Guid.NewGuid();
                    }
                }

                await _appDbContext.SaveChangesAsync();
                if (commit && manualTransaction)
                    await _appDbContext.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (commit)
                {
                    manualTransaction = false;
                }
                Semaphore.Release();
            }
        }

        public void Dispose()
        {
            _appDbContext.ChangeTracker.Clear();
            if (manualTransaction && _appDbContext.Database.CurrentTransaction != null)
            {
                _appDbContext.Database.RollbackTransaction();
            }
        }

        public void DiscardChanges()
        {
            this._appDbContext.ChangeTracker.Clear();
        }

        public class ConcurrencyException : Exception
        {
            public ConcurrencyException(string message) : base(message)
            {
            }
        }
    }
}
