﻿using Core.Server.Common.Cache;
using Core.Server.Common.Entities;
using Core.Server.Common.Repositories;
using Core.Server.Injection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Unity;

namespace Core.Server.Persistence.Cache
{
    [Inject]
    public class AlterCache<TEntity>
        :IAlterCachedRepository<TEntity>
        where TEntity : Entity
    {
        [Dependency]
        public ICache<TEntity> Cache;

        [Dependency]
        public IAlterRepository<TEntity> AlterRepository;

        public async Task Add(TEntity entity)
        {
            await AlterRepository.Add(entity);
            Cache.AddOrSet(entity);
        }

        public async Task DeleteMany(Expression<Func<TEntity, bool>> predicate)
        {
            await AlterRepository.DeleteMany(predicate);
            Cache.Clear();
        }

        public async Task DeleteOne(Expression<Func<TEntity, bool>> predicate)
        {
            await AlterRepository.DeleteOne(predicate);
            Cache.Clear();
        }

        public async Task Replace(TEntity entity)
        {
            await AlterRepository.Replace(entity);
            Cache.AddOrSet(entity);
        }

        public async Task Update(TEntity entity)
        {
            await AlterRepository.Update(entity);
            Cache.Delete(entity.Id);
        }

        public async Task Delete(TEntity entity)
        {
            await AlterRepository.Delete(entity);
            Cache.Delete(entity.Id);
        }
    }
}