// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TradeSystem.FileContextCore.Infrastructure.Internal;
using TradeSystem.FileContextCore.ValueGeneration.Internal;

namespace TradeSystem.FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextTableFactory : IdentityMapFactoryFactoryBase, IFileContextTableFactory
    {
		private readonly FileContextIntegerValueGeneratorCache idCache;
		private FileContextOptionsExtension options;

		public FileContextTableFactory(FileContextIntegerValueGeneratorCache _idCache)
        {
			idCache = _idCache;
        }

        private readonly ConcurrentDictionary<IKey, Func<IFileContextTable>> _factories
            = new ConcurrentDictionary<IKey, Func<IFileContextTable>>();

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public virtual IFileContextTable Create(IEntityType entityType, FileContextOptionsExtension _options) {
			if (options == null)
			{
				options = _options;
			}

			return _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();
		}

        private Func<IFileContextTable> Create([NotNull] IKey key)
            => (Func<IFileContextTable>)typeof(FileContextTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, idCache, options });

        [UsedImplicitly]
        private static Func<IFileContextTable> CreateFactory<TKey>(IKey key, FileContextIntegerValueGeneratorCache idCache, FileContextOptionsExtension options)
            => () => new FileContextTable<TKey>(key.GetPrincipalKeyValueFactory<TKey>(), key.DeclaringEntityType, idCache, options);
    }
}
