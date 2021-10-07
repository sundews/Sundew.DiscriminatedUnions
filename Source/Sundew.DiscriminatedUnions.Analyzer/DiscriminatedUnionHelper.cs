// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal static class DiscriminatedUnionHelper
    {
        public static bool IsDiscriminatedUnionSwitch([NotNullWhen(true)] ITypeSymbol? unionType)
        {
            if (unionType == null || unionType.TypeKind != TypeKind.Class)
            {
                return false;
            }

            return IsDiscriminatedUnion(unionType);
        }

        public static bool IsDiscriminatedUnion(ITypeSymbol unionType)
        {
            return unionType.IsAbstract && unionType.GetAttributes().Any(attribute =>
            {
                var containingClass = attribute.AttributeClass?.ToDisplayString();
                return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
            });
        }
    }
}