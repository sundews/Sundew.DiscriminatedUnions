using System;

namespace Sundew.DiscriminatedUnions.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Result result = Compute(false);
            var isSucesss = result switch
            {
                Error error => error.Code.ToString(),
                Success success => success.Message,
            };

            Console.WriteLine(isSucesss);
        }

        private static Result Compute(bool isSuccess)
        {
            return isSuccess ? new Success("It worked") : new Error(-1);
        }
    }

    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract class Result
    { }

    public class Success : Result
    {
        public Success(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }
    }

    public class Error : Result
    {
        public Error(int code)
        {
            this.Code = code;
        }

        public int Code { get; private set; }
    }
}
