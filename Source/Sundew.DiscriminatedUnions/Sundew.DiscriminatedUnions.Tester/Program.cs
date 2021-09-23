using System;

namespace Sundew.DiscriminatedUnions.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Result result = Compute("Error");
            var message = result switch
            {
                Error { Code: > 70 } error => $"Error code: {error.Code}",
                Error error => $"Error code: {error.Code}",
                Warning { Message: "Tough warning" } => "Not good",
                Warning warning => warning.Message,
                Success => "Great",
                _ => throw new DiscriminatedUnionException(result),
            };

            switch (result)
            {
                case Success:
                    break;
                case Warning { Message: "Tough warning" } warning:
                    break;
                case Warning:
                    break;
                case Error:
                    break;
                default:
                    break;
            }

            Console.WriteLine(result);

            SwitchReturn(result);
            Switch(result);
        }

        private static bool SwitchReturn(Result result)
        {
            switch (result)
            {
                case Error error:
                    return false;
                case Warning { Message: "Tough Warning" } warning:
                    return false;
                case Warning warning:
                    return true;
                case Success:
                    return true;
                default:
                    throw new DiscriminatedUnionException(result);
            }
        }

        private static void Switch(Result result)
        {
            switch (result)
            {
                case Error error:
                    break;
                case Warning warning:
                    break;
                case Success:
                    break;
            }
        }

        private static Result Compute(string magic)
        {
            return magic switch
            {
                "Success" => new Success(),
                "Error" => new Error(65),
                _ => new Warning(magic),
            };
        }
    }

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
    }
}
