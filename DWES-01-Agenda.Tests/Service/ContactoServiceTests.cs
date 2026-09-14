using DWES_01_Agenda.Models;
using DWES_01_Agenda.Repositories;
using DWES_01_Agenda.Service;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DWES_01_Agenda.Tests.Services;

[TestFixture]
public class ContactoServiceTests
{
    private Mock<IContactoRepository> _repoMock;
    private Mock<ILogger<ContactoService>> _loggerMock;
    private IMemoryCache _cache;
    private ContactoService _service;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IContactoRepository>();
        _loggerMock = new Mock<ILogger<ContactoService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new ContactoService(_repoMock.Object, _loggerMock.Object, _cache);
    }

    [TearDown]
    public void TearDown()
    {
        _cache.Dispose();
    }

    [Test]
    public void Create_ContactoValido_LlamaAlRepositorio()
    {
        var contacto = CrearContactoValido();

        _service.Create(contacto);

        _repoMock.Verify(r => r.Create(contacto), Times.Once);
    }

    [TestCase("", "juanp", "+34611223344", "juan@test.com", "nombre")]
    [TestCase("Juan", "", "+34611223344", "juan@test.com", "alias")]
    [TestCase("Juan", "juanp", "invalido", "juan@test.com", "teléfono")]
    [TestCase("Juan", "juanp", "+34611223344", "email_invalido", "correo")]
    public void Create_ContactoInvalido_LanzaArgumentException(string nombre, string alias, string telefono, string correo, string campoError)
    {
        var contacto = new Contacto { Nombre = nombre, Alias = alias, Telefono = telefono, Correo = correo };

        Action act = () => _service.Create(contacto);

        act.Should().Throw<ArgumentException>();
        _repoMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void GetById_CuandoEstaEnCache_DevuelveDeCacheSinLlamarAlRepo()
    {
        int id = 1;
        var contactoCache = CrearContactoValido(id);
        _cache.Set($"contacto{id}", contactoCache);

        var resultado = _service.GetById(id);

        resultado.Should().BeEquivalentTo(contactoCache);
        _repoMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void GetById_CuandoNoEstaEnCachePeroExisteEnRepo_GuardaEnCacheYDevuelve()
    {
        int id = 1;
        var contactoBD = CrearContactoValido(id);
        _repoMock.Setup(r => r.GetById(id)).Returns(contactoBD);

        var resultado = _service.GetById(id);

        resultado.Should().BeEquivalentTo(contactoBD);
        _repoMock.Verify(r => r.GetById(id), Times.Once);

        _cache.TryGetValue($"contacto{id}", out Contacto? cacheado).Should().BeTrue();
        cacheado.Should().BeEquivalentTo(contactoBD);
    }

    [Test]
    public void GetById_CuandoNoExisteEnRepo_LanzaKeyNotFoundException()
    {
        int id = 99;
        _repoMock.Setup(r => r.GetById(id)).Returns((Contacto?)null);

        Action act = () => _service.GetById(id);

        act.Should().Throw<KeyNotFoundException>()
           .WithMessage("No existe contacto para esta id");
    }

    [Test]
    public void GetByAlias_AliasVacio_LanzaArgumentException()
    {
        Action act = () => _service.GetByAlias("");

        act.Should().Throw<ArgumentException>()
           .WithMessage("El alias no puede estar vacio.");
    }

    [Test]
    public void GetByAlias_EnCache_DevuelveDeCache()
    {
        string alias = "juanp";
        var contactoCache = CrearContactoValido(1);
        _cache.Set($"alias{alias}", contactoCache);

        var resultado = _service.GetByAlias(alias);

        resultado.Should().BeEquivalentTo(contactoCache);
        _repoMock.Verify(r => r.GetByAlias(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetByAlias_ExisteEnRepo_GuardaEnCacheYDevuelve()
    {
        string alias = "juanp";
        var contactoBD = CrearContactoValido(1);
        _repoMock.Setup(r => r.GetByAlias(alias)).Returns(contactoBD);

        var resultado = _service.GetByAlias(alias);

        resultado.Should().BeEquivalentTo(contactoBD);
        _cache.TryGetValue($"alias{alias}", out Contacto? cacheado).Should().BeTrue();
    }

    [Test]
    public void GetByAlias_NoExisteEnRepo_LanzaKeyNotFoundException()
    {
        string alias = "desconocido";
        _repoMock.Setup(r => r.GetByAlias(alias)).Returns((Contacto?)null);

        Action act = () => _service.GetByAlias(alias);

        act.Should().Throw<KeyNotFoundException>()
           .WithMessage("No existe contacto para esta alias");
    }

    [Test]
    public void Update_ContactoExistente_ActualizaYLimpiaCache()
    {
        int id = 1;
        var contactoExistente = CrearContactoValido(id);
        var datosNuevos = new Contacto { Id = id, Nombre = "Juan Modificado", Alias = "juanp", Telefono = "+34611223344", Correo = "juan@test.com" };

        _repoMock.Setup(r => r.GetById(id)).Returns(contactoExistente);
        
        _cache.Set($"contacto{id}", contactoExistente);
        _cache.Set($"alias{contactoExistente.Alias}", contactoExistente);

        _service.Update(datosNuevos, id);

        _repoMock.Verify(r => r.Update(datosNuevos, id), Times.Once);
        _cache.TryGetValue($"contacto{id}", out _).Should().BeFalse();
        _cache.TryGetValue($"alias{contactoExistente.Alias}", out _).Should().BeFalse();
    }

    [Test]
    public void Update_IdNoExistente_LanzaKeyNotFoundException()
    {
        int id = 99;
        _repoMock.Setup(r => r.GetById(id)).Returns((Contacto?)null);

        Action act = () => _service.Update(CrearContactoValido(id), id);

        act.Should().Throw<KeyNotFoundException>();
        _repoMock.Verify(r => r.Update(It.IsAny<Contacto>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void Delete_ContactoExistente_EliminaYLimpiaCache()
    {
        int id = 1;
        var contactoExistente = CrearContactoValido(id);
        _repoMock.Setup(r => r.GetById(id)).Returns(contactoExistente);

        _cache.Set($"contacto{id}", contactoExistente);
        _cache.Set($"alias{contactoExistente.Alias}", contactoExistente);

        _service.Delete(id);

        _repoMock.Verify(r => r.Delete(id), Times.Once);
        _cache.TryGetValue($"contacto{id}", out _).Should().BeFalse();
        _cache.TryGetValue($"alias{contactoExistente.Alias}", out _).Should().BeFalse();
    }

    [Test]
    public void Delete_IdNoExistente_LanzaKeyNotFoundException()
    {
        int id = 99;
        _repoMock.Setup(r => r.GetById(id)).Returns((Contacto?)null);

        Action act = () => _service.Delete(id);

        act.Should().Throw<KeyNotFoundException>();
        _repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void GetAll_LlamaAlRepositorioPaginado()
    {
        int pagina = 1;
        var listaEsperada = new List<Contacto> { CrearContactoValido(1), CrearContactoValido(2) };
        _repoMock.Setup(r => r.GetAll(pagina)).Returns(listaEsperada);

        var resultado = _service.GetAll(pagina);

        resultado.Should().BeEquivalentTo(listaEsperada);
        _repoMock.Verify(r => r.GetAll(pagina), Times.Once);
    }

    private static Contacto CrearContactoValido(int id = 1)
    {
        return new Contacto
        {
            Id = id,
            Nombre = "Juan Pérez",
            Alias = "juanp",
            Telefono = "+34611223344",
            Correo = "juan@test.com"
        };
    }
}