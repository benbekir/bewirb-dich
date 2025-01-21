using CreepyApi.Domain;

namespace CreepyApi.Services;

// TODO: dependency inject in controller once EF is used
public interface IDokumenteService
{
    Dokument? Find(Guid id);
    List<Dokument> GetAll();
    void Create(Dokument dokument);
}
