// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpVerifierHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal static class CSharpVerifierHelper
{
    public const string RequiredTypes = @"
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Sundew.DiscriminatedUnions
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface)]
    internal class DiscriminatedUnion : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property)]
    internal class CaseTypeAttribute : System.Attribute
    {
        public CaseTypeAttribute(System.Type caseType)
        {
            this.CaseType = caseType;
        }

        public System.Type CaseType { get; }
    }

    internal class UnreachableCaseException : System.Exception
    {
        public UnreachableCaseException(System.Type enumType)
            : base($""{enumType.Name} is not a valid discriminated union."")
        {
        }
    }
}
";

    /// <summary>
    /// Gets the nullable warnings.
    /// By default, the compiler reports diagnostics for nullable reference types at
    /// <see cref="DiagnosticSeverity.Warning"/>, and the analyzer test framework defaults to only validating
    /// diagnostics at <see cref="DiagnosticSeverity.Error"/>. This map contains all compiler diagnostic IDs
    /// related to nullability mapped to <see cref="ReportDiagnostic.Error"/>, which is then used to enable all
    /// of these warnings for default validation during analyzer and code fix tests.
    /// </summary>
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        // Workaround for https://github.com/dotnet/roslyn/issues/41610
        nullableWarnings = nullableWarnings
            .SetItem("CS8632", ReportDiagnostic.Error)
            .SetItem("CS8669", ReportDiagnostic.Error);

        return nullableWarnings;
    }
}