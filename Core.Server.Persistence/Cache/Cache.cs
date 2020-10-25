﻿using Core.Server.Common.Cache;
using Core.Server.Common.Entities;
using Core.Server.Injection.Attributes;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Core.Server.Persistence.Cache
{
    [Inject]
    public class Cache<TEntity>
       : ICache<TEntity>
        where TEntity : Entity
    {
        private readonly Dictionary<string, TEntity> cache;

        public Cache()
        {
            cache = new Dictionary<string, TEntity>();
        }

        public TEntity Get(string id)
        {
            return cache.ContainsKey(id) 
                ? cache[id] 
                : null;
        }

        public IEnumerable<TEntity> Get(IEnumerable<string> ids)
        {
            foreach (var id in ids)
                yield return Get(id);
        }

        public IEnumerable<TEntity> Get( )
        {
            return cache.Values;
        }

        public void AddOrSet(TEntity entity)
        {
            if (cache.ContainsKey(entity.Id))
                cache[entity.Id] = entity;
            else
                cache.Add(entity.Id, entity);
        }

        public void AddOrSet(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                AddOrSet(entity);
        }

        public void Delete(string id)
        {
            if (cache.ContainsKey(id))
                cache.Remove(id);
        }

        public void Delete(IEnumerable<string> ids)
        {
            foreach (var id in ids)
                Delete(id);
        }

        public bool IsCached(string id)
        {
            return cache.ContainsKey(id);
        }

        public void Clear()
        {
            cache.Clear();
        }

        public bool Any()
        {
            return cache.Any();
        }
    }
}