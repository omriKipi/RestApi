﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Server.Common.Applications;
using Core.Server.Shared.Resources;
using Core.Server.Shared.Query;
using Core.Server.Common.Attributes;

namespace Core.Server.Web.Controllers
{
    [InjectBoundleController]
    public class QueryController<TResource>
        : BaseController<IQueryApplication<TResource>>
        where TResource : Resource
    {
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TResource>>> GetAll([FromQuery] string ids)
        {
            if (ids == null)
                return await Application.GetAll();
            else
                return await Application.GetByIds(ids.Split(','));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TResource>> GetById(string id)
        {
            return await Application.GetById(id);
        }

        [HttpPost("ids")]
        public virtual async Task<ActionResult<IEnumerable<TResource>>> GetByIds(string[] ids)
        {
            return await Application.GetByIds(ids);
        }

        [HttpPost("query")]
        public virtual async Task<ActionResult<IEnumerable<TResource>>> Query(QueryResource query)
        {
            return await Application.Query(query);
        }

        [HttpHead("{id}")]
        public virtual async Task<ActionResult> Exists(string id)
        {
            return await Application.Exists(id);
        }
    }
}