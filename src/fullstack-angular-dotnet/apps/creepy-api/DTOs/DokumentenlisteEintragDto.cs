namespace CreepyApi.DTOs;

public class DokumentenlisteEintragDto
{
    public Guid Id { get; init; }

    public string Dokumenttyp { get; init; }

    public string Berechnungsart { get; init; }

    public string Risiko { get; init; }

    public string Zusatzschutz { get; init; }

    public bool WebshopVersichert { get; init; }

    public decimal Versicherungssumme { get; init; }

    public decimal Beitrag { get; init; }

    public bool KannAngenommenWerden { get; init; }

    public bool KannAusgestelltWerden { get; init; }
}
