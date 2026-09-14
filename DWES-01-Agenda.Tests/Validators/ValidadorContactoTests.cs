using DWES_01_Agenda.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace DWES_01_Agenda.Tests.Validators;

[TestFixture]
public class ValidadorContactoTests
{
    [TestCase("Juan Pérez")]
    [TestCase("A")]
    [TestCase(" María José ")]
    public void ValidarNombre_NombreValido_DevuelveTrue(string nombre)
    {
        var resultado = ValidadorContacto.ValidarNombre(nombre);

        resultado.Should().BeTrue();
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void ValidarNombre_NombreInvalido_DevuelveFalse(string? nombre)
    {
        var resultado = ValidadorContacto.ValidarNombre(nombre!);

        resultado.Should().BeFalse();
    }

    [TestCase("juanp")]
    [TestCase("jperez_99")]
    [TestCase("X")]
    public void ValidarAlias_AliasValido_DevuelveTrue(string alias)
    {
        var resultado = ValidadorContacto.ValidarAlias(alias);

        resultado.Should().BeTrue();
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void ValidarAlias_AliasInvalido_DevuelveFalse(string? alias)
    {
        var resultado = ValidadorContacto.ValidarAlias(alias!);

        resultado.Should().BeFalse();
    }

    [TestCase("+34611223344")]
    [TestCase("611223344")]
    [TestCase("+1 555-1234")]
    [TestCase("123 456 789")]
    [TestCase("1234567")]
    public void ValidarTelefono_TelefonoValido_DevuelveTrue(string telefono)
    {
        var resultado = ValidadorContacto.ValidarTelefono(telefono);

        resultado.Should().BeTrue();
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    [TestCase("123456")]
    [TestCase("1234567890123456")]
    [TestCase("611223344abc")]
    [TestCase("++34611223344")]
    public void ValidarTelefono_TelefonoInvalido_DevuelveFalse(string? telefono)
    {
        var resultado = ValidadorContacto.ValidarTelefono(telefono!);

        resultado.Should().BeFalse();
    }

    [TestCase("usuario@dominio.com")]
    [TestCase("juan.perez@empresa.es")]
    [TestCase("contacto_123@sub.dominio.org")]
    [TestCase("a@b.co")]
    public void ValidarCorreo_CorreoValido_DevuelveTrue(string correo)
    {
        var resultado = ValidadorContacto.ValidarCorreo(correo);

        resultado.Should().BeTrue();
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    [TestCase("usuario")]
    [TestCase("usuario@")]
    [TestCase("@dominio.com")]
    [TestCase("usuario@dominio")]
    [TestCase("usuario@dominio.c")]
    [TestCase("usuario @dominio.com")]
    public void ValidarCorreo_CorreoInvalido_DevuelveFalse(string? correo)
    {
        var resultado = ValidadorContacto.ValidarCorreo(correo!);

        resultado.Should().BeFalse();
    }
}