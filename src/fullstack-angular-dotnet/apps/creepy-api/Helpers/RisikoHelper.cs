using CreepyApi.Database.Models;

namespace CreepyApi.Helpers;

public static class RisikoHelper
{
    public static Risiko Parse(string risiko)
    {
        if (Enum.TryParse(risiko, out Risiko value))
        {
            return value;
        }
        throw new ArgumentException($"{risiko} ist kein gültiger Wert");
    }
}