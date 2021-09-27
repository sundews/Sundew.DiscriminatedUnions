# Sundew.DiscriminatedUnions

A poor man's implementation of discriminated unions for C# until a future version of C# provides it out of the box. The goal here is to create a standin replacement, so that when DUs are available switch statements/expressions are near compatible (See issues below).

## How it works
A Roslyn analyzer asserts and reports errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), the base class constructor is private protected. Similarly all case classes should be sealed.
There are thoughts to implement source generators for the unions, so no analyzers will be needed.

Create discriminated unions by inheriting from an abstract base class marked with the DiscriminatedUnion attribute to build various cases.

## Sample
### Defining a DU
```
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract class Result
{
    private protected Result()
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
```

### Evaluation
```
var message = result switch
{
    Error { Code: > 70 } error => $"High Error code: {error.Code}",
    Error error => $"Error code: {error.Code}",
    Warning { Message: "Tough warning" } => "Not good",
    Warning warning => warning.Message,
    Success => "Great",
    _ => throw new Sundew.DiscriminatedUnions.DiscriminatedUnionException(typeof(Result)),
};
```

## Issues/Todos
* Adding a case class (meaning all switches doesn't handle all cases) correctly fails the build, but with no errors. Restarting VS is needed in order to correctly report the diagnostic.
* Handling the default case is currently necessary to silence ```CS8509 The switch expression does not handle all possible values of its input type (it is not exhaustive). For example, the pattern '_' is not covered.```
  Currently not aware of a way the analyzer can silence other diagnostics. Help anyone?
* The analyzer is compatible with C# 9, however currently it references are more recent version (3.11) of the Microsoft.CodeAnalysis package, meaning it will fail to load on Visual Studio versions shipping with older ones. Still need to figure out with version first shipped with C# 9 support.
* No generics support as of now
* CodeFixes not yet implemented
* SourceGenerator not yet implemented: https://github.com/dotnet/csharplang/blob/main/proposals/discriminated-unions.md
* When installing via NuGet the package must be manually added to all projects that evaluate DUs.
