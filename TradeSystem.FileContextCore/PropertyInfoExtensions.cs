// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TradeSystem.FileContextCore
{
    [DebuggerStepThrough]
    internal static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo property)
            => (property.GetMethod ?? property.SetMethod).IsStatic;

        public static bool IsCandidateProperty(this PropertyInfo propertyInfo, bool needsWrite = true)
            => !propertyInfo.IsStatic()
               && propertyInfo.GetIndexParameters().Length == 0
               && propertyInfo.CanRead
               && (!needsWrite || propertyInfo.CanWrite)
               && propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic;

        public static Type FindCandidateNavigationPropertyType(this PropertyInfo propertyInfo, Func<Type, bool> isPrimitiveProperty)
        {
            var targetType = propertyInfo.PropertyType;
            var targetSequenceType = targetType.TryGetSequenceType();
            if (!propertyInfo.IsCandidateProperty(targetSequenceType == null))
            {
                return null;
            }

            targetType = targetSequenceType ?? targetType;
            targetType = targetType.UnwrapNullableType();

            if (isPrimitiveProperty(targetType)
                || targetType.GetTypeInfo().IsInterface
                || targetType.GetTypeInfo().IsValueType
                || targetType == typeof(object))
            {
                return null;
            }

            return targetType;
        }

        public static bool IsSameAs(this PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo)
        {
            Check.NotNull(propertyInfo, nameof(propertyInfo));
            Check.NotNull(otherPropertyInfo, nameof(otherPropertyInfo));

            return Equals(propertyInfo, otherPropertyInfo)
                   || propertyInfo.Name == otherPropertyInfo.Name
                   && (propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType
                       || propertyInfo.DeclaringType.GetTypeInfo().IsSubclassOf(otherPropertyInfo.DeclaringType)
                       || otherPropertyInfo.DeclaringType.GetTypeInfo().IsSubclassOf(propertyInfo.DeclaringType)
                       || propertyInfo.DeclaringType.GetTypeInfo().ImplementedInterfaces.Contains(otherPropertyInfo.DeclaringType)
                       || otherPropertyInfo.DeclaringType.GetTypeInfo().ImplementedInterfaces.Contains(propertyInfo.DeclaringType));
        }

        public static PropertyInfo FindGetterProperty([NotNull] this PropertyInfo propertyInfo)
            => propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.Name)
                .FirstOrDefault(p => p.GetMethod != null);

        public static PropertyInfo FindSetterProperty([NotNull] this PropertyInfo propertyInfo)
            => propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.Name)
                .FirstOrDefault(p => p.SetMethod != null);
    }
}
