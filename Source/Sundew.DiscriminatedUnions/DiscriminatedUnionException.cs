using System;

namespace Sundew.DiscriminatedUnions
{
    public class DiscriminatedUnionException : Exception
    {
        public DiscriminatedUnionException(Type enumType)
            : base($"{enumType.Name} is not a valid discriminated union.")
        {
        }
    }
}