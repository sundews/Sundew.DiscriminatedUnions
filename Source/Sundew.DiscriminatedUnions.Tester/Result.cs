// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion(GeneratorFeatures.Segregate)]
public abstract class Result
{

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Success))]
    public static Result Success() => new Success();

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Warning))]
    public static Result Warning(string message) => new Warning(message);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Error))]
    public static Result Error(int code) => new Error(code);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(FatalError))]
    public static Result FatalError(int code) => new FatalError(code);
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

    public string Message { get; init; }
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract class ErrorResult : Result
{

    [CaseTypeAttribute(typeof(Error))]
    public new static ErrorResult Error(int code) => new Error(code);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(FatalError))]
    public new static ErrorResult FatalError(int code) => new FatalError(code);
}

public sealed class Error : ErrorResult
{
    public Error(int code)
    {
        this.Code = code;
    }

    public int Code { get; init; }
}

public sealed class FatalError : ErrorResult
{
    public FatalError(int code)
    {
        this.Code = code;
    }

    public int Code { get; init; }
}
