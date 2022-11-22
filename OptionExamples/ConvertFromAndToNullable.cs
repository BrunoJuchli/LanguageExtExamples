using System.Collections;
using LanguageExt;

namespace OptionExamples;

public class ConvertFromAndToNullable
{
    public Option<int> ConvertFromNullableToOption(int? someValue) 
        => Prelude.Optional(someValue);
    
    public OptionAsync<int> ConvertFromNullableToOptionAsync(int? somevalue)
        => Prelude.OptionalAsync(somevalue);

    public IEnumerable? ConvertFromOptionToNullable(Option<IEnumerable> option)
    {
        // DON'T -- language ext will will throw an exception!
        IEnumerable? result = option.IfNone(() => null!);

        // DO - but before you do it: better to return the Option instead?
        return (IEnumerable?)option.Case;
    }
    
    public async Task<IEnumerable?> ConvertFromOptionAsyncToNullable(OptionAsync<IEnumerable> option)
    {
        // DON'T -- language ext will will throw an exception!
        IEnumerable? result = await option.IfNone(() => null!);

        // DO - but before you do it: better to return the Option instead?
        return (IEnumerable?)await option.Case;
    }
}