namespace Sundew.DiscriminatedUnions.Tester
{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {
        private protected Expression()
        { }
    }

    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record ArithmeticExpression : Expression
    {
        private protected ArithmeticExpression()
        { }

        public abstract Expression Lhs { get; init; }

        public abstract Expression Rhs { get; init;}
    }

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

    public sealed record ValueExpression(int Value) : Expression;
}