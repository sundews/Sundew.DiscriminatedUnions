// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseTypeAttribute.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions;

using System;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
internal class CaseTypeAttribute : Attribute
{
    public CaseTypeAttribute(Type caseType)
    {
        this.CaseType = caseType;
    }

    public Type CaseType { get; }
}