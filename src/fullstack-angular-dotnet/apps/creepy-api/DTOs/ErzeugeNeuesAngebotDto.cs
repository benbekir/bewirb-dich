namespace CreepyApi.DTOs;

public class ErzeugeNeuesAngebotDto
{
    public bool HatWebshop { get; init; }

    public string Berechnungsart { get; init; }

    public string Risiko { get; init; }

    public bool WillZusatzschutz { get; init; }

    public string ZusatzschutzAufschlag { get; set; }

    public decimal Versicherungssumme { get; init; }
}
