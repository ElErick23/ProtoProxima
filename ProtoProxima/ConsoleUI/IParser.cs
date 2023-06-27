using Humanizer;
using Humanizer.Localisation;

namespace ProtoProxima.ConsoleUI;

public interface IParser
{
    public static string Humanize(TimeSpan timeSpan)
    {
        return timeSpan.Humanize(7, minUnit: TimeUnit.Minute, collectionSeparator: " ")
            .Replace(" year", "y")
            .Replace(" month", "M")
            .Replace(" week", "w")
            .Replace(" day", "d")
            .Replace(" hour", "hr")
            .Replace(" minute", "min")
            .Replace(" second", "sec");
    }
}