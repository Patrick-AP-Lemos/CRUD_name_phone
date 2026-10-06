using ControleAcessos.Data;
using ControleAcessos.Models;

namespace ControleAcessos.Forms;

/// <summary>Tela única do sistema: formulário de cadastro, busca e grade de registros.</summary>
public class MainForm : Form
{
    private readonly PessoaRepository _repositorio;

    private readonly TextBox _txtBusca = new() { PlaceholderText = "Buscar por nome..." };
    private readonly DataGridView _grade = new();
    private readonly TextBox _txtNome = new() { MaxLength = 100 };
    private readonly TextBox _txtTelefone = new() { MaxLength = 20, PlaceholderText = "(11) 91234-5678" };
    private readonly Button _btnNovo = new() { Text = "Novo" };
    private readonly Button _btnSalvar = new() { Text = "Salvar" };
    private readonly Button _btnExcluir = new() { Text = "Excluir" };
    private readonly Label _lblStatus = new() { AutoSize = true };

    // Id do registro em edição; null indica que o próximo "Salvar" cria um novo registro.
    private int? _idEmEdicao;

    // Ao recarregar a grade, o DataGridView dispara SelectionChanged sozinho; este indicador
    // evita que isso sobrescreva o formulário (ex.: enquanto o usuário digita na busca).
    private bool _recarregandoGrade;

    public MainForm(PessoaRepository repositorio)
    {
        _repositorio = repositorio;
        MontarInterface();
        CarregarGrade();
        LimparFormulario();
    }

    private void MontarInterface()
    {
        Text = "Controle de Acessos";
        MinimumSize = new Size(640, 480);
        Size = new Size(760, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);

        // Painel superior: busca
        var painelBusca = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(12, 10, 12, 6) };
        _txtBusca.Dock = DockStyle.Fill;
        _txtBusca.TextChanged += (_, _) => CarregarGrade();
        painelBusca.Controls.Add(_txtBusca);

