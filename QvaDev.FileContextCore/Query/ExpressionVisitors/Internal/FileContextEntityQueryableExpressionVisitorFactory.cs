// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using QvaDev.FileContextCore.Query.Internal;
using Remotion.Linq.Clauses;

namespace QvaDev.FileContextCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        private readonly IModel _model;
        private readonly IMaterializerFactory _materializerFactory;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextEntityQueryableExpressionVisitorFactory(
            [NotNull] IModel model,
            [NotNull] IMaterializerFactory materializerFactory)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(materializerFactory, nameof(materializerFactory));

            _model = model;
            _materializerFactory = materializerFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ExpressionVisitor Create(
            EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource)
            => new FileContextEntityQueryableExpressionVisitor(
                _model,
                _materializerFactory,
                Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)),
                querySource);
    }
}
