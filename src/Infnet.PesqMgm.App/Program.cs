using System.Linq;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Domain.Tests;
using Infnet.PesqMgm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

//var pesquisa = PesquisaTests.CriarPesquisaExemplo();
// A linha do helper já escreve no terminal; você pode também escrever info extra:
//Console.WriteLine("Pesquisa criada (app): " + pesquisa.Titulo);


// 1. Configuração do Banco em Memória
var options = new DbContextOptionsBuilder<PesquisaDbContext>()
    .UseInMemoryDatabase("PesquisaDb_Testes")
    .Options;

// 2. Cenário de Escrita (Persistência)
using (var context = new PesquisaDbContext(options))
{
    Console.WriteLine("--- Iniciando Persistência ---");
    var pesquisas = PesquisaTests.CriarCenarioEleicoes();
    
    context.Pesquisas.AddRange(pesquisas);

    context.SaveChanges();
    Console.WriteLine($">> {pesquisas.Count} pesquisa(s) salva(s) no banco em memória com sucesso.");
}

// 3. Cenário de Leitura (Consulta)
using (var context = new PesquisaDbContext(options))
{
    Console.WriteLine("\n--- Lendo do Banco de Dados ---");
    var pesquisasSalvas = context.Pesquisas
        .Include(p => p.Gestor) // Carrega dados relacionados se houver navegação
        .Include(p => p.Perguntas)
        .Include(p => p.ResultadoSumarizado)
        .ThenInclude(r => r.Leitores)
        .ToList();

    foreach (var pesquisa in pesquisasSalvas)
    {
        Console.WriteLine($"\n[Recuperado] Título: {pesquisa.Titulo}");
        Console.WriteLine($"[Recuperado] Gestor: {pesquisa.Gestor?.Nome}");
        Console.WriteLine("Perguntas:");
        foreach (var p in pesquisa.Perguntas)
        {
            Console.WriteLine($" - {p.Texto} (Opções: {string.Join(", ", p.Opcoes)})");
        }

        if (pesquisa.ResultadoSumarizado != null)
        {
            Console.WriteLine("\n   [Resultado Sumarizado]");
            Console.WriteLine($"   Data Apuração: {pesquisa.ResultadoSumarizado.DataApuracao}");
            Console.WriteLine("   Contagem de Votos:");
            foreach (var pergunta in pesquisa.ResultadoSumarizado.ContagemVotos)
            {
                Console.WriteLine($"     Pergunta: {pergunta.Key}");
                foreach (var opcao in pergunta.Value)
                {
                    Console.WriteLine($"       - {opcao.Key}: {opcao.Value} votos");
                }
            }
            Console.WriteLine($"   Leitores Autorizados: {string.Join(", ", pesquisa.ResultadoSumarizado.Leitores.Select(l => l.Nome))}");
        }
    }

    Console.WriteLine("\n--- Lendo Respostas ---");
    // Como Respostas não tem DbSet, carregamos via Pesquisa
    var pesquisasComRespostas = context.Pesquisas
        .Include(p => p.Respostas)
        .ThenInclude(r => r.Itens)
        .ToList();

    foreach (var r in pesquisasComRespostas.SelectMany(p => p.Respostas))
    {
        Console.WriteLine($"[Resposta] Pesquisa: {r.Pesquisa.Titulo} | Data: {r.DataHora}");
        Console.WriteLine($"   - Escolhas: {string.Join(", ", r.Itens.Select(i => i.OpcaoSelecionada))}");
    }
}

// 4. Inspeção Geral (Dump do Banco)
// Como o banco é em memória, não é possível conectar via SQL Management Studio.
// A única forma de ver os dados é imprimindo-os via código.
using (var context = new PesquisaDbContext(options))
{
    Console.WriteLine("\n--- [DEBUG] Dump das Tabelas ---");

    Console.WriteLine(">> Tabela: Usuarios");
    foreach (var u in context.Usuarios)
    {
        // Acessando Shadow Property 'Id' (configurada no DbContext)
        var id = context.Entry(u).Property("Id").CurrentValue;
        Console.WriteLine($"   ID: {id} | Nome: {u.Nome} | Email: {u.Email} | Perfil: {u.Perfil}");
    }

    Console.WriteLine(">> Tabela: Pesquisas");
    var pesquisasDump = context.Pesquisas
        .Include(p => p.Gestor)
        .Include(p => p.Perguntas);

    foreach (var p in pesquisasDump)
    {
        Console.WriteLine($"   ID: {p.Id} | Título: {p.Titulo} | Status: {p.Status} | Gestor: {p.Gestor?.Nome} | Qtd Perguntas: {p.Perguntas.Count}");
        foreach (var perg in p.Perguntas)
        {
            Console.WriteLine($"      - {perg.Texto} [Opções: {string.Join(", ", perg.Opcoes)}]");
        }
    }
}

Console.WriteLine("\nPressione ENTER para encerrar...");
Console.ReadLine();