using Microsoft.Data.Sqlite;

namespace ControleAcessos.Data;

/// <summary>Cuida da conexão com o SQLite e da criação do esquema na primeira execução.</summary>
public static class Database
{
    public static string CaminhoArquivo { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ControleAcessos",
        "acessos.db");

    private static string ConnectionString => new SqliteConnectionStringBuilder
    {
        DataSource = CaminhoArquivo,
        Mode = SqliteOpenMode.ReadWriteCreate
    }.ToString();

    public static SqliteConnection CriarConexao() => new(ConnectionString);

    public static void Inicializar()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CaminhoArquivo)!);

        using var conexao = CriarConexao();
        conexao.Open();

        using var comando = conexao.CreateCommand();
        comando.CommandText = """
            CREATE TABLE IF NOT EXISTS Pessoas (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome     TEXT NOT NULL,
                Telefone TEXT NOT NULL
            );
            """;
        comando.ExecuteNonQuery();
    }
}
