using CreepyApi.Controllers;
using CreepyApi.Database;
using CreepyApi.Database.Models;
using CreepyApi.DTOs;
using CreepyApi.Helpers;

namespace CreepyApi.Services;

public class DocumentService(CreepyApiDbContext dbContext, ILoggerFactory loggerFactory) : IDocumentService
{
    private readonly Logger<DocumentController> _logger = new Logger<DocumentController>(loggerFactory);

    public List<DokumentenlisteEintragDto> Get() => dbContext.Set<Document>().Select(DokumentenlisteEintragDto.FromEntity).ToList();

    public DokumentenlisteEintragDto GetById(Guid id)
    {
        Document? dokument = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid.Equals(id));
        if (dokument is null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentNullException("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
        }
        return DokumentenlisteEintragDto.FromEntity(dokument);
    }

    public void Accept(Guid id)
    {
        Document? dokument = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid == id);
        if (dokument is null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentException("Das Dokument mit der Id " + id + " konnte nicht gefunden werden.");
        }
        dokument.Typ = Dokumenttyp.Versicherungsschein;
        dbContext.SaveChanges();
    }

    public void Export(Guid id)
    {
        Document? dokument = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid == id);
        if (dokument is null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentException("Das Dokument mit der Id " + id + " konnte nicht gefunden werden.");
        }
        if (dokument.Typ != Dokumenttyp.Versicherungsschein)
        {
            throw new ArgumentException("Nur ein Versicherungsschein kann ausgestellt werden.");
        }
        dokument.VersicherungsscheinAusgestellt = true;
        dbContext.SaveChanges();
    }

    public void Create(ErzeugeNeuesAngebotDto dto)
    {
        if (dto.Versicherungssumme < 0)
        {
            throw new ArgumentOutOfRangeException("Die Versicherungssumme darf nicht negativ sein.");
        }

        if (string.IsNullOrWhiteSpace(dto.ZusatzschutzAufschlag))
        {
            dto.ZusatzschutzAufschlag = "0%";
        }

        if (dto.ZusatzschutzAufschlag.StartsWith("-"))
        {
            throw new ArgumentOutOfRangeException("Der Zusatzschutzaufschlag darf nicht negativ sein.");
        }

        Document dokument = new()
        {
            InkludiereZusatzschutz = dto.WillZusatzschutz,
            HatWebshop = dto.HatWebshop,
            VersicherungsscheinAusgestellt = false,
            Risiko = RisikoHelper.Parse(dto.Risiko),
            Versicherungssumme = dto.Versicherungssumme,
            ZusatzschutzAufschlag = float.Parse(dto.ZusatzschutzAufschlag.Replace("%", "")),
            Typ = Dokumenttyp.Angebot,
            Berechnungsart = BerechnungsartHelper.Parse(dto.Berechnungsart)
        };

        Kalkuliere(dokument);
        dbContext.Add(dokument);
        dbContext.SaveChanges();
    }

    private static void Kalkuliere(Document document)
    {
        //Versicherungsnehmer, die nach Haushaltssumme versichert werden (primär Vereine) stellen immer ein mittleres Risiko da
        if (document.Berechnungsart == Berechnungsart.Haushaltssumme)
        {
            document.Risiko = Risiko.Mittel;
        }

        //Versicherungsnehmer, die nach Anzahl Mitarbeiter abgerechnet werden und mehr als 5 Mitarbeiter haben, können kein Lösegeld absichern
        if (document.Berechnungsart == Berechnungsart.AnzahlMitarbeiter)
            if (document.Berechnungbasis > 5)
            {
                document.InkludiereZusatzschutz = false;
                document.ZusatzschutzAufschlag = 0;
            }

        //Versicherungsnehmer, die nach Umsatz abgerechnet werden, mehr als 100.000€ ausweisen und Lösegeld versichern, haben immer mittleres Risiko
        if (document.Berechnungsart == Berechnungsart.Umsatz)
            if (document.Berechnungbasis > 100000m && document.InkludiereZusatzschutz)
            {
                document.Risiko = Risiko.Mittel;
            }

        decimal beitrag;
        switch (document.Berechnungsart)
        {
            case Berechnungsart.Umsatz:
                decimal faktorUmsatz = (decimal)Math.Pow((double)document.Versicherungssumme, 0.25d);
                beitrag = 1.1m + faktorUmsatz * (document.Berechnungbasis / 100000);
                if (document.HatWebshop) //Webshop gibt es nur bei Unternehmen, die nach Umsatz abgerechnet werden
                    beitrag *= 2;
                break;
            case Berechnungsart.Haushaltssumme:
                decimal faktorHaushaltssumme = (decimal)Math.Log10((double)document.Versicherungssumme);
                beitrag = 1.0m + faktorHaushaltssumme * document.Berechnungbasis + 100m;
                break;
            case Berechnungsart.AnzahlMitarbeiter:
                decimal faktorMitarbeiter = document.Versicherungssumme / 1000;

                if (document.Berechnungbasis < 4)
                    beitrag = faktorMitarbeiter + document.Berechnungbasis * 250m;
                else
                    beitrag = faktorMitarbeiter + document.Berechnungbasis * 200m;

                break;
            default:
                throw new Exception();
        }

        if (document.InkludiereZusatzschutz)
            beitrag *= 1.0m + (decimal)document.ZusatzschutzAufschlag / 100.0m;

        if (document.Risiko == Risiko.Mittel)
        {
            if (document.Berechnungsart is Berechnungsart.Haushaltssumme or Berechnungsart.Umsatz)
                beitrag *= 1.2m;
            else
                beitrag *= 1.3m;
        }

        document.Berechnungbasis = Math.Round(document.Berechnungbasis, 2);
        document.Beitrag = Math.Round(beitrag, 2);
    }
}
