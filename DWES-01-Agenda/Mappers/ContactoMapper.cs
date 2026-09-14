using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Models;

namespace DWES_01_Agenda.Mappers;

public class ContactoMapper : IMapper<Contacto, ContactoEntity>
{
    public Contacto ToModel(ContactoEntity entity)
    {
        return new Contacto
        {
            Id = entity.Id, Nombre = entity.Nombre, Telefono = entity.Telefono, Correo = entity.Correo,
            Alias = entity.Alias
        };
    }

    public ContactoEntity ToEntity(Contacto model)
    {
        return new ContactoEntity
        {
            Id = model.Id, Nombre = model.Nombre, Telefono = model.Telefono, Correo = model.Correo,
            Alias = model.Alias
        };
    }
}