using System;
using System.Collections.Generic;
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
        var pergunta = Pergunta.Criar("Você votaria no candidato X?", new List<string> { "Sim", "Não" });

        var gestor = Usuario.Criar("Gestor Exemplo", "gestor@exemplo.com", PerfilUsuario.Gestor);

        var pesquisa = Pesquisa.Criar(titulo, descricao, gestor);
        pesquisa.AdicionarPergunta(pergunta);
        pesquisa.Publicar();

        Console.WriteLine($"[PesquisaExemplo] Título: {pesquisa.Titulo} | Status: {pesquisa.Status} | Gestor: {pesquisa.Gestor?.Nome}");

        return pesquisa;
    }
}