        // Painel inferior: formulário de cadastro
        var painelForm = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 150,
            Padding = new Padding(12),
            ColumnCount = 2,
            RowCount = 4
        };
        painelForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        painelForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < 3; i++) painelForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        painelForm.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        painelForm.Controls.Add(new Label { Text = "Nome", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _txtNome.Dock = DockStyle.Fill;
        painelForm.Controls.Add(_txtNome, 1, 0);

        painelForm.Controls.Add(new Label { Text = "Telefone", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        _txtTelefone.Dock = DockStyle.Fill;
        painelForm.Controls.Add(_txtTelefone, 1, 1);

        var botoes = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        foreach (var b in new[] { _btnNovo, _btnSalvar, _btnExcluir })
        {
            b.AutoSize = true;
            b.MinimumSize = new Size(90, 32);
            botoes.Controls.Add(b);
        }
        painelForm.Controls.Add(botoes, 1, 2);
        painelForm.Controls.Add(_lblStatus, 1, 3);

        // Grade
        _grade.Dock = DockStyle.Fill;
        _grade.AutoGenerateColumns = false;
        _grade.ReadOnly = true;
        _grade.AllowUserToAddRows = false;
        _grade.AllowUserToDeleteRows = false;
        _grade.AllowUserToResizeRows = false;
        _grade.MultiSelect = false;
        _grade.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grade.RowHeadersVisible = false;
        _grade.BackgroundColor = SystemColors.Window;
        _grade.BorderStyle = BorderStyle.None;
        _grade.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pessoa.Id), HeaderText = "Id", Width = 60
        });
        _grade.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pessoa.Nome), HeaderText = "Nome",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        _grade.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pessoa.TelefoneFormatado), HeaderText = "Telefone", Width = 170
        });
        _grade.SelectionChanged += (_, _) => CarregarSelecaoNoFormulario();

        // A ordem de inclusão importa no Dock: o WinForms posiciona primeiro os controles
        // adicionados por último (Top e Bottom), e o Fill, adicionado antes, ocupa o espaço que sobra.
        Controls.Add(_grade);
        Controls.Add(painelForm);
        Controls.Add(painelBusca);

        _btnNovo.Click += (_, _) => LimparFormulario();
        _btnSalvar.Click += (_, _) => Salvar();
        _btnExcluir.Click += (_, _) => Excluir();

        _txtNome.KeyDown += (s, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            _txtTelefone.Focus();
        };
        _txtTelefone.KeyDown += (s, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            Salvar();
        };
        _txtTelefone.KeyPress += (s, e) =>
        {
            // Aceita só dígitos e caracteres comuns de formatação.
            if (!char.IsControl(e.KeyChar) && !char.IsAsciiDigit(e.KeyChar) && "()- +".IndexOf(e.KeyChar) < 0)
                e.Handled = true;
        };
    }

    private void CarregarGrade(int? idParaSelecionar = null)
    {
        _recarregandoGrade = true;
        try
        {
            var pessoas = _repositorio.Listar(_txtBusca.Text);
            _grade.DataSource = pessoas;
            _grade.ClearSelection();

            if (idParaSelecionar is { } id)
            {
                foreach (DataGridViewRow linha in _grade.Rows)
                {
                    if (linha.DataBoundItem is Pessoa p && p.Id == id)
                    {
                        linha.Selected = true;
                        _grade.CurrentCell = linha.Cells[1];
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MostrarErro("Não foi possível carregar os registros.", ex);
        }
        finally
        {
            _recarregandoGrade = false;
        }
    }

    private void CarregarSelecaoNoFormulario()
    {
        if (_recarregandoGrade || _grade.SelectedRows.Count == 0 || _grade.SelectedRows[0].DataBoundItem is not Pessoa p)
            return;

        _idEmEdicao = p.Id;
        _txtNome.Text = p.Nome;
        _txtTelefone.Text = p.TelefoneFormatado;
        _btnExcluir.Enabled = true;
        _lblStatus.Text = $"Editando registro #{p.Id}";
    }

    private void LimparFormulario()
    {
        _idEmEdicao = null;
        _txtNome.Clear();
        _txtTelefone.Clear();
        _grade.ClearSelection();
        _btnExcluir.Enabled = false;
        _lblStatus.Text = "Novo registro";
        _txtNome.Focus();
    }

    private void Salvar()
    {
        var nome = _txtNome.Text.Trim();
        var telefone = TelefoneHelper.SomenteDigitos(_txtTelefone.Text);

        if (nome.Length == 0)
        {
            MostrarAviso("Informe o nome.");
            _txtNome.Focus();
            return;
        }
        if (!TelefoneHelper.EhValido(telefone))
        {
            MostrarAviso("Telefone inválido. Informe DDD + número (10 ou 11 dígitos).");
            _txtTelefone.Focus();
            return;
        }

        try
        {
            var pessoa = new Pessoa { Nome = nome, Telefone = telefone };
            int id;
            if (_idEmEdicao is { } idExistente)
            {
                pessoa.Id = idExistente;
                if (!_repositorio.Atualizar(pessoa))
                {
                    MostrarAviso("O registro não existe mais (pode ter sido excluído).");
                    CarregarGrade();
                    LimparFormulario();
                    return;
                }
                id = idExistente;
            }
            else
            {
                id = _repositorio.Inserir(pessoa);
            }

            // O filtro de busca pode esconder o registro recém-salvo; limpar evita confusão.
            if (_txtBusca.Text.Length > 0) _txtBusca.Clear();
            CarregarGrade(id);
            LimparFormulario();
            _lblStatus.Text = "Registro salvo com sucesso.";
        }
        catch (Exception ex)
        {
            MostrarErro("Não foi possível salvar o registro.", ex);
        }
    }

    private void Excluir()
    {
        if (_idEmEdicao is not { } id) return;

        var confirmacao = MessageBox.Show(
            $"Excluir o registro de \"{_txtNome.Text}\"?\nEsta ação não pode ser desfeita.",
            "Confirmar exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (confirmacao != DialogResult.Yes) return;

        try
        {
            _repositorio.Excluir(id);
            CarregarGrade();
            LimparFormulario();
            _lblStatus.Text = "Registro excluído.";
        }
        catch (Exception ex)
        {
            MostrarErro("Não foi possível excluir o registro.", ex);
        }
    }

    private void MostrarAviso(string mensagem) =>
        MessageBox.Show(mensagem, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void MostrarErro(string mensagem, Exception ex) =>
        MessageBox.Show($"{mensagem}\n\nDetalhe: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
}
