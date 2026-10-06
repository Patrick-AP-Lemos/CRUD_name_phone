namespace ControleAcessos.Models;

/// <summary>Regras de telefone brasileiro: DDD + 8 dígitos (fixo) ou 9 dígitos (celular).</summary>
public static class TelefoneHelper
{
    public static string SomenteDigitos(string? texto) =>
        new((texto ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    public static bool EhValido(string? texto)
    {
        var digitos = SomenteDigitos(texto);
        return digitos.Length is 10 or 11;
    }

    public static string Formatar(string? texto)
    {
        var d = SomenteDigitos(texto);
        return d.Length switch
        {
            11 => $"({d[..2]}) {d[2..7]}-{d[7..]}",
            10 => $"({d[..2]}) {d[2..6]}-{d[6..]}",
            _ => texto ?? string.Empty
        };
    }
}
