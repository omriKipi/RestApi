﻿using Core.Server.Client.Results;
using Core.Server.Shared.Resources;
using System;

namespace Core.Server.Tests.ResourceCreators.Interfaces
{
    public interface IResourceAlter<TCreateResource, TUpdateResource, TResource>
        where TCreateResource : CreateResource
        where TUpdateResource : UpdateResource
        where TResource : Resource
    {
        ActionResult<TResource> Create();
        ActionResult<TResource> Create(Action<TCreateResource> editFunc);
        ActionResult<TResource> Replace();
        ActionResult<TResource> Replace(Action<TUpdateResource> editFunc);
        ActionResult<TResource> Update(Action<TUpdateResource> editFunc);
    }
}