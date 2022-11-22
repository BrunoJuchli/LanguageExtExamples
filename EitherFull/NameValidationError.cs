using LanguageExt;

namespace EitherFull;

public record NameValidationError
{
    private NameValidationError() { }

    public sealed record Length(
            string Name,
            RangeInclusive Range)
        : NameValidationError;

    public sealed record ForbiddenCharacter(
            string Name,
            Seq<(int index, char character)> Violations,
            Seq<char> ForbiddenCharacters)
        : NameValidationError;
}

