﻿using Core.Server.Common.Applications;
using Core.Server.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using Core.Server.Shared.Resources;
using Core.Server.Shared.Query;
using Core.Server.Shared.Errors;
using Core.Server.Common.Mappers;
using Core.Server.Injection.Attributes;
using Core.Server.Application.Query;

namespace Core.Server.Application
{
    [Inject]
    public class QueryApplication<TResource, TEntity>
        : BaseApplication<TEntity>,
          IQueryApplication<TResource>
        where TResource : Resource
        where TEntity : Entity
    {
        [Dependency]
        public IQueringBuilder QueringBuilder;

        [Dependency]
        public IQueringValidator QueringValidator;

        [Dependency]
        public IResourceMapper<TResource, TEntity> ResourceMapper;

        public virtual async Task<ActionResult<IEnumerable<TResource>>> Query(QueryResource queryResource)
        {
            var validationError = QueringValidator.Validate<TResource>(queryResource);
            if (validationError != null)
                return BadRequest((BadRequestReason)validationError);

            var query = QueringBuilder.Build<TResource>(queryResource);
            var entities = await QueryRepository.Query(query);
            return Ok(await ResourceMapper.Map(entities));
        }
    }
}