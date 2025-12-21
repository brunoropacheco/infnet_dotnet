using Infnet.PesqMgm.Domain.Tests;

var pesquisa = PesquisaTests.CriarPesquisaExemplo();
// A linha do helper já escreve no terminal; você pode também escrever info extra:
Console.WriteLine("Pesquisa criada (app): " + pesquisa.Titulo);