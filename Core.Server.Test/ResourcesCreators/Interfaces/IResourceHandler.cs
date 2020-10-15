﻿using Core.Server.Client.Results;
using Core.Server.Shared.Resources;
using System.Collections.Generic;

namespace Core.Server.Tests.ResourceCreators.Interfaces
{
    public interface IResourceHandler<TResource> 
        : IResourceDeleter
        where TResource : Resource
    {
        ActionResult<TResource> Create();
        TResource Get();
        ActionResult<TResource> Get(string id);
        IEnumerable<TResource> GetAll();
    }
}
