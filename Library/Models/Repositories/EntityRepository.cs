﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Library.Models.Repositories
{
    public class EntityRepository<T> : IRepository<T>
        where T: class, Identifiable
    {
        protected LibraryContext db = new LibraryContext();

        protected Func<LibraryContext, DbSet<T>> dbSetFunc;

        public EntityRepository(Func<LibraryContext,DbSet<T>> dbSetFunc)
        {
            this.dbSetFunc = dbSetFunc;
        }

        virtual public T GetByID(int id)
        {
            return dbSetFunc(db).FirstOrDefault(p => p.Id == id);
        }

        virtual public IEnumerable<T> GetAll()
        {
            return dbSetFunc(db).ToList();
        }

        public int Create(T entity)
        {
            dbSetFunc(db).Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(int id)
        {
            var entity = GetByID(id);
            dbSetFunc(db).Remove(entity);
            db.SaveChanges();
        }

        public int Save()
        {
            return db.SaveChanges();
        }

        public void MarkModified(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}