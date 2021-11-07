# Sundew.DiscriminatedUnions

A poor man's implementation of discriminated unions for C# until a future version of C# provides it out of the box. The goal here is to create a standin replacement, so that when DUs are available switch statements/expressions are near compatible (See issues below).

## How it works
A Roslyn analyzer asserts and report errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), the base class constructor is private. Similarly all case classes should be sealed.

Create discriminated unions by inheriting from an abstract base class marked with the DiscriminatedUnion attribute to build various cases.

## Sample
### Defining a DU
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{
    private protected Result()
    { }

    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}
```
Alternatively, a DU can be defined with unnested case classes (see below).

### Evaluation
```csharp
var message = result switch
{
    Result.Error { Code: > 70 } error => $"High Error code: {error.Code}",
    Result.Error error => $"Error code: {error.Code}",
    Result.Warning { Message: "Tough warning" } => "Not good",
    Result.Warning warning => warning.Message,
    Result.Success => "Great",
};
```

### Unnested cases and subunions
To support subunions, unnested cases help because the cases are no longer defined inside a discriminated union. However, for this to work the unions are required to declare a factory method named exactly like the case type, that consists of a single return statement that creates the cases. The return type of a factory method should match the discriminated union in which it is declared. 
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{
    private protected Expression()
    { }

    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
 
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{
    private protected ArithmeticExpression()
    { }

    public static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs)
    {
        return new SubtractExpression(lhs, rhs);
    }
}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;
```

## Supported diagnostics:
| Diagnostic Id | Description                                                       | Code Fix |
| ------------- | ----------------------------------------------------------------- | :------: |
| SDU0001       | Switch does not handled all cases                                 |   yes    |
| SDU0002       | Switch should not handle default case                             |   yes    |
| SDU0003       | Switch has unreachable null case                                  |   yes    |
| SDU0004       | Discriminated unions can only have private protected constructors |   yes    |
| SDU0005       | Discriminated unions must have private protected constructor      |   yes    |
| SDU0006       | Class discriminated unions must be abstract                       |   yes    |
| SDU0007       | Interface discriminated unions must be internal                   |   yes    |
| SDU0008       | Cases should be sealed                                            |   yes    |
| SDU0009       | Unnested cases should have factory method                         |   yes    |
| SDU9999       | Switch should throw in default case                               |    no    |

## Issues/Todos
* Switch appears with red squiggly lines in VS: https://github.com/dotnet/roslyn/issues/57041
* Nullability is falsely evaluated when the switch hints null is possible: https://github.com/dotnet/roslyn/issues/57042
* SDU0009 gets reported in VS, but no code fix is offered. VS issue here: https://github.com/dotnet/roslyn/issues/57621
* SourceGenerator not yet implemented: https://github.com/dotnet/csharplang/blob/main/proposals/discriminated-unions.md