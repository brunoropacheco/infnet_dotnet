using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Infnet.PesqMgm.Domain.Pesquisas;

namespace Infnet.PesqMgm.Domain.Tests;

public class PesquisaTests
{
    [Fact]
    public void CriarPesquisa_DeveInicializarComStatusRascunho()
    {
        // Arrange
        var gestor = Usuario.Criar("Gestor Teste", "gestor@teste.com", PerfilUsuario.Gestor);

        // Act
        var pesquisa = Pesquisa.Criar("Pesquisa Teste", "Descricao", gestor);

        // Assert
        Assert.Equal("Pesquisa Teste", pesquisa.Titulo);
        Assert.Equal(EstadoPesquisa.Rascunho, pesquisa.Status);
        Assert.NotNull(pesquisa.Gestor);
        Assert.Empty(pesquisa.Perguntas);
    }

    [Fact]
    public void AdicionarPergunta_DeveFuncionar_QuandoEmRascunho()
    {
        // Arrange
        var gestor = Usuario.Criar("Gestor", "g@t.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("P1", "D1", gestor);
        var pergunta = Pergunta.Criar("Questão 1?", new List<string> { "A", "B" });

        // Act
        pesquisa.AdicionarPergunta(pergunta);

        // Assert
        Assert.Single(pesquisa.Perguntas);
        Assert.Equal("Questão 1?", pesquisa.Perguntas.First().Texto);
    }

    [Fact]
    public void Publicar_DeveAlterarStatusParaAtiva_QuandoHouverPerguntas()
    {
        // Arrange
        var gestor = Usuario.Criar("Gestor", "g@t.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("P1", "D1", gestor);
        pesquisa.AdicionarPergunta(Pergunta.Criar("Q1", new List<string> { "A" }));

        // Act
        pesquisa.Publicar();

        // Assert
        Assert.Equal(EstadoPesquisa.Ativa, pesquisa.Status);
    }

    [Fact]
    public void AdicionarResposta_DeveRegistrarVoto_QuandoPesquisaAtiva()
    {
        // Arrange
        var gestor = Usuario.Criar("Gestor", "g@t.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("P1", "D1", gestor);
        pesquisa.AdicionarPergunta(Pergunta.Criar("Q1", new List<string> { "Opcao A", "Opcao B" }));
        pesquisa.Publicar();

        var escolhas = new List<string> { "Opcao A" };
        var resposta = Resposta.Criar(pesquisa, escolhas);

        // Act
        pesquisa.AdicionarResposta(resposta);

        // Assert
        Assert.Single(pesquisa.Respostas);
        Assert.Equal("Opcao A", pesquisa.Respostas.First().Itens.First().OpcaoSelecionada);
    }

    [Fact]
    public void GerarResultado_DeveContabilizarVotosCorretamente()
    {
        // Arrange
        var gestor = Usuario.Criar("Gestor", "g@t.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("Eleição", "Desc", gestor);
        var p1 = Pergunta.Criar("Candidato?", new List<string> { "A", "B" });
        pesquisa.AdicionarPergunta(p1);
        pesquisa.Publicar();

        // 2 votos para A, 1 voto para B
        pesquisa.AdicionarResposta(Resposta.Criar(pesquisa, new List<string> { "A" }));
        pesquisa.AdicionarResposta(Resposta.Criar(pesquisa, new List<string> { "A" }));
        pesquisa.AdicionarResposta(Resposta.Criar(pesquisa, new List<string> { "B" }));

        pesquisa.Encerrar();

        // Act
        pesquisa.GerarResultado(new List<Usuario>());

        // Assert
        Assert.NotNull(pesquisa.ResultadoSumarizado);
        var votos = pesquisa.ResultadoSumarizado.ContagemVotos;

        Assert.True(votos.ContainsKey("Candidato?"));
        Assert.Equal(2, votos["Candidato?"]["A"]);
        Assert.Equal(1, votos["Candidato?"]["B"]);
    }
}