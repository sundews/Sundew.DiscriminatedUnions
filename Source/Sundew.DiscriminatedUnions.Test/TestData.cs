// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    public static class TestData
    {
        public const string ConsoleApplication1Result = "ConsoleApplication1.Result";
        public const string ConsoleApplication1OptionInt = "ConsoleApplication1.Option<int>";

        public const string Usings = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Sundew.DiscriminatedUnions;";

        public const string ValidResultDiscriminatedUnion = @"
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract record Result
        { 
            private Result()
            { }

            public sealed record Success : Result;

            public sealed record Warning(string Message) : Result;

            public sealed record Error(int Code) : Result;
        }";

        public const string ValidGenericOptionalDiscriminatedUnion = @"
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract record Option<T>
            where T : notnull
        {
            private Option()
            { }

            public sealed record Some(T Value) : Option<T>;

            public sealed record None : Option<T>;
        }";
    }
}