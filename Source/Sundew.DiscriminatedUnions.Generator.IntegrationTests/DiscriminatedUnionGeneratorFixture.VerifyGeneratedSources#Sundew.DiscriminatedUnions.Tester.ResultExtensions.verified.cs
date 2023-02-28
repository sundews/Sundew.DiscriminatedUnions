//HintName: Sundew.DiscriminatedUnions.Tester.ResultExtensions.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public static class ResultExtensions
    {
        public static ResultSegregation Segregate(this System.Collections.Generic.IEnumerable<Sundew.DiscriminatedUnions.Tester.Result> results)
        {
            var successes = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.Success>();
            var warnings = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.Warning>();
            var errors = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.Error>();
            var fatalErrors = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.FatalError>();

            foreach (var value in results)
            {
                switch (value)
                {
                    case Sundew.DiscriminatedUnions.Tester.Success success:
                        successes.Add(success);
                        break;
                    case Sundew.DiscriminatedUnions.Tester.Warning warning:
                        warnings.Add(warning);
                        break;
                    case Sundew.DiscriminatedUnions.Tester.Error error:
                        errors.Add(error);
                        break;
                    case Sundew.DiscriminatedUnions.Tester.FatalError fatalError:
                        fatalErrors.Add(fatalError);
                        break;
                }
            }

            return new ResultSegregation(successes, warnings, errors, fatalErrors);
        }
    }
}