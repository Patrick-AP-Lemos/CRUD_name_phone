using ControleAcessos.Models;

namespace ControleAcessos.Data;

/// <summary>Operações de CRUD sobre a tabela Pessoas. Todo SQL usa parâmetros (sem concatenação).</summary>
public class PessoaRepository
{
    // Create
    public int Inserir(Pessoa pessoa)
    {
        using var conexao = Database.CriarConexao();
        conexao.Open();

        using var cmd = conexao.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Pessoas (Nome, Telefone) VALUES ($nome, $telefone);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$nome", pessoa.Nome);
        cmd.Parameters.AddWithValue("$telefone", pessoa.Telefone);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // Read
    public List<Pessoa> Listar(string? filtroNome = null)
    {
        var lista = new List<Pessoa>();

        using var conexao = Database.CriarConexao();
        conexao.Open();

        using var cmd = conexao.CreateCommand();
        cmd.CommandText = """
            SELECT Id, Nome, Telefone
            FROM Pessoas
            WHERE $filtro IS NULL OR Nome LIKE '%' || $filtro || '%'
            ORDER BY Nome COLLATE NOCASE, Id;
            """;
        var filtro = string.IsNullOrWhiteSpace(filtroNome) ? null : filtroNome.Trim();
        cmd.Parameters.AddWithValue("$filtro", (object?)filtro ?? DBNull.Value);

        using var leitor = cmd.ExecuteReader();
        while (leitor.Read())
        {
            lista.Add(new Pessoa
            {
                Id = leitor.GetInt32(0),
                Nome = leitor.GetString(1),
                Telefone = leitor.GetString(2)
            });
        }
        return lista;
    }

    // Update
    public bool Atualizar(Pessoa pessoa)
    {
        using var conexao = Database.CriarConexao();
        conexao.Open();

        using var cmd = conexao.CreateCommand();
        cmd.CommandText = "UPDATE Pessoas SET Nome = $nome, Telefone = $telefone WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$nome", pessoa.Nome);
        cmd.Parameters.AddWithValue("$telefone", pessoa.Telefone);
        cmd.Parameters.AddWithValue("$id", pessoa.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // Delete
    public bool Excluir(int id)
    {
        using var conexao = Database.CriarConexao();
        conexao.Open();

        using var cmd = conexao.CreateCommand();
        cmd.CommandText = "DELETE FROM Pessoas WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
