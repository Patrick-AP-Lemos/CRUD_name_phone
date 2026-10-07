using ControleAcessos.Data;
using ControleAcessos.Forms;

namespace ControleAcessos;

//Classe principal do programa. iniciando a aplicação, criando o banco de dados e abrindo o formulário principal.
static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try //Inicializa o banco de dados, criando o arquivo e a tabela Pessoas caso não existam
        {
            Database.Inicializar();
        }
        catch (Exception ex) //Se houver algum erro ao criar o banco de dados, exibe uma mensagem e encerra a aplicação
        {
            MessageBox.Show(
                $"Não foi possível iniciar o banco de dados:\n{ex.Message}",
                "Controle de Acessos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        //Abre o formulário principal, passando o repositório de Pessoas
        Application.Run(new MainForm(new PessoaRepository()));
    }
}
