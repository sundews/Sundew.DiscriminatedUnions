// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpCodeFixVerifier`2+Test.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.Verifiers;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix, TSuppressor>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
    where TSuppressor : DiagnosticSuppressor, new()
{
    public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
    {
        public Test()
        {
            this.SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)?.CompilationOptions;
                compilationOptions = compilationOptions?.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                if (compilationOptions != null)
                {
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);
                }

                return solution;
            });
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            return base.GetDiagnosticAnalyzers().Concat(new DiagnosticAnalyzer[] { new TSuppressor() });
        }
    }
}