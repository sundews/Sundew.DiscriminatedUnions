// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseTypeAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions
{
    using System;

    /// <summary>
    /// Indicates the case type constructed by a factory method.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    internal class CaseTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseTypeAttribute"/> class.
        /// </summary>
        /// <param name="caseType">The case type.</param>
        public CaseTypeAttribute(Type caseType)
        {
            this.CaseType = caseType;
        }

        /// <summary>
        /// Gets the case type.
        /// </summary>
        public Type CaseType { get; }
    }
}