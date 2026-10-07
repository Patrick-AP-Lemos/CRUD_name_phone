# Controle de Acessos — CRUD Desktop

Aplicação desktop para Windows que cadastra pessoas com apenas **Nome** e **Telefone**, com as quatro operações de CRUD (criar, listar/buscar, editar, excluir).

## Stack

| Camada | Tecnologia | Por quê |
|---|---|---|
| Linguagem | C# (.NET 10) | Tipada, orientada a objetos, base de muitos sistemas Windows/PDV |
| Interface | Windows Forms | Janela nativa do Windows, simples para um CRUD |
| Banco | SQLite (`Microsoft.Data.Sqlite`) | Relacional, embarcado, um arquivo, sem instalação, funciona offline |

## Como executar

### Opção 1: executável pronto (recomendado)

1. Baixe o `ControleAcessos.exe` na [última release](https://github.com/Patrick-AP-Lemos/CRUD_name_phone/releases/latest).
2. Dê dois cliques no arquivo. Não é preciso instalar o .NET.

> Como o executável não tem assinatura digital, o Windows pode exibir o aviso do SmartScreen ("O Windows protegeu seu computador"). Clique em **Mais informações** e depois em **Executar assim mesmo**.

### Opção 2: a partir do código-fonte

Requisitos: Windows e [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/Patrick-AP-Lemos/CRUD_name_phone.git
cd CRUD_name_phone
dotnet run --project ControleAcessos/ControleAcessos.csproj
```

O banco é criado automaticamente na primeira execução em
`%LocalAppData%\ControleAcessos\acessos.db`.

## Como usar

- **Novo registro:** preencha Nome e Telefone (DDD + número) e clique em **Salvar** (ou pressione `Enter` no campo Telefone).
- **Editar:** clique em uma linha da grade, altere os campos e clique em **Salvar**.
- **Excluir:** selecione a linha, clique em **Excluir** e confirme.
- **Buscar:** digite parte do nome no campo de busca; a grade filtra enquanto você digita.
- **Novo (botão):** limpa o formulário para iniciar outro cadastro.

## Como gerar o .exe

```powershell
dotnet publish ControleAcessos/ControleAcessos.csproj -c Release -o publish
```

O arquivo é gerado em `publish\ControleAcessos.exe`. As opções de publicação (single-file, self-contained, win-x64) já estão no `.csproj`. A pasta `publish` não é versionada: o executável fica disponível nas releases.

## Estrutura

```
ControleAcessos/
  Program.cs                  # entrada: inicializa o banco e abre a tela
  Models/Pessoa.cs            # entidade (Id, Nome, Telefone)
  Models/TelefoneHelper.cs    # validação e formatação de telefone
  Data/Database.cs            # conexão e criação da tabela
  Data/PessoaRepository.cs    # CRUD com SQL parametrizado
  Forms/MainForm.cs           # interface
```

## Regras e decisões

- **SQL parametrizado** em todas as consultas (sem risco de SQL injection).
- **Validação**: nome obrigatório; telefone brasileiro com DDD (10 ou 11 dígitos). O telefone é gravado só com dígitos e exibido formatado.
- **Confirmação** antes de excluir; erros de banco viram mensagem amigável, sem derrubar o app.
- **Atalhos**: `Enter` no Nome vai para o Telefone; `Enter` no Telefone salva.
- **Busca** por nome (parcial, sem diferenciar maiúsculas de minúsculas para letras ASCII) enquanto digita.

## Limitações conhecidas

- Sem autenticação de usuários nem log de acessos: o escopo pedido é o cadastro com Nome e Telefone.
- Telefones internacionais não são aceitos (somente formato brasileiro).
- Não impede nomes ou telefones duplicados.
- A busca e a ordenação ignoram maiúsculas apenas para letras ASCII: "joão" não encontra "JOÃO", e nomes iniciados em letra acentuada (como "Álvaro") podem aparecer depois do "Z".
- Os caracteres `%` e `_` digitados na busca funcionam como curingas do SQL.
