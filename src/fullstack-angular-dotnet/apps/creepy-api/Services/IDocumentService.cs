using CreepyApi.DTOs;

namespace CreepyApi.Services;

public interface IDocumentService
{
    List<DokumentenlisteEintragDto> Get();
    DokumentenlisteEintragDto GetById(Guid id);
    void Create(ErzeugeNeuesAngebotDto dto);
    void Accept(Guid id);
    void Export(Guid id);
}
