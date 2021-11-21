# Dimensional Unions

Inspired by F#, DimensionalUnions started out as a poor man's implementation of discriminated unions for C#. During development  until a future version of C# provides it out of the box. The goal here is to create a standin replacement, so that when DUs are available switch statements/expressions are near compatible (See issues below).

## How it works
A Roslyn analyzer asserts and report errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), an analyzer ensure unions are not derived from. Similarly all case classes should be sealed.

Create a union by inheriting from an abstract base class marked with the Union attribute to build various cases.

## Sample
### Defining a union
```csharp
[DimensionalUnions.Union]
public abstract record Result
{
    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}
```
Alternatively, a union can be defined with unnested case classes, allowing the possibility of creating dimensional unions (see below).

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

### Dimensional unions
To support dimensional unions, unnested cases help because the cases are no longer defined inside a union. However, for this to work the unions are required to declare a factory method named exactly like the case type, that consists of a single return statement that creates the cases. The return type of a factory method should match the union in which it is declared. 
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IExpression
{
    public static IExpression AddExpression(IExpression lhs, IExpression rhs) => new AddExpression(lhs, rhs);

    public static IExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    public static IExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    public static IExpression DivisionExpression(IExpression lhs, IExpression rhs) => new DivisionExpression(lhs, rhs);
 
    public static IExpression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IArithmeticExpression : IExpression
{
    public static IArithmeticExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    public static IArithmeticExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    public static IArithmeticExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    public static IArithmeticExpression DivisionExpression(IExpression lhs, IExpression rhs) => new DivisionExpression(lhs, rhs);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface ICommutativeExpression : IArithmeticExpression
{
    public static ICommutativeExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    public static ICommutativeExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);
}

public sealed record AdditionExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed record SubtractionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record MultiplicationExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed record DivisionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record ValueExpression(int Value) : IExpression;
```

## Supported diagnostics:
| Diagnostic Id | Description                                                            | Code Fix |
| ------------- | ---------------------------------------------------------------------- | :------: |
| SDU0001       | Switch does not handled all cases                                      |   yes    |
| SDU0002       | Switch should not handle default case                                  |   yes    |
| SDU0003       | Switch has unreachable null case                                       |   yes    |
| SDU0004       | Class unions must be abstract                                          |   yes    |
| SDU0005       | Union extensions must be declared in the same assembly as their unions |   no     |
| SDU0006       | Cases must be declared in the same assembly as their unions            |   no     |
| SDU0007       | Cases should be sealed                                                 |   yes    |
| SDU0008       | Unnested cases should have factory method                              |   yes    |
| SDU9999       | Switch should throw in default case                                    |   no     |

## Issues/Todos
* Switch appears with red squiggly lines in VS: https://github.com/dotnet/roslyn/issues/57041
* Nullability is falsely evaluated when the switch hints null is possible: https://github.com/dotnet/roslyn/issues/57042
* SDU0009 gets reported in VS, but no code fix is offered. VS issue here: https://github.com/dotnet/roslyn/issues/57621
* SourceGenerator not yet implemented: https://github.com/dotnet/csharplang/blob/main/proposals/discriminated-unions.md