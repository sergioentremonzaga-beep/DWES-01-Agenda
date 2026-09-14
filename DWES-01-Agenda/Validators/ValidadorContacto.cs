using System.Text.RegularExpressions;

namespace DWES_01_Agenda.Validators;

public static class ValidadorContacto
{
    private static readonly Regex RegexTelefono = new(@"^\+?[0-9\s\-]{7,15}$");
    private static readonly Regex RegexCorreo = new(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");

    public static bool ValidarTelefono(string telefono)
    {
        return !string.IsNullOrWhiteSpace(telefono) && RegexTelefono.IsMatch(telefono);
    }

    public static bool ValidarCorreo(string correo)
    {
        return !string.IsNullOrWhiteSpace(correo) && RegexCorreo.IsMatch(correo);
    }

    public static bool ValidarNombre(string nombre)
    {
        return !string.IsNullOrWhiteSpace(nombre);
    }
    
    public static bool ValidarAlias(string alias)
    {
        return !string.IsNullOrWhiteSpace(alias);
    }
}