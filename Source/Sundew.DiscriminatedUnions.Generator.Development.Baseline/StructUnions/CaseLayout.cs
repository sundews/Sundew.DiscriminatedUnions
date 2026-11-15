// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseLayout.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using System.Globalization;

/// <summary>
/// Describes where a generated struct union keeps a case.
/// </summary>
/// <param name="Case">The case.</param>
/// <param name="Tag">The tag that identifies the case. The default case has the tag 0, which is also what an uninitialized union has.</param>
/// <param name="Storage">The storage.</param>
/// <param name="Identifier">An identifier for the case that is unique within the union.</param>
internal readonly record struct CaseLayout(StructUnionCase Case, int Tag, CaseStorage Storage, string Identifier)
{
    private const string FieldPrefix = "__case";
    private const string BoxedFieldPrefix = "__boxed";

    /// <summary>
    /// Gets the tag as it is written in generated code.
    /// </summary>
    public string TagLiteral => this.Tag.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the name of the field that stores the case, when it is stored in a field of its own.
    /// </summary>
    public string FieldName => FieldPrefix + this.Identifier;

    /// <summary>
    /// Gets the name of the static field that holds the boxed case, when the case has no payload.
    /// </summary>
    public string BoxedFieldName => BoxedFieldPrefix + this.Identifier;

    /// <summary>
    /// Gets the expression that reads the case from the specified instance. It is only valid when the instance contains the case.
    /// </summary>
    /// <param name="instance">The expression for the instance to read from.</param>
    /// <returns>The expression.</returns>
    public string GetReadExpression(string instance)
    {
        return this.Storage switch
        {
            CaseStorage.Field => this.Case.Kind == StructUnionCaseKind.Reference ? $"{instance}.{this.FieldName}!" : $"{instance}.{this.FieldName}",
            CaseStorage.UnmanagedField => $"{instance}.{StructUnionSourceBuilder.UnmanagedFieldName}.{this.Identifier}",
            CaseStorage.ReferenceField => $"(({this.Case.TypeName}){instance}.{StructUnionSourceBuilder.ReferenceFieldName}!)",
            _ => $"default({this.Case.TypeName})",
        };
    }
}
