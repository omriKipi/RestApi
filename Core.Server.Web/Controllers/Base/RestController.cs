﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Server.Common.Applications;
using Unity;
using Microsoft.Extensions.Logging;
using Core.Server.Shared.Resources;
using Core.Server.Shared.Query;

namespace Core.Server.Web.Controllers
{
    public class RestController<TCreateResource, TUpdateResource, TResource, TApplication>
        : QueryController<TResource, TApplication>
        where TCreateResource : CreateResource
        where TUpdateResource : UpdateResource
        where TResource : Resource
        where TApplication: IRestApplication<TCreateResource, TUpdateResource, TResource>
    {
        [HttpPost]
        public virtual async Task<ActionResult<TResource>> Create(TCreateResource resource)
        {
            return await Application.Create(resource);
        }

        [HttpPut]
        public virtual async Task<ActionResult<TResource>> Update(TUpdateResource resource)
        {
            return await Application.Update(resource);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TResource>> Delete(string id)
        {
            return await Application.Delete(id);
        }
    }
}