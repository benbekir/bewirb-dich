using CreepyApi.Database.Models;

namespace CreepyApi.DTOs;

public class DokumentenlisteEintragDto
{
    public Guid Id { get; init; }

    // TODO: rename to "typ"
    public string Dokumenttyp { get; init; }

    public string Berechnungsart { get; init; }

    public string Risiko { get; init; }

    public string Zusatzschutz { get; init; }

    public bool WebshopVersichert { get; init; }

    public decimal Versicherungssumme { get; init; }

    public decimal Beitrag { get; init; }

    public bool KannAngenommenWerden { get; init; }

    public bool KannAusgestelltWerden { get; init; }

    public static DokumentenlisteEintragDto FromEntity(Document dokument)
    {
        return new DokumentenlisteEintragDto()
        {
            Id = dokument.Uuid,
            Beitrag = dokument.Beitrag,
            Berechnungsart = dokument.Berechnungsart.ToString(),
            Dokumenttyp = dokument.Typ.ToString(),
            Risiko = Enum.GetName(typeof(Risiko), dokument.Risiko)!,
            Versicherungssumme = dokument.Versicherungssumme,
            Zusatzschutz = $"{dokument.ZusatzschutzAufschlag}%",
            WebshopVersichert = dokument.HatWebshop,
            KannAngenommenWerden = !dokument.VersicherungsscheinAusgestellt && dokument.Typ == Database.Models.Dokumenttyp.Angebot,
            KannAusgestelltWerden = !dokument.VersicherungsscheinAusgestellt && dokument.Typ == Database.Models.Dokumenttyp.Versicherungsschein
        };
    }
}
