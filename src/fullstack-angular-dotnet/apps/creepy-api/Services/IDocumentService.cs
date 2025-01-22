using CreepyApi.Database.Models;
using CreepyApi.DTOs;

namespace CreepyApi.Services;

public interface IDocumentService
{
    List<DokumentenlisteEintragDto> Get();
    void Create(ErzeugeNeuesAngebotDto dto);
    void Accept(Document document);
    void Export(Document document);
}
