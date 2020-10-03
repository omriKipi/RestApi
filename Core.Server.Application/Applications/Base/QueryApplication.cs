﻿using Core.Server.Common.Applications;
using Core.Server.Common.Entities;
using Core.Server.Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using Core.Server.Shared.Resources;
using Core.Server.Shared.Resources.Users;
using Core.Server.Shared.Query;
using Core.Server.Application.Helpers;
using Core.Server.Shared.Errors;

namespace Core.Server.Application
{
    public class QueryApplication<TResource, TEntity>
        : ApplicationBase, IQueryApplication< TResource>
        where TResource : Resource
        where TEntity : Entity, new()
    {
        [Dependency]
        public IQueryRepository<TEntity> QueryRepository { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency]
        public IQueryBuilder QueryBuilder;

        public UserResource CurrentUser { get; set; }

        public virtual async Task<ActionResult<IEnumerable<TResource>>> Get()
        {
            var entities = await QueryRepository.Get();
            var resources = Mapper.Map<IEnumerable<TResource>>(entities);
            return Ok(resources);
        }

        public virtual async Task<ActionResult<TResource>> Get(string id)
        {
            var entity = await QueryRepository.Get(id);
            if (entity == null)
                return NotFound(id);
            return await Map(entity);
        }

        public virtual async Task<ActionResult<IEnumerable<TResource>>> Query(QueryResource queryResource)
        {
            var validationError = QueryBuilder.Validate<TResource>(queryResource);
            if (validationError != null)
                return BadRequest((BadRequestReason)validationError);

            var query = QueryBuilder.Build<TResource>(queryResource);
            var entities = await QueryRepository.Query(query);
            var resources = Mapper.Map<IEnumerable<TResource>>(entities);
            return Ok(resources);
        }

        public virtual async Task<ActionResult> Exists(string id)
        {
            return await QueryRepository.Exists(id) ?
                Ok() :
                NotFound(id);
        }

        protected async virtual Task<TResource> Map(TEntity entity)
        {
            return Mapper.Map<TResource>(entity);
        }

        protected async Task<ActionResult<IEnumerable<TResource>>> MapMany(IEnumerable<TEntity> entities)
        {
            return Ok(Mapper.Map<IEnumerable<TResource>>(entities));
        }

        protected async Task<bool> IsEntityExists<TFEntity>(string entityId)
            where TFEntity : Entity
        {
            var repository = UnityContainer.Resolve<IRestRepository<TFEntity>>();
            return await repository.Exists(entityId);
        }

        protected async Task<TFEntity> GetEntity<TFEntity>(string entityId)
            where TFEntity : Entity
        {
            var repository = UnityContainer.Resolve<IRestRepository<TFEntity>>();
            return await repository.Get(entityId);
        }
    }
}