﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Library.Models
{
    // TODO add some tests for this stuff
    public class InMemoryRepository<T> : IRepository<T>
        where T: Identifiable
    {
        private IDictionary<int, T> entities = new Dictionary<int, T>();
        public int Create(T entity)
        {
            T copy = DeepClone(entity);
            if (entity.Id == 0)
                entity.Id = NextId();
            entities[entity.Id] = entity;
            return entity.Id;
        }

        // See http://stackoverflow.com/questions/17065264/create-copy-of-object
        private T DeepClone(T original)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "original");
            if (ReferenceEquals(original, null))
                return default(T);
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter
                { Context = new StreamingContext(StreamingContextStates.Clone) };
                formatter.Serialize(stream, original);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        private int NextId()
        {
            return entities.Keys.Max() + 1;
        }

        public void Delete(int id)
        {
            entities.Remove(id);
        }

        public void Dispose()
        {
        }

        public IEnumerable<T> GetAll()
        {
            return entities.Values;
        }

        public T GetByID(int id)
        {
            return entities[id];
        }

        public void MarkModified(T entity)
        {
        }

        public int Save()
        {
            return 0;
        }
    }
}