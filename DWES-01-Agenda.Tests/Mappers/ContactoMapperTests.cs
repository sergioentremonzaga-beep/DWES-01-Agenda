using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Mappers;
using DWES_01_Agenda.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DWES_01_Agenda.Tests.Mappers;

[TestFixture]
public class ContactoMapperTests
{
    private ContactoMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new ContactoMapper();
    }

    [Test]
    public void ToModel_EntityValida_MapeaCorrectamenteAModel()
    {
        var entidad = new ContactoEntity
        {
            Id = 1,
            Nombre = "Juan Pérez",
            Alias = "juanp",
            Telefono = "+34611223344",
            Correo = "juan@test.com"
        };

        var resultado = _mapper.ToModel(entidad);

        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(entidad.Id);
        resultado.Nombre.Should().Be(entidad.Nombre);
        resultado.Alias.Should().Be(entidad.Alias);
        resultado.Telefono.Should().Be(entidad.Telefono);
        resultado.Correo.Should().Be(entidad.Correo);
    }

    [Test]
    public void ToEntity_ModelValido_MapeaCorrectamenteAEntity()
    {
        var modelo = new Contacto
        {
            Id = 1,
            Nombre = "Juan Pérez",
            Alias = "juanp",
            Telefono = "+34611223344",
            Correo = "juan@test.com"
        };

        var resultado = _mapper.ToEntity(modelo);

        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(modelo.Id);
        resultado.Nombre.Should().Be(modelo.Nombre);
        resultado.Alias.Should().Be(modelo.Alias);
        resultado.Telefono.Should().Be(modelo.Telefono);
        resultado.Correo.Should().Be(modelo.Correo);
    }
}