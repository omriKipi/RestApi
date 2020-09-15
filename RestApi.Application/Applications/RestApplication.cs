﻿using RestApi.Common.Applications;
using RestApi.Common.Entities;
using RestApi.Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using RestApi.Shared.Resources;
using RestApi.Shared.Resources.Users;

namespace RestApi.Application
{
    public class RestApplication<TCreateResource, TResource, TEntity>
        : ApplicationBase, IRestApplication<TCreateResource, TResource>
        where TCreateResource : CreateResource
        where TResource : Resource
        where TEntity : Entity, new()
    {
        [Dependency]
        public IRepository<TEntity> Repository { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public UserResource CurrentUser { get; set; }

        public virtual async Task<ActionResult<IEnumerable<TResource>>> Get()
        {
            var entities = await Repository.GetAll();
            var resources = Mapper.Map<IEnumerable<TResource>>(entities);
            return Ok(resources);
        }

        public virtual async Task<ActionResult<TResource>> Get(string id)
        {
            var entity = await Repository.Get(id);
            if (entity == null)
                return NotFound(id);
            return await Map(entity);
        }

        public virtual async Task<ActionResult<TResource>> Create(TCreateResource createResource)
        {
            var entity = GetNewTEntity(createResource);
            await AddEntity(entity);
            return await Map(entity);
        }

        public virtual async Task<ActionResult<TResource>> Update(string id, TCreateResource createResource)
        {
            var entity = await Repository.Get(id);
            if (entity == null)
                return NotFound(id);
            Mapper.Map(createResource, entity);
            await UpdateEntity(entity);
            entity = await Repository.Get(entity.Id);
            return await Map(entity);
        }

        public virtual async Task<ActionResult> Delete(string id)
        {
            var entity = await Repository.Get(id);
            if (entity == null)
                return NotFound(id);
            return await DeleteEntity(entity);
        }

        public virtual async Task<ActionResult> Exists(string id)
        {
            return await Repository.Exists(id) ? 
                Ok() : 
                NotFound(id);
        }

        protected async virtual Task<TResource> Map(TEntity entity)
        {
            return Mapper.Map<TResource>(entity);
        }

        protected ActionResult<IEnumerable<TResource>> MapMany(IEnumerable<TEntity> entities)
        {
            return Ok(Mapper.Map<IEnumerable<TResource>>(entities));
        }

        protected virtual TEntity GetNewTEntity(TCreateResource resource)
        {
            return Mapper.Map<TEntity>(resource);
        }

        protected virtual async Task UpdateEntity(TEntity entity)
        {
            await Repository.Update(entity);
        }

        protected async virtual Task<ActionResult> DeleteEntity(TEntity entity)
        {
            await Repository.Delete(entity);
            return Ok();
        }

        protected virtual async Task AddEntity(TEntity entity)
        {
            await Repository.Add(entity);
        }

        protected async Task<bool> IsEntityExists<TFEntity>(string entityId)
            where TFEntity : Entity
        {
            var repository = UnityContainer.Resolve<IRepository<TFEntity>>();
            return await repository.Exists(entityId);
        }

        protected async Task<TFEntity> GetEntity<TFEntity>(string entityId)
            where TFEntity : Entity
        {
            var repository = UnityContainer.Resolve<IRepository<TFEntity>>();
            return await repository.Get(entityId);
        }
    }
}