// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using QvaDev.FileContextCore;
using QvaDev.FileContextCore.Infrastructure.Internal;
using QvaDev.FileContextCore.Query.ExpressionVisitors.Internal;
using QvaDev.FileContextCore.Query.Internal;
using QvaDev.FileContextCore.Storage.Internal;
using QvaDev.FileContextCore.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace QvaDev.FileContextCore.Extensions
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class FileContextServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the in-memory database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />. You use this method when using dependency injection
        ///         in your application, such as with ASP.NET. For more information on setting up dependency
        ///         injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        ///     <para>
        ///         You only need to use this functionality when you want Entity Framework to resolve the services it uses
        ///         from an external dependency injection container. If you are not using an external
        ///         dependency injection container, Entity Framework will take care of creating the services it requires.
        ///     </para>
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services)
        ///         {
        ///             services
        ///                 .AddEntityFrameworkFileContext()
        ///                 .AddDbContext&lt;MyContext&gt;((serviceProvider, options) =>
        ///                     options.UseFileContext("MyDatabase")
        ///                            .UseInternalServiceProvider(serviceProvider));
        ///         }
        ///     </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkFileContext([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            EntityFrameworkServicesBuilder builder = new EntityFrameworkServicesBuilder(serviceCollection)
                .TryAdd<IDatabaseProvider, DatabaseProvider<FileContextOptionsExtension>>()
                .TryAdd<IValueGeneratorSelector, FileContextValueGeneratorSelector>()
                .TryAdd<IDatabase>(p => p.GetService<IFileContextDatabase>())
                .TryAdd<IDbContextTransactionManager, FileContextTransactionManager>()
                .TryAdd<IDatabaseCreator, FileContextDatabaseCreator>()
                .TryAdd<IQueryContextFactory, FileContextQueryContextFactory>()
                .TryAdd<IEntityQueryModelVisitorFactory, FileContextQueryModelVisitorFactory>()
                .TryAdd<IEntityQueryableExpressionVisitorFactory, FileContextEntityQueryableExpressionVisitorFactory>()
                .TryAdd<IEvaluatableExpressionFilter, EvaluatableExpressionFilter>()
                .TryAddProviderSpecificServices(b => b
                    .TryAddScoped<IFileContextStoreCache, FileContextStoreCache>()
                    .TryAddScoped<IFileContextTableFactory, FileContextTableFactory>()
                    .TryAddScoped<IFileContextDatabase, FileContextDatabase>()
                    .TryAddScoped<IMaterializerFactory, MaterializerFactory>()
					.TryAddScoped<FileContextIntegerValueGeneratorCache, FileContextIntegerValueGeneratorCache>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
