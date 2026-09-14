using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Models;

namespace DWES_01_Agenda.Repositories;

public interface IContactoRepository
{
    public void Create (Contacto contacto);
    public void Delete (int id);
    public void Update (Contacto contacto, int id);
    public Contacto? GetById (int id);
    public Contacto? GetByAlias (string alias);
    public List<Contacto> GetAll(int pagina);
}