﻿using Core.Server.Common.Entities;
using Core.Server.Shared.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Server.Common.Mappers
{
    public interface IResourceMapper<TResource, TEntity>
        where TResource : Resource
        where TEntity : Entity
    {
        Task<TResource> Map(TEntity entity);

        Task<IEnumerable<TResource>> Map(IEnumerable<TEntity> entities);
    }
}
