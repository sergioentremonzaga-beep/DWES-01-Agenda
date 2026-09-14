using DWES_01_Agenda.Models;
using DWES_01_Agenda.Service;
using Microsoft.AspNetCore.Mvc;

namespace DWES_01_Agenda.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactosController(ContactoService service) : ControllerBase
{
    [HttpGet]
    public List<Contacto> GetAll(int pagina = 1) => service.GetAll(pagina);

    [HttpGet("{id:int}")]
    public Contacto GetById(int id) => service.GetById(id);

    [HttpGet("alias/{alias}")]
    public Contacto GetByAlias(string alias) => service.GetByAlias(alias);

    [HttpPost]
    public void Create([FromBody] Contacto contacto) => service.Create(contacto); //Frombody lee datos desde la peticion JSON

    [HttpPut("{id:int}")]
    public void Update(int id, [FromBody] Contacto contacto) => service.Update(contacto, id);

    [HttpDelete("{id:int}")]
    public void Delete(int id) => service.Delete(id);
}   