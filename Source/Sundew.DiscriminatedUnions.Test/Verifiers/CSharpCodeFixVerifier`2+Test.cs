// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpCodeFixVerifier`2+Test.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Sundew.DiscriminatedUnions.Analyzer;

    public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix, TSuppressor>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
        where TSuppressor : DiagnosticSuppressor, new()
    {
        public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
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
}
