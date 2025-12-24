using System;
using System.Collections.Generic;
using System.Linq;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class ResultadoSumarizado : ValueObject
{
    public Pesquisa Pesquisa { get; private set; }
    public DateTime DataApuracao { get; private set; }
    
    private readonly List<Usuario> _leitores;
    public IReadOnlyList<Usuario> Leitores => _leitores.AsReadOnly();
    
    // Estrutura: Pergunta -> (Opção -> Quantidade de Votos)
    private readonly Dictionary<string, Dictionary<string, int>> _contagemVotos;
    public IReadOnlyDictionary<string, Dictionary<string, int>> ContagemVotos => _contagemVotos;

    private ResultadoSumarizado(Pesquisa pesquisa, List<Usuario> leitores, Dictionary<string, Dictionary<string, int>> contagemVotos)
    {
        Pesquisa = pesquisa;
        _leitores = leitores;
        _contagemVotos = contagemVotos;
        DataApuracao = DateTime.UtcNow;
    }

    public static ResultadoSumarizado Criar(Pesquisa pesquisa, List<Usuario> leitores, Dictionary<string, Dictionary<string, int>> contagemVotos)
    {
        if (pesquisa is null)
            throw new DomainException("A pesquisa é obrigatória para gerar o resultado.");

        if (leitores != null && leitores.Any(u => u.Perfil != PerfilUsuario.LeitorDeRelatorios))
            throw new DomainException("Apenas usuários com perfil 'LeitorDeRelatorios' podem ter acesso ao resultado.");

        if (contagemVotos == null)
            throw new DomainException("Os dados da contagem de votos são obrigatórios.");

        return new ResultadoSumarizado(pesquisa, leitores ?? [], contagemVotos);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Pesquisa;
        yield return DataApuracao;

        foreach (var leitor in _leitores.OrderBy(u => u.Email))
        {
            yield return leitor;
        }

        foreach (var pergunta in _contagemVotos.OrderBy(k => k.Key))
        {
            yield return pergunta.Key;
            foreach (var opcao in pergunta.Value.OrderBy(k => k.Key))
            {
                yield return opcao.Key;
                yield return opcao.Value;
            }
        }
    }
}