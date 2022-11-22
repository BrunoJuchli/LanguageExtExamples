using LanguageExt;

namespace OptionExamples;

public class OptionAsyncExamples
{
    public async Task<int> Awaiting_DONT(OptionAsync<int> option)
    {
        // DON'T
        // this will throw an exception in case of Option<T>.None !
        int result = await option;
        return result;
    }

    public Task<int> Returning_Fallback_value(OptionAsync<int> option)
    {
        return option
            .IfNone(int.MinValue);
    }

    public OptionAsync<string> Conversion(OptionAsync<int> option)
    {
        return option
            .Map(val => val.ToString());
    }

    public OptionAsync<string> ChainingWithNextOption(OptionAsync<int> option)
    {
        return option
            .Bind(
                val => 
                    val > 0
                        ? val.ToString()
                        : OptionAsync<string>.None);
    }

    public EitherAsync<string, int> NoneMeansError(OptionAsync<int> option)
    {
        return option
            .ToEither("sorry but there's no value");
    }
}