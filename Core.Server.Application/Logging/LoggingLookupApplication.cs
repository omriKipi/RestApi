﻿using Core.Server.Common.Applications;
using Core.Server.Common.Entities;
using Core.Server.Common.Attributes;
using Core.Server.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace Core.Server.Application.Logging
{
    [Inject(2)]
    public class LoggingLookupApplication<TResource, TEntity>
        : LoggingApplication<LookupApplication<TResource, TEntity>>
        , ILookupApplication<TResource>
        where TResource : Resource
        where TEntity : Entity
    {
        public Task<ActionResult> Any()
        {
            return CallApplicationWithLog(() => Application.Any());
        }

        public Task<ActionResult> Exists(string id)
        {
            return CallApplicationWithLog(() => Application.Exists(id));
        }

        public Task<ActionResult<IEnumerable<TResource>>> GetAll()
        {
            return CallApplicationWithLog(() => Application.GetAll());
        }

        public Task<ActionResult<TResource>> GetById(string id)
        {
            return CallApplicationWithLog(() => Application.GetById(id), id);
        }

        public Task<ActionResult<IEnumerable<TResource>>> GetByIds(string[] ids)
        {
            return CallApplicationWithLog(() => Application.GetByIds(ids), ids);
        }
    }
}
