using CreepyApi.Controllers;
using CreepyApi.Database;
using CreepyApi.Database.Models;
using CreepyApi.DTOs;
using CreepyApi.Helpers;

namespace CreepyApi.Services;

public class DocumentService(CreepyApiDbContext dbContext, ILoggerFactory loggerFactory) : IDocumentService
{
    private readonly Logger<DocumentController> _logger = new(loggerFactory);

    public List<DokumentenlisteEintragDto> Get() => dbContext.Set<Document>().Select(DokumentenlisteEintragDto.FromEntity).ToList();

    public void Accept(Document document)
    {
        if (document.Typ == Dokumenttyp.Versicherungsschein)
        {
            _logger.LogError("Das Dokument mit der ID " + document.Uuid + " wurde bereits angenommen.");
            throw new ArgumentException("Das Dokument wurde bereits angenommen.");
        }
        document.Typ = Dokumenttyp.Versicherungsschein;
        dbContext.SaveChanges();
    }

    public void Export(Document document)
    {
        if (document.Typ != Dokumenttyp.Versicherungsschein)
        {
            _logger.LogError("Das Dokument mit der ID " + document.Uuid + " wurde bereits ausgestellt.");
            throw new ArgumentException("Nur ein Versicherungsschein kann ausgestellt werden.");
        }
        document.VersicherungsscheinAusgestellt = true;
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

        DocumentBeitragHelper.Calculate(dokument);
        dbContext.Add(dokument);
        dbContext.SaveChanges();
    }
}
