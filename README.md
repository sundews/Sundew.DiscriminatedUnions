# Sundew.DiscriminatedUnions

A poor man's implementation of discriminated unions for C# until a future version of C# provides it out of the box. The goal here is to create a standin replacement, so that when DUs are available switch statements/expressions are near compatible (See issues below).

## How it works
A Roslyn analyzer asserts and report errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), the base class constructor is private. Similarly all case classes should be sealed.

Create discriminated unions by inheriting from an abstract base class marked with the DiscriminatedUnion attribute to build various cases.

## Sample
### Defining a DU
```
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{
    private Result()
    { }

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}
```

### Evaluation
```
var message = result switch
{
    Result.Error { Code: > 70 } error => $"High Error code: {error.Code}",
    Result.Error error => $"Error code: {error.Code}",
    Result.Warning { Message: "Tough warning" } => "Not good",
    Result.Warning warning => warning.Message,
    Result.Success => "Great",
};
```

## Features:
- Analyzer and CodeFix for incomplete switch statements and expressions
- Analyzer and CodeFix for unsealed cases classes
- Analyzer and CodeFix for non-private default constructor
- Analyzer for any other declared non-private constructor
- F# equivalent Option&lt;T&gt;

## Issues/Todos
* The analyzer is compatible with C# 9, however currently it references are more recent version (3.11) of the Microsoft.CodeAnalysis package, meaning it will fail to load on Visual Studio versions shipping with older ones. Still need to figure out with version first shipped with C# 9 support.
* Experimental generics support, but still have some issues with nullability.
* SourceGenerator not yet implemented: https://github.com/dotnet/csharplang/blob/main/proposals/discriminated-unions.md
* When installing via NuGet the package must be manually added to all projects that evaluate DUs.
