namespace Sundew.DiscriminatedUnions
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class DiscriminatedUnion : Attribute
    {
    }
}