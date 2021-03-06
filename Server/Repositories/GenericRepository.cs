﻿using Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Server.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private FaceRecognitionDatabaseEntities _db = null;
        // obiekt reprezentuje kolekcję wszystkich encji w danym kontekście
        // lub może być wynikiem zapytania z bazy danych
        IDbSet<T> _objectSet;
        public GenericRepository(FaceRecognitionDatabaseEntities db)
        {
            this._db = db;
            _objectSet = db.Set<T>();
        }
        public void Add(T entity)
        {
            _objectSet.Add(entity);
        }

        public void DeleteAll()
        {
            foreach (var entity in _objectSet)
            {
                _objectSet.Remove(entity);
            }
        }

        public IEnumerable<T> GetOverview(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
                return _objectSet.Where(predicate);
            return _objectSet.AsEnumerable();
        }
    }
}