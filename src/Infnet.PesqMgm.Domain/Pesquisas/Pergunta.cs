using System.Collections.Generic;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Pergunta : ValueObject
{
    public string Texto { get; private set; }
    private readonly List<string> _opcoes;
    public IReadOnlyList<string> Opcoes => _opcoes.AsReadOnly();

    // Construtor vazio necessário para o EF Core materializar o objeto
    private Pergunta() 
    {
        Texto = string.Empty;
        _opcoes = new List<string>();
    }

    private Pergunta(string texto, List<string> opcoes)
    {
        Texto = texto;
        _opcoes = opcoes;
    }

    public static Pergunta Criar(string texto, List<string> opcoes)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("O texto da pergunta é obrigatório.");

        if (opcoes == null || opcoes.Count < 2)
            throw new DomainException("A pergunta deve ter pelo menos duas opções.");

        return new Pergunta(texto, opcoes);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Texto;
        foreach (var opcao in _opcoes)
        {
            yield return opcao;
        }
    }
}