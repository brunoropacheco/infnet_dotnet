using Xunit;
using Microsoft.EntityFrameworkCore;
using Infnet.PesqMgm.Infrastructure.Data;
using Infnet.PesqMgm.Domain.Pesquisas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Infnet.PesqMgm.Infrastructure.Data.Tests;

public class PesquisaRepositoryTests
{
    private PesquisaDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PesquisaDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PesquisaDbContext(options);
    }

    [Fact]
    public async Task DeveSalvarERecuperarPesquisaComPerguntas()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var gestor = Usuario.Criar("Gestor Infra", "infra@teste.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("Pesquisa Infra", "Teste BD", gestor);
        pesquisa.AdicionarPergunta(Pergunta.Criar("Q1", new List<string> { "Sim", "Não" }));

        // Act
        using (var context = CreateDbContext(dbName))
        {
            context.Usuarios.Add(gestor);
            context.Pesquisas.Add(pesquisa);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = CreateDbContext(dbName))
        {
            var pesquisaSalva = await context.Pesquisas
                .Include(p => p.Perguntas)
                .Include(p => p.Gestor)
                .FirstOrDefaultAsync(p => p.Id == pesquisa.Id);

            Assert.NotNull(pesquisaSalva);
            Assert.Equal("Pesquisa Infra", pesquisaSalva.Titulo);
            Assert.Single(pesquisaSalva.Perguntas);
            Assert.Equal("Q1", pesquisaSalva.Perguntas.First().Texto);
            // Verifica se a serialização JSON das opções funcionou
            Assert.Equal(2, pesquisaSalva.Perguntas.First().Opcoes.Count);
            Assert.Contains("Sim", pesquisaSalva.Perguntas.First().Opcoes);
        }
    }

    [Fact]
    public async Task DeveSalvarRespostasEResultadoSumarizado()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var gestor = Usuario.Criar("Gestor", "g@t.com", PerfilUsuario.Gestor);
        var pesquisa = Pesquisa.Criar("P1", "D1", gestor);
        pesquisa.AdicionarPergunta(Pergunta.Criar("Q1", new List<string> { "A", "B" }));
        pesquisa.Publicar();
        
        var resposta = Resposta.Criar(pesquisa, new List<string> { "A" });
        pesquisa.AdicionarResposta(resposta);
        
        pesquisa.Encerrar();
        pesquisa.GerarResultado(new List<Usuario>());

        // Act
        using (var context = CreateDbContext(dbName))
        {
            context.Usuarios.Add(gestor);
            context.Pesquisas.Add(pesquisa);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = CreateDbContext(dbName))
        {
            var pesquisaSalva = await context.Pesquisas
                .Include(p => p.ResultadoSumarizado)
                .FirstOrDefaultAsync(p => p.Id == pesquisa.Id);

            Assert.NotNull(pesquisaSalva);
            Assert.NotNull(pesquisaSalva.ResultadoSumarizado);
            Assert.True(pesquisaSalva.ResultadoSumarizado.ContagemVotos.ContainsKey("Q1"));
        }
    }
}
