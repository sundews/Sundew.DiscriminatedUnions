//HintName: Sundew.DiscriminatedUnions.Tester.IntError.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.1.0.0")]
    public partial record IntError
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the OutOfRangeError case.
        /// </summary>
        /// <returns>The OutOfRangeError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.IntError.OutOfRangeError))]
        public static global::Sundew.DiscriminatedUnions.Tester.IntError _OutOfRangeError { get; }
            = new global::Sundew.DiscriminatedUnions.Tester.IntError.OutOfRangeError();
    }
}
