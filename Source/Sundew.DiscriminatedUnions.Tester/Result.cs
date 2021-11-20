// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract class Result
{
    private protected Result()
    {
    }
}
#pragma warning disable SA1402

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
    private protected ErrorResult()
    {
    }
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
#pragma warning restore SA1402