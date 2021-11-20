namespace Sundew.DiscriminatedUnions.TestData;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IExpression
{
    public static IExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    public static IExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);

    public static IExpression ValueExpression(int value) => new ValueExpression(value);
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IArithmeticExpression : IExpression
{
    IExpression Lhs { get; init; }

    IExpression Rhs { get; init; }

    public new static IArithmeticExpression AdditionExpression(IExpression lhs, IExpression rhs) => new AdditionExpression(lhs, rhs);

    public new static IArithmeticExpression MultiplicationExpression(IExpression lhs, IExpression rhs) => new MultiplicationExpression(lhs, rhs);
}

public sealed record AdditionExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record MultiplicationExpression(IExpression Lhs, IExpression Rhs) : IArithmeticExpression;

public sealed record ValueExpression(int Value) : IExpression;