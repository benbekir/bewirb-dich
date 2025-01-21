using CreepyApi.DTOs;
using CreepyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepyApi.Controllers;

[Controller]
public class DocumentController(ILoggerFactory loggerFactory, IDocumentService dokumenteService) : Controller
{
    private readonly Logger<DocumentController> _logger = new(loggerFactory);

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
        DokumentenlisteEintragDto result = dokumenteService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    [Route("/Dokumente")]
    public ActionResult Create([FromBody] ErzeugeNeuesAngebotDto dto)
    {
        dokumenteService.Create(dto);
        return Ok();
    }

    // TODO: use get
    [HttpPost]
    [Route("/Dokumente/{id}/annehmen")]
    public ActionResult Accept([FromRoute] Guid id)
    {
        dokumenteService.Accept(id);
        return Ok();
    }

    // TODO: use get
    [HttpPost]
    [Route("/Dokumente/{id}/ausstellen")]
    public ActionResult Export([FromRoute] Guid id)
    {
        dokumenteService.Export(id);
        return Ok();
    }
}
