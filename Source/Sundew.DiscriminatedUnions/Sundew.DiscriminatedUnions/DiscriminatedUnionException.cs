using System;

namespace Sundew.DiscriminatedUnions
{
    public class DiscriminatedUnionException : Exception
    {
        public DiscriminatedUnionException(object discriminatedUnion)
            : base($"{discriminatedUnion} is not a valid discriminated union.")
        {
        }
    }
}