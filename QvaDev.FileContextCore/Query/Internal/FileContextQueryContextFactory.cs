// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using QvaDev.FileContextCore.Infrastructure.Internal;
using QvaDev.FileContextCore.Storage.Internal;

namespace QvaDev.FileContextCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextQueryContextFactory : QueryContextFactory
    {
        private readonly IFileContextStore _store;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextQueryContextFactory(
            [NotNull] QueryContextDependencies dependencies,
            [NotNull] IFileContextStoreCache storeCache,
            [NotNull] IDbContextOptions contextOptions)
            : base(dependencies)
        {
            _store = storeCache.GetStore(contextOptions.Extensions.OfType<FileContextOptionsExtension>().First());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override QueryContext Create()
            => new FileContextQueryContext(Dependencies, CreateQueryBuffer, _store);
    }
}
