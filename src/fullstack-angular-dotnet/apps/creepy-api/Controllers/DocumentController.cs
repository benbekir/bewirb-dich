using CreepyApi.Database;
using CreepyApi.Database.Models;
using CreepyApi.DTOs;
using CreepyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepyApi.Controllers;

[Controller]
public class DocumentController(CreepyApiDbContext dbContext, IDocumentService dokumenteService) : Controller
{
    [HttpGet]
    [Route("/Dokumente")]
    public ActionResult<IEnumerable<DokumentenlisteEintragDto>> Get()
    {
        IEnumerable<DokumentenlisteEintragDto> result = dokumenteService.Get();
        return Ok(result);
    }

    [HttpGet]
    [Route("/Dokumente/{id}")]
    public ActionResult<DokumentenlisteEintragDto> GetById([FromRoute] Guid id)
    {
        Document? document = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid.Equals(id));
        if (document is null)
        {
            return NotFound("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
        }

        DokumentenlisteEintragDto result = DokumentenlisteEintragDto.FromEntity(document);
        return Ok(result);
    }

    [HttpPost]
    [Route("/Dokumente")]
    public ActionResult Create([FromBody] ErzeugeNeuesAngebotDto dto)
    {
        dokumenteService.Create(dto);
        return Ok();
    }

    [HttpPut]
    [Route("/Dokumente/{id}/annehmen")]
    public ActionResult Accept([FromRoute] Guid id)
    {
        Document? document = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid.Equals(id));
        if (document is null)
        {
            return NotFound("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
        }

        dokumenteService.Accept(document);
        return Ok();
    }

    [HttpPut]
    [Route("/Dokumente/{id}/ausstellen")]
    public ActionResult Export([FromRoute] Guid id)
    {
        Document? document = dbContext.Set<Document>().SingleOrDefault(x => x.Uuid.Equals(id));
        if (document is null)
        {
            return NotFound("Das Dokument mit der ID " + id + " konnte nicht gefunden werden.");
        }

        dokumenteService.Export(document);
        return Ok();
    }
}
