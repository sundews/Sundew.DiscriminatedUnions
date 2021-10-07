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
        public abstract class Result
        { 
            private Result()
            { }

            public sealed class Success : Result
            {
            }

            public sealed class Warning : Result
            {
                public Warning(string message)
                {
                    this.Message = message;
                }

                public string Message { get; private set; }
            }

            public sealed class Error : Result
            {
                public Error(int code)
                {
                    this.Code = code;
                }

                public int Code { get; private set; }
            }
        }";

        public const string ValidGenericOptionalDiscriminatedUnion = @"
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract record Option<T>
            where T : notnull
        {
            public sealed record Some : Option<T>
            {
                public Some(T value)
                {
                    Value = value;
                }

                public T Value { get; }
            }

            public sealed record None : Option<T>
            {
            }
        }";
    }
}