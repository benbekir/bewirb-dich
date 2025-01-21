using CreepyApi.Database.Models;

namespace CreepyApi.Helpers;

public static class BerechnungsartHelper
{
    public static Berechnungsart Parse(string berechnungsart)
    {
        if (Enum.TryParse(berechnungsart, out Berechnungsart value))
        {
            return value;
        }
        throw new ArgumentException($"{berechnungsart} ist kein gültiger Wert");
    }
}
