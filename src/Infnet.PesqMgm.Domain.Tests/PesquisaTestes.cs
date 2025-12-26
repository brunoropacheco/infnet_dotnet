using System;
using System.Collections.Generic;
using System.Linq;
using Infnet.PesqMgm.Domain.Pesquisas;

namespace Infnet.PesqMgm.Domain.Tests;

// Biblioteca de suporte / placeholder. Contém helpers para testes de domínio.
public static class PesquisaTests
{
    public static string Info => "Project for domain tests (class library placeholder)";

    /// <summary>
    /// Cria uma instância de exemplo de <see cref="Pesquisa"/> com uma pergunta e duas opções.
    /// Útil para testes unitários e cenários de integração dentro deste contexto.
    /// </summary>
    public static Pesquisa CriarPesquisaExemplo(
        string titulo = "Pesquisa de Exemplo",
        string descricao = "Pesquisa de intenção de voto - exemplo"
    )
    {
        var pergunta1 = Pergunta.Criar("Você votaria no candidato X?", new List<string> { "Sim", "Não" });
        var pergunta2 = Pergunta.Criar("Qual seu sexo?", new List<string> { "Masculino", "Feminino" });

        var gestor = Usuario.Criar("Gestor Exemplo", "gestor@exemplo.com", PerfilUsuario.Gestor);

        var pesquisa = Pesquisa.Criar(titulo, descricao, gestor);
        pesquisa.AdicionarPergunta(pergunta1);
        pesquisa.AdicionarPergunta(pergunta2);
        pesquisa.Publicar();

        Console.WriteLine($"[PesquisaExemplo] Título: {pesquisa.Titulo} | Status: {pesquisa.Status} | Gestor: {pesquisa.Gestor?.Nome}");
        Console.WriteLine("Perguntas:");
        foreach (var p in pesquisa.Perguntas)
        {
            Console.WriteLine($" - {p.Texto} (Opções: {string.Join(", ", p.Opcoes)})");
        }

        // Simula uma resposta para a pesquisa criada
        Console.WriteLine("\n[Simulação] Criando uma resposta de exemplo...");
        var escolhas = new List<string> { "Sim", "Masculino" };
        var resposta = Resposta.Criar(pesquisa, escolhas);
        
        Console.WriteLine($"Resposta registrada em: {resposta.DataHora}");
        Console.WriteLine($" - Opções escolhidas: {string.Join(", ", resposta.Itens.Select(i => i.OpcaoSelecionada))}");

        return pesquisa;
    }

    public static List<Pesquisa> CriarCenarioEleicoes()
    {
        var lista = new List<Pesquisa>();
        var gestor = Usuario.Criar("Gestor Eleitoral", "eleicoes@tse.jus.br", PerfilUsuario.Gestor);

        // 1. Criar Pesquisa
        var pesquisa = Pesquisa.Criar("Eleições Presidenciais 2026", "Intenção de voto para Presidente", gestor);
        pesquisa.AdicionarPergunta(Pergunta.Criar("Em qual candidato vota para Presidente?", new List<string> { "Candidato A", "Candidato B", "Candidato C", "Branco/Nulo" }));
        pesquisa.AdicionarPergunta(Pergunta.Criar("Qual sua faixa de idade?", new List<string> { "16-24", "25-34", "35-44", "45-59", "60+" }));
        pesquisa.Publicar();

        // 2. Criar 5 Respostas
        var respostas = GerarRespostasAleatorias(pesquisa, 5);
        foreach (var r in respostas) pesquisa.AdicionarResposta(r);

        // 3. Encerrar Pesquisa (Necessário para gerar resultado)
        pesquisa.Encerrar();

        // 4. Gerar Resultado Sumarizado
        
        var leitor1 = Usuario.Criar("Leitor 1", "leitor1@dominio.com", PerfilUsuario.LeitorDeRelatorios);
        var leitor2 = Usuario.Criar("Leitor 2", "leitor2@dominio.com", PerfilUsuario.LeitorDeRelatorios);
        
        pesquisa.GerarResultado(new List<Usuario> { leitor1, leitor2 });

        if (pesquisa.ResultadoSumarizado != null)
            Console.WriteLine($"[Setup] Resultado gerado em {pesquisa.ResultadoSumarizado.DataApuracao}. Leitores autorizados: {pesquisa.ResultadoSumarizado.Leitores.Count}");

        lista.Add(pesquisa);
        return lista;
    }

    public static List<Resposta> GerarRespostasAleatorias(Pesquisa pesquisa, int quantidade)
    {
        var respostas = new List<Resposta>();
        var random = new Random();

        for (int i = 0; i < quantidade; i++)
        {
            var escolhas = new List<string>();
            foreach (var pergunta in pesquisa.Perguntas)
            {
                var indiceAleatorio = random.Next(pergunta.Opcoes.Count);
                escolhas.Add(pergunta.Opcoes[indiceAleatorio]);
            }
            respostas.Add(Resposta.Criar(pesquisa, escolhas));
        }
        return respostas;
    }
}