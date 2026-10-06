# Controle de Acessos — CRUD Desktop

Aplicação desktop para Windows que cadastra pessoas com apenas **Nome** e **Telefone**, com as quatro operações de CRUD (criar, listar/buscar, editar, excluir).

## Stack

| Camada | Tecnologia | Por quê |
|---|---|---|
| Linguagem | C# (.NET 10) | Tipada, orientada a objetos, base de muitos sistemas Windows/PDV |
| Interface | Windows Forms | Janela nativa do Windows, simples para um CRUD |
| Banco | SQLite (`Microsoft.Data.Sqlite`) | Relacional, embarcado, um arquivo, sem instalação, funciona offline |

## Como executar

Executável pronto: `publish\ControleAcessos.exe` (não precisa ter o .NET instalado).

O banco é criado automaticamente na primeira execução em
`%LocalAppData%\ControleAcessos\acessos.db`.

## Como gerar o .exe

```powershell
dotnet publish ControleAcessos/ControleAcessos.csproj -c Release -o publish
```

As opções de publicação (single-file, self-contained, win-x64) já estão no `.csproj`.

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
