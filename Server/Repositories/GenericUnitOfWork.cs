using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Repositories
{
    public class GenericUnitOfWork : IDisposable
    {
        private FaceRecognitionDatabaseEntities _db = null;
        public GenericUnitOfWork()
        {
            _db = new FaceRecognitionDatabaseEntities();
        }
        // Słownik będzie używany do sprawdzania instancji repozytoriów
        public Dictionary<Type, object> Repositories = new Dictionary<Type, object>();
        public IRepository<T> Repository<T>() where T : class
        {
            // Jeżeli instancja danego repozytorium istnieje - zostanie zwrócona
            if (Repositories.Keys.Contains(typeof(T)) == true)
                return Repositories[typeof(T)] as IRepository<T>;
            // Jeżeli nie, zostanie utworzona nowa i dodana do słownika
            IRepository<T> repo = new GenericRepository<T>(_db);
            Repositories.Add(typeof(T), repo);
            return repo;
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                    _db.Dispose();
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}