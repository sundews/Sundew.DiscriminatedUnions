# Discriminated Unions

Sundew.DiscriminatedUnions was originally introduced to provide unions in C# until the language supported them. With the release of C# 15, this is now the case, and many of the use cases covered by this package can now be replaced with the built-in union declarations and closed hierarchies.

Since this library has gained a number of features that the built-in unions do not support, it has been updated to build on top of the C# 15 unions and closed hierarchies.

Sundew.DiscriminatedUnions generates allocation free struct unions for C# 15, which the compiler matches exhaustively like union declarations, but without boxing cases that are structs.
It also implements discriminated unions for class hierarchies and enums, with analyzers that check that switch statements and switch expressions handle all cases.
The compiler only checks switch expressions over unions and closed types, so the analyzer checks the switch statements over them as well.

In addition, the project supports dimensional unions through default interface methods (traits).
A dimensional union is a union where cases can be reused in any number of unions, by supporting interface unions through the possibility of implementing multiple interface and default interface members.

## What is supported
The library works with five kinds of unions. The first three are the ones C# 15 offers, which the library adds to. The last two are the library's own and are checked by its analyzer.

| | [Struct union](#struct-unions) | [Union declaration](#union-declarations) | [Closed type](#closed-types) | [Class union](#class-unions) | [Enum](#enums) |
| --- | :---: | :---: | :---: | :---: | :---: |
| Declared as | `partial struct` that lists `ICase<TCase>` | `union` | `closed` class or record | abstract class, record or interface with `[DiscriminatedUnion]` | enum with `[DiscriminatedUnion]` |
| [Switch expressions are checked by](#checked-switch-expressions) | the compiler | the compiler | the compiler | the analyzer | the analyzer |
| [Switch statements are checked by](#checked-switch-statements) | the analyzer | the analyzer | the analyzer | the analyzer | the analyzer |
| [Cases that are structs are stored without boxing](#cases-without-boxing) | yes | no | – | – | – |
| [Factories that return the union](#factories-that-return-the-union) | yes | when partial | when partial | when partial | – |
| [`Cases` and `IDiscriminatedUnion`](#cases-and-idiscriminatedunion) | yes | when partial | when partial | when partial | – |
| [Equality and `ToString`](#equality-and-tostring) | yes | when partial | – | – | – |
| [`Segregate`](#segregate) | yes | yes | yes | yes | – |
| [A case can be in several unions](#a-case-in-several-unions) | yes | yes | no | interfaces | – |
| [A union can be a case of a union](#unions-of-unions) | no | no | yes | yes | – |

A dash means that it does not apply: the cases of a closed type and of a class union are classes, which are not boxed and bring their own equality, and nothing is generated for an enum.

## Features
The examples use these unions:

```csharp
public readonly record struct Circle(double Radius);
public readonly record struct Square(double Side);

[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public readonly partial struct Shape : ICase<Circle>, ICase<Square>;

[DiscriminatedUnion]
public abstract partial record Result
{
    public sealed partial record Success : Result;

    public sealed partial record Error(int Code) : Result;
}
```

### Checked switch expressions
A switch expression that does not handle every case is reported. For a struct union, a union declaration and a closed type the compiler does that:

```csharp
var area = shape switch
{
    Circle circle => Math.PI * circle.Radius * circle.Radius,
}; // warning CS8509: The switch expression does not handle all possible values of its input type. For example, the pattern 'Square' is not covered.
```

For a class union and an enum the analyzer does:

```csharp
var text = result switch
{
    Result.Success => "ok",
}; // error SDU0001: 'Error' case in 'Result' is not handled.
```

### Checked switch statements
The compiler only checks switch expressions: a switch statement that leaves out a case compiles without a warning. The analyzer reports SDU0001 for it, for every kind of union:

```csharp
switch (shape)
{
    case Circle circle:
        Console.WriteLine(circle.Radius);
        break;
} // error SDU0001: 'Square' case in 'Shape' is not handled.
```

```csharp
switch (result)
{
    case Result.Success:
        Console.WriteLine("ok");
        break;
} // error SDU0001: 'Error' case in 'Result' is not handled.
```

```csharp
[DiscriminatedUnion]
public enum Color { Red, Green }
```

```csharp
switch (color)
{
    case Color.Red:
        break;
} // error SDU0001: 'Green' case in 'Color' is not handled.
```

For a struct union, a union declaration and a closed type the check needs no attribute, and it follows what the compiler asks of a switch expression:
* A case is handled by a pattern for its type or for a type it converts to, so `case Exception:` also handles a case that is an `InvalidOperationException`. A case with a `when` clause or a condition, such as `case Circle { Radius: > 1 }:`, does not handle the case.
* A `default` section handles every case that is not listed. Add one where a switch statement handles only some of the cases on purpose.
* `null` has to be handled where the compiler asks a switch expression to handle it: where the value can be null according to nullable analysis, and where a case type of the union is nullable.

The code fix for SDU0001 adds a section for each case that is missing.

### Cases without boxing
A union declaration stores its case as an `object`, so a case that is a struct is boxed when it is converted to the union. A struct union stores it in the struct:

```csharp
public union BoxedShape(Circle, Square);
```

```csharp
Shape shape = new Circle(2);      // Stored in the struct, which is 16 bytes. Nothing is allocated.
BoxedShape boxed = new Circle(2); // Stored as an object: the Circle is boxed, which allocates 24 bytes.
```

[Storage](#storage) describes how a struct union lays out its cases.

### Factories that return the union
A case converts to its union where a union is asked for. Where the type is inferred it stays the case, and two different cases have no type in common. A factory creates the union:

```csharp
var circle = Shape.Circle(2);                            // A Shape, where new Circle(2) is a Circle.
var shapes = new[] { Shape.Circle(2), Shape.Square(3) }; // A Shape[].
var failed = Result._Error(404);                         // A Result. The underscore tells the factory from the case Result.Error.
```

[Factories](#factories) lists which cases get a factory.

### `Cases` and `IDiscriminatedUnion`
`Cases` lists the case types of a union, which a union declaration and a closed type do not have at runtime. On .NET 8 and later the union also implements `IDiscriminatedUnion`, so that generic code can ask any union for its cases:

```csharp
foreach (var caseType in Shape.Cases)
{
    Console.WriteLine(caseType.Name); // Circle, Square
}

static IReadOnlyList<Type> GetCases<TUnion>()
    where TUnion : IDiscriminatedUnion
{
    return TUnion.Cases;
}
```

### Equality and `ToString`
A struct union and a partial union declaration are equal when the cases they contain are, and they print the case:

```csharp
Shape first = new Circle(2);
Shape second = new Circle(2);

Console.WriteLine(first == second); // True
Console.WriteLine(first);           // Circle { Radius = 2 }
```

### `Segregate`
`GeneratorFeatures.Segregate` on the attribute generates an extension method that sorts the unions in an enumerable into a list per case:

```csharp
var segregation = shapes.Segregate(); // A ShapeSegregation
IReadOnlyList<Circle> circles = segregation.Circles;
IReadOnlyList<Square> squares = segregation.Squares;
```

[Generator features](#generator-features) has the details.

### A case in several unions
A struct union and a union declaration list their cases, so a type can be a case of any number of them:

```csharp
public readonly record struct Found(int Value);
public sealed record NotFound(string Key);
public sealed record TimedOut(TimeSpan After);

public readonly partial struct LookupResult : ICase<Found>, ICase<NotFound>;

public readonly partial struct RemoteLookupResult : ICase<Found>, ICase<NotFound>, ICase<TimedOut>;
```

A class has a single base class, so a class that is a case of a closed type or of a class union is a case of that one. A class union that is an interface has no such limit, which is what makes [dimensional unions](#dimensional-unions) possible:

```csharp
[DiscriminatedUnion]
public partial interface IReadError;

[DiscriminatedUnion]
public partial interface IWriteError;

public sealed partial record AccessDenied : IReadError, IWriteError;

public sealed partial record EndOfFile : IReadError;

public sealed partial record DiskFull : IWriteError;
```

### Unions of unions
A closed type can derive from a closed type, and a class union from a class union. A switch over the outer union then handles the inner union as a whole or case by case:

```csharp
public closed partial record Animal;

public closed partial record Mammal : Animal;

public sealed record Dog : Mammal;

public sealed record Cat : Mammal;

public sealed record Fish : Animal;
```

```csharp
var text = animal switch
{
    Mammal mammal => Describe(mammal), // Handles Dog and Cat.
    Fish => "fish",
};
```

`Animal.Cases` lists `Dog`, `Cat` and `Fish`. A struct union or a union declaration that is a case of another union is one case of it: the compiler does not match through to its cases.

## Struct unions
C# 15 comes with union declarations: `union Result(Success, Error);`. A union stores its case in a single `object`, so a case that is a struct is boxed every time it is converted to the union.
A struct union has the shape the compiler recognizes as a union, which means that cases convert implicitly and that switches are checked exhaustively, but it stores its cases in fields and nothing is boxed.
Struct unions are cumbersome to write by hand, so a source generator writes them. Declare a partial struct that lists its cases with `ICase<TCase>`:

```csharp
public readonly record struct Empty<TItem>;
public readonly record struct Single<TItem>(TItem Item);
public readonly record struct Multiple<TItem>(IEnumerable<TItem> Items);

public readonly partial struct ListCardinality<TItem>
    : IDefaultCase<Empty<TItem>>,
      ICase<Single<TItem>>,
      ICase<Multiple<TItem>>;
```

```csharp
ListCardinality<int> cardinality = new Single<int>(4);

var text = cardinality switch
{
    Empty<int> => "none",
    Single<int> single => $"one: {single.Item}",
    Multiple<int> multiple => $"many: {multiple.Items.Count()}",
};
```

The generator adds a constructor, a `TryGetValue` method and a factory per case, `Value`, `HasValue`, equality (`IEquatable<T>`, `==` and `!=`), `GetHashCode`, `ToString` and `Cases`. On .NET 8 and later the struct also implements `IDiscriminatedUnion`.

### The default value
A struct always has a default value. What it means for a struct union is decided by the declaration:
* `IDefaultCase<TCase>` declares the case that the default value represents. It must be a struct without instance fields, and there can be only one. The union then always contains a case.
* Without a default case the default value contains no case: `HasValue` is `false` and it only matches `null`, so a switch needs a `null` arm to handle it. The compiler does not ask for that arm, so the analyzer reports SDU0013 where such a default value is created.

A reference case that is `null` is treated the same way: it creates a union without a value, or throws an `ArgumentNullException` when the union has a default case and therefore no such value.

### Cases that are related types
The compiler matches a union by the type of the value it contains and not by the case it was created from. So when the case types of a union are related, a value is every case that its type converts to:

```csharp
public readonly partial struct Failure : ICase<Exception>, ICase<InvalidOperationException>, ICase<int>;
```

An `InvalidOperationException` matches both `InvalidOperationException` and `Exception`, whichever of the two the union was created from it as, and both ways of creating it give equal unions. In a switch the more specific case comes first, which the compiler insists on. The same goes for a case that implements an interface that is a case as well.

### Cases that are nullable value types
A case can be declared as a nullable value type, such as `ICase<int?>`. The union stores the `int` and hands it out as an `int`. It is created from an `int?`, and `null` creates it without a value, the way a `null` reference does.

### Factories
`new Single<int>(4)` is a `Single<int>`. It converts to the union where a `ListCardinality<int>` is asked for, but with `var`, in a lambda or as a generic type argument it stays a `Single<int>`, and two different cases have no type in common. A factory returns the union:

```csharp
var cardinality = ListCardinality<int>.Single(4);
var other = items.Count == 0 ? ListCardinality<int>.Empty : ListCardinality<int>.Multiple(items);
```

* A case gets a factory named after its type when it is a class or a struct that is declared in the same assembly as the union.
* The factory is a method with the parameters of the constructor of the case that takes the most parameters, or a property when that constructor takes none.
* Cases that share a name, such as `Quantity<int>` and `Quantity<double>`, get factories that overload each other.
* A factory is not generated when the union declares a member of that name itself.

### Storage
* A case without instance fields needs no storage.
* Cases that contain no references share their storage, so the union is as large as the largest of them. This requires explicit layout, which the runtime does not allow for generic types. For a generic union the generator therefore declares the shared storage as a struct of its own next to the union, and it holds the cases that do not depend on a type parameter. A case that depends on a type parameter, such as `Single<TItem>`, has a field of its own.
* Cases that are reference types share a single reference field.
* Any other case is stored in a field of its own.

When all cases are reference types, a union declaration does not box anything and is smaller than a struct union. The generator reports GDU0003 and generates nothing in that case.

### Requirements
* The C# 15 compiler, which is part of the .NET 11 SDK.
* `UnionAttribute` and `IUnion`, which are part of .NET 11. For older target frameworks they can be provided by a polyfill such as PolySharp.

### Opting out
`[DiscriminatedUnion(disableGenerator: true)]` on the struct turns the generator off, so that the members can be written by hand.

## Union declarations
A union declaration only comes with the value it contains. Declare it partial, and the source generator adds what a union lacks:

```csharp
public partial union Result(Success, Error);
```

* `Cases`, the list of case types, which a union declaration does not have at runtime. On .NET 8 and later the union also implements `IDiscriminatedUnion`.
* Equality (`IEquatable<T>`, `==` and `!=`) and `GetHashCode`, which compare the contained cases.
* `ToString`, which describes the contained case.
* A factory per case, as for struct unions: `Result.Success(value)` is a `Result`, where `new Success(value)` is a `Success`.

A member that the union declares itself is not generated, and `[DiscriminatedUnion(disableGenerator: true)]` turns the generator off for the union. The same applies to struct unions.

Struct unions and union declarations can also be segregated, see [Generator features](#generator-features). The attribute only configures the source generator for them: the compiler checks these unions, so the analyzer does not treat them as class unions. What the compiler leaves unchecked, the analyzer checks with or without the attribute: [switch statements](#checked-switch-statements).

## Closed types
The compiler knows the cases of a closed class or record and checks that a switch handles them all. Declare it partial, and the source generator adds a factory per case, which returns the case as the closed type, and the list of cases:

```csharp
public closed partial record Shape
{
    public sealed record Circle(double Radius) : Shape;
}

public sealed record Square(double Side) : Shape;
```

```csharp
var circle = Shape._Circle(2.5); // A Shape, where new Shape.Circle(2.5) is a Shape.Circle.
var square = Shape.Square(4);
```

* The factories follow the same rules as those of struct unions.
* The factory of a case that is nested in the closed type starts with an underscore, because the name of the case is taken by the case itself.
* A case that is a closed type itself is replaced by its own cases. When it is partial as well, both get a factory for those cases, each returning its own type.
* `Cases` is the list of case types, which a closed type does not have at runtime: the types a switch has to handle. On .NET 8 and later the closed type also implements `IDiscriminatedUnion`. It is not generated when the closed type declares a member of that name itself.
* A case that is partial as well gets a `Cases` of its own, which lists the case itself, as the cases of class unions do. A case that is not partial has the list of the closed type it derives from.
* A closed type can be segregated like the other unions, see [Generator features](#generator-features).
* `[DiscriminatedUnion(disableGenerator: true)]` turns the generator off for the closed type. The attribute does not make the analyzer treat a closed type as a class union, because the compiler already checks it. [Switch statements](#checked-switch-statements) over a closed type are checked by the analyzer with or without the attribute.

## Class unions
A class union is an abstract class, an abstract record or an interface that has the `DiscriminatedUnion` attribute. It is what this library offered before C# 15. It remains the way to get unions with an older compiler, [checked switch statements](#checked-switch-statements) and [dimensional unions](#dimensional-unions).

A Roslyn analyzer asserts and report errors in case switch statements or switch expression do not handle all cases.
C# 8 and 9 already comes with great pattern matching support for evaluation.

In order that the inheritance hierarchy remain closed (All cases in the same assembly), an analyzer ensures that unions are not derived from in referencing assemblies.
Similarly all case classes should be sealed.

Create a union by inheriting from an abstract base (record) class (or interface) marked with the DiscriminatedUnion attribute to build various cases.
Either specify the partial keyword to the union for a source generator to implement factory methods or use the codefix PDU0001 to generate them.

### Defining a union
```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record Result
{
    public sealed partial record Success : Result;

    public sealed partial record Warning(string Message) : Result;

    public sealed partial record Error(int Code) : Result;
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
To support dimensional unions, unnested cases help because the cases are no longer defined inside a union. However, for this to work the unions are required to declare a factory method named exactly like the case type and that has the CaseType attribute specifying the actual type.
Since version 3, factory methods are generated when the union is declared partial. Alternatively, a code fix (PDU0001) is available to generate the factory methods. 

```csharp
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public partial interface IExpression;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public partial interface IArithmeticExpression : IExpression;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public partial interface ICommutativeExpression : IArithmeticExpression;

public sealed partial record AdditionExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed partial record SubtractionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed partial record MultiplicationExpression(IExpression Lhs, IExpression Rhs) : ICommutativeExpression;

public sealed partial record DivisionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed partial record ValueExpression(int Value) : IExpression;
```

#### Evaluating dimensional unions
With dimensional unions it is possible to handle all cases using a sub union.
As seen in the example below, handling the ArithmeticExpression covers Addition-, Subtraction-, Multiplication- and DivisionExpression.
Typically one would dispatch these to a method handling ArithmeticExpression and where handling all cases would be checked, but it is not required.
This makes it convienient to separate handling logic in smaller chucks of code.

```csharp
public int Evaluate(Expression expression)
{
    return expression switch
        {
            ArithmeticExpression arithmeticExpression => Evaluate(arithmeticExpression),
            ValueExpression valueExpression => valueExpression.Value,
        };
}

public int Evaluate(ArithmeticExpression arithmeticExpression)
{
    return arithmeticExpression switch
        {
            AdditionExpression additionExpression => Evaluate(additionExpression.Lhs) + Evaluate(additionExpression.Rhs),
            SubtractionExpression subtractionExpression => Evaluate(subtractionExpression.Lhs) - Evaluate(subtractionExpression.Rhs),
            MultiplicationExpression multiplicationExpression => Evaluate(multiplicationExpression.Lhs) * Evaluate(multiplicationExpression.Rhs),
            DivisionExpression divisionExpression => Evaluate(divisionExpression.Lhs) / Evaluate(divisionExpression.Rhs),
        };
}
```

### Enums
As of version 5.1, regular enums can also use the DiscriminatedUnion attribute causing the analyzer to exhaustively check switch statements and expressions.

## Generator features
As mentioned a source generator is automatically activated for generating factory methods when the partial keyword is specified.
In addition, the DiscriminatedUnion attribute can specify a flags enum (GeneratorFeatures) to control additional code generation.

* Segregate - Generates an extension method for IEnumerable<TUnion> that segregates all items into buckets of the different result.

Segregate is available for every kind of union: class unions, struct unions, union declarations and closed types.

```csharp
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public partial union Result(Success, Error);
```

```csharp
var segregation = results.Segregate(); // A ResultSegregation
var successes = segregation.Successes; // An IReadOnlyList<Success>
var errors = segregation.Errors;       // An IReadOnlyList<Error>
```

* The generator adds two types to the namespace of the union: `ResultSegregation`, which has a list per case, and `ResultExtensions`, which declares the `Segregate` extension method. A type of that name that the project declares itself has to be partial.
* The lists are named after the case types. Cases that share a name are told apart by their type arguments, so `Quantity<int>` and `Quantity<double>` get `QuantityOfInt32s` and `QuantityOfDoubles`.
* The attribute is all it takes: a union declaration or a closed type does not have to be partial to be segregated.
* An item that contains no case is left out: `null`, and the default value of a struct union without a default case.
* A union declaration keeps the value and not the case it was created from, so with cases that derive from each other an item goes to the most specific case it is.
* The generated types are public when the union and all of its cases are, and internal otherwise. A union cannot be segregated when it or one of its cases is hidden within a type, because the generated types could not refer to it. The generator reports GDU0007 for it.

The source generator can be turned off for a union with `[DiscriminatedUnion(disableGenerator: true)]`.

## Supported diagnostics:
| Diagnostic Id | Description                                                                                                               | Code Fix |
| ------------- | ------------------------------------------------------------------------------------------------------------------------- | :------: |
| SDU0001       | Switch does not handled all cases                                                                                         |   yes    |
| SDU0002       | Switch should not handle default case                                                                                     |   yes    |
| SDU0003       | Switch has unreachable null case                                                                                          |   yes    |
| SDU0004       | Class unions must be abstract                                                                                             |   yes    |
| SDU0005       | Only unions can extended other unions                                                                                     |    no    |
| SDU0006       | Unions cannot be extended outside their assembly                                                                          |    no    |
| SDU0007       | Cases must be declared in the same assembly as their unions                                                               |    no    |
| SDU0008       | Cases should be sealed                                                                                                    |   yes    |
| SDU0009       | Unnested cases should have factory method                                                                                 | PDU0001  |
| SDU0010       | Factory method should have correct CaseTypeAttribute                                                                      |   yes    |
| SDU0011       | Reported when a case is implemented by throwing NotImplementedException, because CodeCleanup may siliently 'fix' SDU0001. |   yes    |
| SDU0012       | Reported when a case contains type parameters that are not in the union type parameter list.                              |   yes    |
| SDU0013       | The default value of a struct union without a default case contains no case                                               |    no    |
| PDU0001       | Make union/case partial for code generator                                                                                |   yes    |
| PDU0002       | Populate union factory methods                                                                                            |   yes    |
| SDU9999       | Switch should throw in default case                                                                                       |    no    |
| GDU0001       | Discriminated union declaration could not be found                                                                        |    no    |
| GDU0002       | Struct unions must be partial structs                                                                                     |    no    |
| GDU0003       | A union declaration is the better choice when all cases are reference types                                               |    no    |
| GDU0004       | Struct unions can only have one default case                                                                              |    no    |
| GDU0005       | The default case of a struct union must be a struct without instance fields                                               |    no    |
| GDU0006       | A type parameter cannot be a case of a struct union                                                                       |    no    |
| GDU0007       | A union that is segregated and its cases must be accessible from the namespace of the union                               |    no    |

## Issues/Todos
* Switch appears with red squiggly lines in VS: https://github.com/dotnet/roslyn/issues/57041
* Nullability is falsely evaluated when the switch hints null is possible: https://github.com/dotnet/roslyn/issues/57042