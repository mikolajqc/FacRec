using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Repositories
{
    public class GenericUnitOfWork : IDisposable
    {
        private FaceRecognitionDatabaseEntities db = null;
        public GenericUnitOfWork()
        {
            db = new FaceRecognitionDatabaseEntities();
        }
        // Słownik będzie używany do sprawdzania instancji repozytoriów
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        public IRepository<T> Repository<T>() where T : class
        {
            // Jeżeli instancja danego repozytorium istnieje - zostanie zwrócona
            if (repositories.Keys.Contains(typeof(T)) == true)
                return repositories[typeof(T)] as IRepository<T>;
            // Jeżeli nie, zostanie utworzona nowa i dodana do słownika
            IRepository<T> repo = new GenericRepository<T>(db);
            repositories.Add(typeof(T), repo);
            return repo;
        }
        public void SaveChanges()
        {
            db.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    db.Dispose();
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}