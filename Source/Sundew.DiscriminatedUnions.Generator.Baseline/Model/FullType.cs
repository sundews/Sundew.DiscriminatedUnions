// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullType.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Model;

using System;

internal readonly record struct FullType(string Name, string Namespace, string AssemblyAlias, bool IsArray, TypeMetadata TypeMetadata) : IEquatable<Type>
{
    public FullType(Type type, TypeMetadata typeMetadata)
        : this(type.Name, type.Namespace, type.AssemblyAlias, type.IsArray, typeMetadata)
    {
    }

    public bool Equals(Type other)
    {
        return this.Name.Equals(other.Name) &&
               this.Namespace.Equals(other.Namespace) &&
               this.AssemblyAlias.Equals(other.AssemblyAlias) &&
               this.IsArray.Equals(other.IsArray);
    }
}