namespace ControleAcessos.Models;

/// <summary>Entidade do cadastro de acessos. O telefone é guardado apenas com dígitos.</summary>
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    // Usado pela grade para exibir o telefone já formatado.
    public string TelefoneFormatado => TelefoneHelper.Formatar(Telefone);
}
