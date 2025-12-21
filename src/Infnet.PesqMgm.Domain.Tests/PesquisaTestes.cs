using System;
using System.Collections.Generic;
using Infnet.PesqMgm.Domain;

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
        string titulo = "Eleição Municipal 2025",
        string descricao = "Pesquisa de intenção de voto - exemplo",
        DateOnly? inicio = null,
        DateOnly? fim = null)
    {
        var dataInicio = inicio ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var dataFim = fim ?? DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7));

        var pergunta = new Pergunta
        {
            Texto = "Você votaria no candidato X?",
            Opcoes = new List<string> { "Sim", "Não" }
        };

        var gestor = new Gestor
        {
            Nome = "Gestor Exemplo",
            Email = "gestor@exemplo.com"
        };

        var pesquisa = new Pesquisa
        {
            Titulo = titulo,
            Descricao = descricao,
            DataInicio = dataInicio,
            DataFim = dataFim,
            Perguntas = new List<Pergunta> { pergunta },
            Gestor = gestor
        };

        Console.WriteLine($"[PesquisaExemplo] Título: {pesquisa.Titulo} | Período: {pesquisa.DataInicio:yyyy-MM-dd} -> {pesquisa.DataFim:yyyy-MM-dd} | Gestor: {pesquisa.Gestor?.Nome}");

        return pesquisa;
    }
}