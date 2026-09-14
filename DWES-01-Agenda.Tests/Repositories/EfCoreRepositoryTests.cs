using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Mappers;
using DWES_01_Agenda.Models;
using DWES_01_Agenda.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace DWES_01_Agenda.Tests.Repositories;

[TestFixture]
public class EfCoreContactoRepositoryTests
{
    private SqliteConnection _conexion;
    private AppDbContext _context;
    private Mock<IMapper<Contacto, ContactoEntity>> _mapperMock;
    private EfCoreContactoRepository _repo;

    [SetUp]
    public void Setup()
    {
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();

        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexion)
            .Options;

        _context = new AppDbContext(opciones);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _mapperMock = new Mock<IMapper<Contacto, ContactoEntity>>();
        
        _mapperMock.Setup(m => m.ToEntity(It.IsAny<Contacto>()))
            .Returns<Contacto>(c => new ContactoEntity
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Alias = c.Alias,
                Telefono = c.Telefono,
                Correo = c.Correo
            });

        _mapperMock.Setup(m => m.ToModel(It.IsAny<ContactoEntity>()))
            .Returns<ContactoEntity>(e => new Contacto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Alias = e.Alias,
                Telefono = e.Telefono,
                Correo = e.Correo
            });

        _repo = new EfCoreContactoRepository(_context, _mapperMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _conexion.Close();
        _conexion.Dispose();
    }

    [Test]
    public void CreateValido()
    {
        var contacto = CrearContacto("juanp");

        _repo.Create(contacto);
        var resultado = _repo.GetById(contacto.Id);

        resultado.Should().NotBeNull();
        resultado!.Alias.Should().Be("juanp");
    }

    [Test]
    public void GetByIdValido()
    {
        var contacto = CrearContacto("chema");
        _repo.Create(contacto);

        var resultado = _repo.GetById(contacto.Id);

        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("chema");
    }

    [Test]
    public void GetByIdNoExistenteDevuelveNull()
    {
        var resultado = _repo.GetById(999);

        resultado.Should().BeNull();
    }

    [Test]
    public void GetByAliasValido()
    {
        var contacto = CrearContacto("carlitos");
        _repo.Create(contacto);

        var resultado = _repo.GetByAlias("carlitos");

        resultado.Should().NotBeNull();
        resultado!.Alias.Should().Be("carlitos");
    }

    [Test]
    public void GetByAliasNoExistenteDevuelveNull()
    {
        var resultado = _repo.GetByAlias("desconocido");

        resultado.Should().BeNull();
    }

    [Test]
    public void UpdateValido()
    {
        var contacto = CrearContacto("pedro");
        _repo.Create(contacto);

        contacto.Nombre = "Pedro Editado";
        contacto.Correo = "nuevo@test.com";
        _repo.Update(contacto, contacto.Id);

        var entidadEnBd = _context.Agenda.Find(contacto.Id);
        entidadEnBd.Should().NotBeNull();
        entidadEnBd!.Nombre.Should().Be("Pedro Editado");
        entidadEnBd.Correo.Should().Be("nuevo@test.com");
        entidadEnBd.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public void DeleteValido()
    {
        var contacto = CrearContacto("aborrar");
        _repo.Create(contacto);

        _repo.Delete(contacto.Id);

        var resultado = _repo.GetById(contacto.Id);
        resultado.Should().BeNull();
    }

    [TestCase(1, 20)]
    [TestCase(2, 5)]
    [TestCase(3, 0)]
    public void GetAllPaginado(int pagina, int esperado)
    {
        for (int i = 1; i <= 25; i++)
        {
            _repo.Create(new Contacto
            {
                Id = i,
                Nombre = $"Contacto {i:D2}",
                Alias = $"alias{i}",
                Telefono = "123456789",
                Correo = $"contacto{i}@test.com"
            });
        }

        var resultado = _repo.GetAll(pagina);

        resultado.Count.Should().Be(esperado);
    }

    private Contacto CrearContacto(string alias)
    {
        return new Contacto
        {
            Id = 1,
            Nombre = "Laura",
            Alias = alias,
            Telefono = "600112233",
            Correo = $"{alias}@test.com"
        };
    }
}