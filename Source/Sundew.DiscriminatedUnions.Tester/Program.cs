// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tester;

using System;

public static class Program
{
    public static void Main(string[] args)
    {
        var expression = new AddExpression(new ValueExpression(5),
            new SubtractExpression(new ValueExpression(3), new ValueExpression(1)));

        int Evaluate(Expression expression)
        {
            return expression switch
            {
                AddExpression addExpression => Evaluate(addExpression.Lhs) + Evaluate(addExpression.Rhs),
                SubtractExpression subtractExpression => Evaluate(subtractExpression.Lhs) - Evaluate(subtractExpression.Rhs),
                ValueExpression valueExpression => valueExpression.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(expression))
            };
        }
            
        Console.WriteLine(Evaluate(expression));

        Console.WriteLine("Hello World!");
        Result? result = Compute("Error");
        var message = result switch
        {
            Error { Code: > 70 } error => $"Error code: {error.Code}",
            Error error => $"Error code: {error.Code}",
            Warning { Message: "Tough warning" } => "Not good",
            Warning warning => warning.Message,
            Success => "Great",
            null => "dd",
            _ => throw new UnreachableCaseException(typeof(Result)),
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
            case null:
                break;
            default:
                break;
        }

        Console.WriteLine(result);

        SwitchReturn(result);
        Switch(result);
    }

    private static bool SwitchReturn(Result? result)
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
                throw new UnreachableCaseException(typeof(Result));
        }
    }

    private static void Switch(Result? result)
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

    private static Result? Compute(string magic)
    {
        return magic switch
        {
            "Success" => new Success(),
            "Error" => new Error(65),
            "null" => null,
            _ => new Warning(magic),
        };
    }
}