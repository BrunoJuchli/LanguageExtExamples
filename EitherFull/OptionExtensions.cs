namespace LanguageExt;

public static class OptionExtensions
{
    public static Option<A> BindNone<A>(
        this Option<A> option,
        Func<Option<A>> action)
    {
        return option.Match(
            some => some,
            () => action());
    }
}
