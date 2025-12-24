using System;
using System.Collections.Generic;
using System.Linq;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Resposta : ValueObject
{
    public Pesquisa Pesquisa { get; private set; }
    public DateTime DataHora { get; private set; }
    
    // Armazena o Texto da Pergunta e a Opção Escolhida
    private readonly Dictionary<string, string> _escolhas;
    public IReadOnlyDictionary<string, string> Escolhas => _escolhas;

    private Resposta(Pesquisa pesquisa)
    {
        Pesquisa = pesquisa;
        DataHora = DateTime.UtcNow;
        _escolhas = [];
    }

    public static Resposta Criar(Pesquisa pesquisa)
    {
        if (pesquisa is null)
            throw new DomainException("A pesquisa é obrigatória.");

        return new Resposta(pesquisa);
    }

    public void RegistrarEscolha(Pergunta pergunta, string opcaoEscolhida)
    {
        if (pergunta is null)
            throw new DomainException("A pergunta não pode ser nula.");

        if (string.IsNullOrWhiteSpace(opcaoEscolhida))
            throw new DomainException("A opção escolhida é inválida.");

        // Valida se a opção existe na pergunta
        if (!pergunta.Opcoes.Contains(opcaoEscolhida))
            throw new DomainException($"A opção '{opcaoEscolhida}' não é válida para a pergunta '{pergunta.Texto}'.");

        // Adiciona ou atualiza a escolha
        if (_escolhas.ContainsKey(pergunta.Texto))
            _escolhas[pergunta.Texto] = opcaoEscolhida;
        else
            _escolhas.Add(pergunta.Texto, opcaoEscolhida);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Pesquisa;
        yield return DataHora;
        foreach (var item in _escolhas.OrderBy(k => k.Key))
        {
            yield return item.Key;
            yield return item.Value;
        }
    }
}