// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

using System.Collections.Generic;
using Sundew.DiscriminatedUnions.Development.TestData;
using Sundew.DiscriminatedUnions.Development.Tester;

public static class Program
{
    public static void Main(string[] args)
    {
        var types = new List<DefiniteType>
        {
            DefiniteType.NamedType("1", "2", "3"), DefiniteType.DefiniteArrayType(DefiniteType.NamedType("2", "3", "4")),
        };

        var result = types.Segregate();

        var expression = new AdditionExpression(new ValueExpression(5), new SubtractionExpression(new ValueExpression(3), new ValueExpression(1)));

        /*static int Evaluate(IExpression expression)
        {
            return expression switch
            {
                AdditionExpression additionExpression => throw new System.NotSupportedException(),
                SubtractionExpression subtractionExpression => throw new System.NotSupportedException(),
                MultiplicationExpression multiplicationExpression => throw new NotSupportedException(),
                ValueExpression valueExpression => valueExpression.Value,
            };
        }

        Console.WriteLine(Evaluate(expression));*/
        /*
        Console.WriteLine("Hello World!");
        Result? result = Compute("Error");
        var message = result switch
        {
            Error { Code: > 70 } error => $"Error code: {error.Code}",
            FatalError fatalError => throw new System.NotImplementedException(),
            Error error => $"Error code: {error.Code}",
            Warning { Message: "Tough warning" } => "Not good",
            Warning warning => warning.Message,
            Success => "Great",
            null => "dd",
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
            case FatalError fatalError:
                throw new System.NotImplementedException();
            case null:
                break;
        }

        Console.WriteLine(result);

        SwitchReturn(result);
        Switch(result);*/
    }

    /*
    private static bool SwitchReturn(Result? result)
    {
        switch (result)
        {
            case Error error:
                return false;
            case FatalError fatalError:
                throw new System.NotImplementedException();
            case Warning { Message: "Tough Warning" } warning:
                return false;
            case Warning warning:
                return true;
            case Success:
                return true;
            case null:
                throw new System.NotImplementedException();
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
            case FatalError fatalError:
                throw new System.NotImplementedException();
            case Warning warning:
                break;
            case Success:
                break;
            case null:
                throw new System.NotImplementedException();
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
    }*/
}