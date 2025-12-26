using System;
using System.Collections.Generic;
using System.Linq;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Resposta : ValueObject
{
    public Pesquisa Pesquisa { get; private set; }
    public DateTime DataHora { get; private set; }
    
    private readonly List<RespostaItem> _itens;
    public IReadOnlyList<RespostaItem> Itens => _itens.AsReadOnly();

    // Construtor vazio para o EF Core
    private Resposta()
    {
        Pesquisa = null!;
        _itens = [];
    }

    private Resposta(Pesquisa pesquisa, List<RespostaItem> itens)
    {
        Pesquisa = pesquisa;
        DataHora = DateTime.UtcNow;
        _itens = itens;
    }

    public static Resposta Criar(Pesquisa pesquisa, List<string> escolhas)
    {
        if (pesquisa is null)
            throw new DomainException("A pesquisa é obrigatória.");

        if (pesquisa.Status != PesquisaStatus.Ativa)
            throw new DomainException("A pesquisa precisa estar ativa para receber respostas.");

        if (escolhas is null || escolhas.Count != pesquisa.Perguntas.Count)
            throw new DomainException("Todas as perguntas devem ser respondidas.");

        var itens = new List<RespostaItem>();

        for (int i = 0; i < pesquisa.Perguntas.Count; i++)
        {
            var pergunta = pesquisa.Perguntas[i];
            var opcaoEscolhida = escolhas[i];

            if (!pergunta.Opcoes.Contains(opcaoEscolhida))
                throw new DomainException($"A opção '{opcaoEscolhida}' é inválida para a pergunta '{pergunta.Texto}'.");

            itens.Add(new RespostaItem { OpcaoSelecionada = opcaoEscolhida });
        }

        return new Resposta(pesquisa, itens);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Pesquisa;
        yield return DataHora;
        foreach (var item in _itens)
        {
            yield return item;
        }
    }
}