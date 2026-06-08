namespace DustInTheWind.CaveOfWonders.Infrastructure;

public static class EnumExtensions
{
    public static string ToDisplay<T>(this T value)
        where T : Enum
    {
        string valueAsString = Convert.ToInt32(value).ToString();

        bool valueIsDefined = Enum.IsDefined(typeof(T), value);

        return valueIsDefined
            ? $"{valueAsString} ({value})"
            : valueAsString;
    }
}