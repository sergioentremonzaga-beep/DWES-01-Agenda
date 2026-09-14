using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Mappers;
using DWES_01_Agenda.Models;

namespace DWES_01_Agenda.Repositories;

public class EfCoreContactoRepository(AppDbContext context, IMapper<Contacto, ContactoEntity> mapper) : IContactoRepository
{
    private const int tamañoPagina = 20;

    public void Create(Contacto contacto)
    {
        var entity = mapper.ToEntity(contacto);
        
        context.Agenda.Add(entity);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var exists = context.Agenda.Find(id);
        if (exists != null)
        {
            context.Agenda.Remove(exists);
            context.SaveChanges();
        }
    }

    public void Update(Contacto contacto, int id)
    {
        var exists = context.Agenda.Find(id);
        if (exists != null)
        {
            exists.UpdatedAt =  DateTime.Now;
            exists.Nombre = contacto.Nombre;
            exists.Alias = contacto.Alias;
            exists.Telefono = contacto.Telefono;
            exists.Correo = contacto.Correo;
            
            context.SaveChanges();
        }
    }

    public Contacto? GetById(int id)
    {
        var exists = context.Agenda.Find(id);
        if (exists != null)
        {
            return mapper.ToModel(exists);
        }
        return null;
    }

    public Contacto? GetByAlias(string alias)
    {
        var exists = context.Agenda.Where(a => a.Alias == alias).FirstOrDefault();
        if (exists != null)
        {
            return mapper.ToModel(exists);
        }
        return null;
    }

    public List<Contacto> GetAll(int pagina)
    {
        return context.Agenda.OrderBy(c => c.Nombre).Skip((pagina-1) * tamañoPagina).Take(tamañoPagina).Select(e => mapper.ToModel(e)).ToList();
    }
}