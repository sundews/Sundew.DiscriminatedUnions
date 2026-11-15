// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultCase{TCase}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    /// <summary>
    /// Declares the case of a struct union that the default value of the union represents.
    /// At most one case can be the default case and it must be a struct without instance fields.
    /// A struct union without a default case reports no value for its default value.
    /// </summary>
    /// <typeparam name="TCase">The case type.</typeparam>
    internal interface IDefaultCase<TCase> : ICase<TCase>
    {
    }
}
