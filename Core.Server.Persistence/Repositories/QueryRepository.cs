﻿using MongoDB.Driver;
using Core.Server.Common;
using Core.Server.Common.Entities;
using Core.Server.Common.Query;
using Core.Server.Common.Repositories;
using Core.Server.Persistence.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication;
using System.Threading.Tasks;
using Unity;

namespace Core.Server.Persistence.Repositories
{
    public class QueryRepository<TEntity>
        : BaseRepository<TEntity>,
        IQueryRepository<TEntity>
        where TEntity : Entity
    {
        [Dependency]
        public IQueryFilterFactory QueryFilterFactory;

        public async Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return (await Collection.FindAsync(predicate)).ToEnumerable();
        }

        public async Task<TEntity> FindFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return (await Collection.FindAsync(predicate)).FirstOrDefault();
        }

        public async Task<TEntity> Get(string id)
        {
            return (await Collection.FindAsync(e => e.Id == id)).FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAll(IEnumerable<string> ids)
        {
            return (await Collection.FindAsync(e => ids.Contains(e.Id))).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAll(Func<TEntity, bool> findFunc)
        {
            return (await Collection.FindAsync(e => findFunc(e))).ToList();
        }
        public virtual async Task<IEnumerable<TEntity>> Get()
        {
            var answer = await Collection.FindAsync(e => true);
            return answer.ToEnumerable();
        }

        public async Task<bool> Exists(string id)
        {
            var answer = await Collection.FindAsync(e => e.Id == id);
            return answer.FirstOrDefault() != null;
        }

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate)
        {
            var answer = await Collection.FindAsync(predicate);
            return answer.FirstOrDefault() != null;
        }

        public async Task<IEnumerable<TEntity>> Query(QueryBase query)
        {
            var filter = QueryFilterFactory.GetFilter<TEntity>(query);
            return (await Collection.FindAsync(filter)).ToEnumerable();
        }
    }
}