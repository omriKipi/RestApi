﻿using Core.Server.Common.Cache;
using Core.Server.Common.Config;
using Core.Server.Common.Entities;
using Core.Server.Common.Repositories;
using Core.Server.Injection.Interfaces;
using System.Threading.Tasks;
using Unity;

namespace Core.Server.Injection.Cache
{
    public class UnityCacheBuilder
    {
        public void AddCache(IUnityContainer unityContainer, IReflactionHelper reflactionHelper)
        {
            unityContainer.RegisterType(typeof(ICacheEntityConfig<>), typeof(CacheEntityConfig<>));

            var cacheConfig = unityContainer.Resolve<CacheConfig>();

            if (cacheConfig.Exclude != null)
                AddExcludedEntities(unityContainer, reflactionHelper, cacheConfig);
            if (cacheConfig.Preload != null)
                AddPreloadEntities(unityContainer, reflactionHelper, cacheConfig);
        }

        private void AddExcludedEntities(IUnityContainer unityContainer, IReflactionHelper reflactionHelper, CacheConfig cacheConfig)
        {
            foreach (var excludedEntityName in cacheConfig.Exclude)
            {
                var excludedType = reflactionHelper.GetTypeByName(excludedEntityName + nameof(Entity));
                var interfaceType = typeof(IEntityCache<>).MakeGenericType(excludedType);
                var excludedCacheType = typeof(ExcludedCache<>).MakeGenericType(excludedType);
                unityContainer.RegisterType(interfaceType, excludedCacheType);
            }
        }

        private void AddPreloadEntities(IUnityContainer unityContainer, IReflactionHelper reflactionHelper, CacheConfig cacheConfig)
        {
            Parallel.ForEach(cacheConfig.Preload, preloadEntityName =>
            {
                var preloadType = reflactionHelper.GetTypeByName(preloadEntityName + nameof(Entity));
                var lookupRepositoryType = typeof(ILookupRepository<>).MakeGenericType(preloadType);
                var lookupRepository = unityContainer.Resolve(lookupRepositoryType);
                lookupRepositoryType.InvokeMember("Get", System.Reflection.BindingFlags.InvokeMethod, null, lookupRepository, null);
            });
        }
    }
}
