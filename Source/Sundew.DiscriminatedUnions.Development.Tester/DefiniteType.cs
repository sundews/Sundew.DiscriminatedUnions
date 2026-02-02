// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnions.DiscriminatedUnion(generatorFeatures: GeneratorFeatures.Segregate)]
public abstract partial record DefiniteType(string Name, string Namespace, string? AssemblyName)
{
    public virtual string FullName => $"{this.Namespace}.{this.Name}";

    public string AssemblyQualifiedName => $"{this.FullName}, {this.AssemblyName}";
}

public sealed partial record NamedType(string Name, string Namespace, string? AssemblyName) : DefiniteType(Name, Namespace, AssemblyName);

public sealed partial record DefiniteArrayType(DefiniteType ElementType) : DefiniteType(ElementType.Name, ElementType.Namespace, ElementType.AssemblyName)
{
    public override string FullName => $"{this.Namespace}.{this.Name}";
}