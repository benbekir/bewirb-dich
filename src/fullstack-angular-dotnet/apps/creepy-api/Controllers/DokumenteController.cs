using CreepyApi.Database;
using CreepyApi.Database.Models;
using CreepyApi.DTOs;
using CreepyApi.Helpers;
using CreepyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepyApi.Controllers;

[Controller]
public class DokumenteController : Controller
{
    private readonly Logger<DokumenteController> _logger;
    private readonly CreepyApiDbContext _dbContext;

    public DokumenteController(ILoggerFactory loggerFactory, CreepyApiDbContext dbContext)
    {
        _logger = new Logger<DokumenteController>(loggerFactory);
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("/Dokumente")]
    public ActionResult<IEnumerable<DokumentenlisteEintragDto>> DokumenteAbrufen()
    {
        var x = _dbContext.Set<Database.Models.Dokument>().ToList();
		DokumenteService service = DokumenteService.Instance;
        IEnumerable<DokumentenlisteEintragDto> result = service.List().Select(MapToDto);
        return Ok(result);
    }

    [HttpGet]
    [Route("/Dokumente/{id}")]
    public DokumentenlisteEintragDto DokumentFinden([FromRoute] Guid id)
    {
        Domain.Dokument? dokument = DokumenteService.Instance.Find(id);

        if (dokument == null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentNullException("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
        }
        else
        {
            return MapToDto(dokument);
        }
    }

    [HttpPost]
    [Route("/Dokumente")]
    public ActionResult DokumenteErstellen([FromBody] ErzeugeNeuesAngebotDto dto)
    {
        DokumenteService service = DokumenteService.Instance;

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

        // TODO: auslagern in dokument ctor
        Domain.Dokument dokument = Domain.Dokument.Create();
        dokument.InkludiereZusatzschutz = dto.WillZusatzschutz;
        dokument.HatWebshop = dto.HatWebshop;
        dokument.VersicherungsscheinAusgestellt = false;
        dokument.Risiko = RisikoHelper.Parse(dto.Risiko);
        dokument.Versicherungssumme = dto.Versicherungssumme;
        dokument.ZusatzschutzAufschlag = float.Parse(dto.ZusatzschutzAufschlag.Replace("%", ""));
        dokument.Typ = Dokumenttyp.Angebot;
        dokument.Berechnungsart = BerechnungsartHelper.Parse(dto.Berechnungsart);

        Kalkuliere(dokument);

        service.Add(dokument);
        service.Save();

        return Ok();
    }

    [HttpPost]
    [Route("/Dokumente/{id}/annehmen")]
    public ActionResult DokumentAnnehmen([FromRoute] Guid id)
    {
        DokumenteService service = DokumenteService.Instance;

        Domain.Dokument? dokument = service.Find(id);

        if (dokument == null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentException("Das Dokument mit der Id " + id + " konnte nicht gefunden werden.");
        }

        dokument.Typ = Dokumenttyp.Versicherungsschein;
        service.Save();

        return Ok();
    }

    [HttpPost]
    [Route("/Dokumente/{id}/ausstellen")]
    public ActionResult DokumentAusstellen([FromRoute] Guid id)
    {
        DokumenteService service = DokumenteService.Instance;

        Domain.Dokument? dokument = service.Find(id);

        if (dokument == null)
        {
            _logger.LogError("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
            throw new ArgumentException("Das Dokument mit der Id " + id + " konnte nicht gefunden werden.");
        }

        if (dokument.Typ != Dokumenttyp.Versicherungsschein)
        {
            throw new ArgumentException("Nur ein Versicherungsschein kann ausgestellt werden.");
        }
        dokument.VersicherungsscheinAusgestellt = true;
        service.Save();

        return Ok();
    }


    // TODO: AB HIER ALLES AUSLAGERN


    private static void Kalkuliere(Domain.Dokument dokument)
    {
        //Versicherungsnehmer, die nach Haushaltssumme versichert werden (primär Vereine) stellen immer ein mittleres Risiko da
        if (dokument.Berechnungsart == Berechnungsart.Haushaltssumme)
        {
            dokument.Risiko = Risiko.Mittel;
        }

        //Versicherungsnehmer, die nach Anzahl Mitarbeiter abgerechnet werden und mehr als 5 Mitarbeiter haben, können kein Lösegeld absichern
        if (dokument.Berechnungsart == Berechnungsart.AnzahlMitarbeiter)
            if (dokument.Berechnungbasis > 5)
            {
                dokument.InkludiereZusatzschutz = false;
                dokument.ZusatzschutzAufschlag = 0;
            }

        //Versicherungsnehmer, die nach Umsatz abgerechnet werden, mehr als 100.000€ ausweisen und Lösegeld versichern, haben immer mittleres Risiko
        if (dokument.Berechnungsart == Berechnungsart.Umsatz)
            if (dokument.Berechnungbasis > 100000m && dokument.InkludiereZusatzschutz)
            {
                dokument.Risiko = Risiko.Mittel;
            }

        decimal beitrag;
        switch (dokument.Berechnungsart)
        {
            case Berechnungsart.Umsatz:
                decimal faktorUmsatz = (decimal)Math.Pow((double)dokument.Versicherungssumme, 0.25d);
                beitrag = 1.1m + faktorUmsatz * (dokument.Berechnungbasis / 100000);
                if (dokument.HatWebshop) //Webshop gibt es nur bei Unternehmen, die nach Umsatz abgerechnet werden
                    beitrag *= 2;
                break;
            case Berechnungsart.Haushaltssumme:
                decimal faktorHaushaltssumme = (decimal)Math.Log10((double)dokument.Versicherungssumme);
                beitrag = 1.0m + faktorHaushaltssumme * dokument.Berechnungbasis + 100m;
                break;
            case Berechnungsart.AnzahlMitarbeiter:
                decimal faktorMitarbeiter = dokument.Versicherungssumme / 1000;

                if (dokument.Berechnungbasis < 4)
                    beitrag = faktorMitarbeiter + dokument.Berechnungbasis * 250m;
                else
                    beitrag = faktorMitarbeiter + dokument.Berechnungbasis * 200m;

                break;
            default:
                throw new Exception();
        }

        if (dokument.InkludiereZusatzschutz)
            beitrag *= 1.0m + (decimal)dokument.ZusatzschutzAufschlag / 100.0m;

        if (dokument.Risiko == Risiko.Mittel)
        {
            if (dokument.Berechnungsart is Berechnungsart.Haushaltssumme or Berechnungsart.Umsatz)
                beitrag *= 1.2m;
            else
                beitrag *= 1.3m;
        }

        dokument.Berechnungbasis = Math.Round(dokument.Berechnungbasis, 2);
        dokument.Beitrag = Math.Round(beitrag, 2);
    }

    private DokumentenlisteEintragDto MapToDto(Domain.Dokument dokument)
    {
        return new DokumentenlisteEintragDto()
        {
            Id = dokument.Id,
            Beitrag = dokument.Beitrag,
            Berechnungsart = dokument.Berechnungsart.ToString(),
            Dokumenttyp = dokument.Typ.ToString(),
            Risiko = Enum.GetName(typeof(Risiko), dokument.Risiko)!,
            Versicherungssumme = dokument.Versicherungssumme,
            Zusatzschutz = $"{dokument.ZusatzschutzAufschlag}%",
            WebshopVersichert = dokument.HatWebshop,
            KannAngenommenWerden = !dokument.VersicherungsscheinAusgestellt && dokument.Typ == Dokumenttyp.Angebot,
            KannAusgestelltWerden = !dokument.VersicherungsscheinAusgestellt && dokument.Typ == Dokumenttyp.Versicherungsschein
        };
    }
}
