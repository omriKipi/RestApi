﻿using Core.Server.Common.Applications;
using Core.Server.Common.Attributes;
using Core.Server.Common.Entities;
using Core.Server.Common.Mappers;
using Core.Server.Common.Query;
using Core.Server.Common.Query.Infrastructure;
using Core.Server.Common.Repositories;
using Core.Server.Shared.Errors;
using Core.Server.Shared.Query;
using Core.Server.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

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
        public IQueryValidator QueryValidator;

        [Dependency]
        public IQueryResourceValidator QueryResourceValidator;

        [Dependency]
        public IResourceMapper<TResource, TEntity> ResourceMapper;

        [Dependency]
        public IQueryResourceMapper QueryResourceMapper;

        [Dependency]
        public IQueryRepository<TEntity> QueryRepository;

        public virtual async Task<ActionResult<IEnumerable<TResource>>> Query(QueryResource queryResource)
        {
            var queryRequest = QueryResourceMapper.Map<TResource>(queryResource);

            var validateResult = Validate(queryRequest);
            if (validateResult != null)
                return BadRequest((BadRequestReason)validateResult);

            var entities = await QueryRepository.Query(queryRequest);
            var response = await ResourceMapper.Map(entities);
            return response.ToList();
        }

        private BadRequestReason? Validate(QueryRequest queryRequest)
        {
            var queryResourceValidation = QueryResourceValidator.Validate<TResource>(queryRequest);
            var queryPhraseValidation = QueryValidator.Validate<TResource>(queryRequest.Query);
            return queryResourceValidation ?? queryPhraseValidation;
        }
    }
}