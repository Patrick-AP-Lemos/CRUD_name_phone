using ControleAcessos.Data;
using ControleAcessos.Forms;

namespace ControleAcessos;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            Database.Inicializar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível iniciar o banco de dados:\n{ex.Message}",
                "Controle de Acessos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new MainForm(new PessoaRepository()));
    }
}
