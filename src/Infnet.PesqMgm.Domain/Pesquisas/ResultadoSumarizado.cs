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

    // Construtor vazio para o EF Core
    private ResultadoSumarizado()
    {
        Pesquisa = null!;
        _leitores = [];
        _contagemVotos = [];
    }

    private ResultadoSumarizado(Pesquisa pesquisa, List<Usuario> leitores)
    {
        Pesquisa = pesquisa;
        _leitores = leitores;
        _contagemVotos = CalcularVotos(pesquisa);
        DataApuracao = DateTime.UtcNow;
    }

    public static ResultadoSumarizado Criar(Pesquisa pesquisa, List<Usuario> leitores)
    {
        if (pesquisa is null)
            throw new DomainException("A pesquisa é obrigatória para gerar o resultado.");

        if (pesquisa.Status != PesquisaStatus.Encerrada)
            throw new DomainException("A pesquisa precisa estar encerrada para gerar o resultado.");

        if (leitores != null && leitores.Any(u => u.Perfil != PerfilUsuario.LeitorDeRelatorios))
            throw new DomainException("Apenas usuários com perfil 'LeitorDeRelatorios' podem ter acesso ao resultado.");
        return new ResultadoSumarizado(pesquisa, leitores ?? []);
    }
    private static Dictionary<string, Dictionary<string, int>> CalcularVotos(Pesquisa pesquisa)
    {
        var contagemVotos = new Dictionary<string, Dictionary<string, int>>();
        for (int i = 0; i < pesquisa.Perguntas.Count; i++)
        {
            var p = pesquisa.Perguntas[i];
            var votos = p.Opcoes.ToDictionary(k => k, v => 0);
            
            foreach (var r in pesquisa.Respostas)
            {
                // Assume-se que a ordem dos itens na resposta corresponde à ordem das perguntas
                if (i < r.Itens.Count)
                {
                    var opcao = r.Itens[i].OpcaoSelecionada;
                    if (votos.ContainsKey(opcao)) votos[opcao]++;
                }
            }
            contagemVotos[p.Texto] = votos;
        }
        return contagemVotos;
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