using DWES_01_Agenda.Models;
using DWES_01_Agenda.Repositories;
using DWES_01_Agenda.Validators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DWES_01_Agenda.Service;

public class ContactoService(IContactoRepository repo, ILogger<ContactoService> logger, IMemoryCache cache)
{
    private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); //Borra a los 5 min
    
    public void Create(Contacto contacto)
    {
        ValidarContacto(contacto, logger);
        repo.Create(contacto);
        logger.LogInformation("Contacto creado. ID: {Id}", contacto.Id);
    }

    public void Update(Contacto contacto, int id)
    {
        var exists = repo.GetById(id);
        if (exists == null)
        {
            logger.LogWarning("Id inválido al actualizar");
            throw new KeyNotFoundException("No existe contacto para esta id"); 
        }; 
        
        if (string.IsNullOrEmpty(contacto.Nombre)) contacto.Nombre = exists.Nombre;
        if (string.IsNullOrEmpty(contacto.Alias)) contacto.Alias = exists.Alias;
        if (string.IsNullOrEmpty(contacto.Telefono)) contacto.Telefono = exists.Telefono;
        if (string.IsNullOrEmpty(contacto.Correo)) contacto.Correo = exists.Correo;
        
        ValidarContacto(contacto, logger);
        
        repo.Update(contacto, id);
        
        cache.Remove($"contacto{id}");
        cache.Remove($"alias{exists.Alias}");
        
        logger.LogInformation("Contacto actualizado. ID: {Id}", contacto.Id);
    }

    public void Delete(int id)
    {
        var exists = repo.GetById(id);
        if (exists == null)
        {
            logger.LogWarning("Id inválido al borrar");
            throw new KeyNotFoundException("No existe contacto para esta id"); 
        }; 
        
        repo.Delete(id);
        
        cache.Remove($"contacto{id}");
        cache.Remove($"alias{exists.Alias}");
        
        logger.LogInformation("Contacto eliminado. ID: {Id}", id);
    }

    public Contacto GetById(int id)
    {
        string key = $"contacto{id}";
        
        if (cache.TryGetValue(key, out Contacto? contactoCache))
            return contactoCache!;
        
        var exists = repo.GetById(id);
        if (exists == null)
        {
            logger.LogWarning("Id inválido al buscar por id");
            throw new KeyNotFoundException("No existe contacto para esta id"); 
        }; 
        
        logger.LogInformation("Contacto encontrado. ID: {Id}", id);
        
        cache.Set(key, exists, _cacheOptions);
        return exists;
    }

    public Contacto GetByAlias(string alias)
    {
        if (!ValidadorContacto.ValidarAlias(alias))
        {
            logger.LogWarning("Alias inválido", alias);
            throw new ArgumentException("El alias no puede estar vacio.");
        }
        
        string key = $"alias{alias}";
        
        if (cache.TryGetValue(key, out Contacto? contactoCache))
            return contactoCache!;
        
        var exists = repo.GetByAlias(alias);
        if (exists == null)
        {
            logger.LogWarning("Alias inválido: {Alias}", alias);
            throw new KeyNotFoundException("No existe contacto para esta alias"); 
        }; 
        
        logger.LogInformation("Contacto encontrado. Alias {Alias}", alias);
        
        cache.Set(key, exists, _cacheOptions);
        return exists;
    }

    public List<Contacto> GetAll(int pagina)
    {
        logger.LogInformation("Devuelve lista de contactos");
        return repo.GetAll(pagina);
    }

    private static void ValidarContacto(Contacto contacto, ILogger logger)
    {
        if (!ValidadorContacto.ValidarNombre(contacto.Nombre))
        {
            logger.LogWarning("Nombre inválido", contacto.Nombre);
            throw new ArgumentException("El nombre no puede estar vacio.");
        }
        
        if (!ValidadorContacto.ValidarAlias(contacto.Alias))
        {
            logger.LogWarning("Alias inválido", contacto.Alias);
            throw new ArgumentException("El alias no puede estar vacio.");
        }

        if (!ValidadorContacto.ValidarTelefono(contacto.Telefono))
        {
            logger.LogWarning("Teléfono inválido: {Telefono}", contacto.Telefono);
            throw new ArgumentException("El formato del teléfono no es válido.");
        }

        if (!ValidadorContacto.ValidarCorreo(contacto.Correo))
        {
            logger.LogWarning("Correo electrónico inválido: {Correo}", contacto.Correo);
            throw new ArgumentException("El formato del correo electrónico no es válido.");
        }
    }
    
}