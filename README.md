# Discriminated Unions

Sundew.DiscriminatedUnions implement discriminated unions for C#, until a future version of C# provides it out of the box.
The idea is that this package can be deleted once unions are supported in C#, without requiring changes to switch expressions and statements.

In addition, the project tries to imagine unions in an object oriented language an adds an aspect of dimensions to unions.
A dimensional union is a union where cases can be reused in any number of unions, by supporting interface unions through the possibility of implementing multiple interface and default interface members.

## How it works
A Roslyn analyzer asserts and report errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), an analyzer ensure that unions are not derived from in referencing assemblies.
Similarly all case classes should be sealed.

Create a union by inheriting from an abstract base (record) class (or interface) marked with the Union attribute to build various cases.

## Sample
### Defining a union
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{
    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}
```
Alternatively, a union can be defined with unnested case classes and interfaces, allowing the possibility of creating dimensional unions (see below).

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
To support dimensional unions, unnested cases help because the cases are no longer defined inside a union. However, for this to work the unions are required to declare a factory method named exactly like the case type, that consists of a single return statement that creates the cases. The return type of a factory method should match the union in which it is declared. A code fix (PDU0001) is available to generate the factory methods. 
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IExpression
{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static IExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static IExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(DivisionExpression))]
    public static IExpression DivisionExpression(IExpression lhs, IExpression rhs) => new DivisionExpression(lhs, rhs);
 
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static IExpression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IArithmeticExpression : IExpression
{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public new static IArithmeticExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public new static IArithmeticExpression SubtractionExpression(IExpression lhs, IExpression rhs) => new SubtractionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static IArithmeticExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(DivisionExpression))]
    public new static IArithmeticExpression DivisionExpression(IExpression lhs, IExpression rhs) => new DivisionExpression(lhs, rhs);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface ICommutativeExpression : IArithmeticExpression
{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public new static ICommutativeExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ICommutativeExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);
}

public sealed record AdditionExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed record SubtractionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record MultiplicationExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed record DivisionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record ValueExpression(int Value) : IExpression;
```

## Supported diagnostics:
| Diagnostic Id | Description                                                            | Code Fix  |
| ------------- | ---------------------------------------------------------------------- | :-------: |
| SDU0001       | Switch does not handled all cases                                      |   yes     |
| SDU0002       | Switch should not handle default case                                  |   yes     |
| SDU0003       | Switch has unreachable null case                                       |   yes     |
| SDU0004       | Class unions must be abstract                                          |   yes     |
| SDU0005       | Only unions can extended other unions                                  |   no      |
| SDU0006       | Unions cannot be extended outside their assembly                       |   no      |
| SDU0007       | Cases must be declared in the same assembly as their unions            |   no      |
| SDU0008       | Cases should be sealed                                                 |   yes     |
| SDU0009       | Unnested cases should have factory method                              |   PDU0001 |
| SDU0010       | Factory method should have correct CaseTypeAttribute                   |   yes     |
| PDU0001       | Populate union factory methods                                         |   yes     |
| SDU9999       | Switch should throw in default case                                    |   no      |

## Issues/Todos
* Switch appears with red squiggly lines in VS: https://github.com/dotnet/roslyn/issues/57041
* Nullability is falsely evaluated when the switch hints null is possible: https://github.com/dotnet/roslyn/issues/57042
* SDU0009 gets reported in VS, but no code fix is offered. VS issue here: https://github.com/dotnet/roslyn/issues/57621
  * Workaround: A PDU0001 is always reported on unions offering to generate factory methods, as it is not technically possible to implement a code fix for SDU0009 (See issue).
* Verbose syntax for defining unions. (No SourceGenerators implemented yet)