namespace Sundew.DiscriminatedUnions.Test
{
    public static class TestData
    {
        public const string Usings = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Sundew.DiscriminatedUnions;";

        public const string ResultDiscriminatedUnion = @"
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract class Result
        { 
            protected internal Result()
            { }
        }

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
        }";
    }
}