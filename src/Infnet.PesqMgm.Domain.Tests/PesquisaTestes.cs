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
        string descricao = "Pesquisa de intenção de voto - exemplo"
    )
    {
        var pergunta = new Pergunta
        {
            Texto = "Você votaria no candidato X?",
            Opcoes = new List<string> { "Sim", "Não" }
        };

        var gestor = new Usuario
        {
            Nome = "Gestor Exemplo",
            Email = "gestor@exemplo.com",
            Perfil = PerfilUsuario.Gestor
        };

        var status = PesquisaStatus.Ativa;

        var pesquisa = new Pesquisa
        {
            Titulo = titulo,
            Descricao = descricao,
            Perguntas = new List<Pergunta> { pergunta },
            Gestor = gestor,
            Status = status
        };

        Console.WriteLine($"[PesquisaExemplo] Título: {pesquisa.Titulo} | Status: {pesquisa.Status.GetType().Name} | Gestor: {pesquisa.Gestor?.Nome}");

        return pesquisa;
    }
}