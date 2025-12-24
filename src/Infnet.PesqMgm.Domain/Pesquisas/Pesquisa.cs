using System;
using System.Collections.Generic;
using System.Linq;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Pesquisa : Entity
{
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    
    private readonly List<Pergunta> _perguntas;
    public IReadOnlyList<Pergunta> Perguntas => _perguntas.AsReadOnly();
    
    public Usuario Gestor { get; private set; }
    public PesquisaStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Pesquisa(string titulo, string descricao, Usuario gestor)
    {
        Titulo = titulo;
        Descricao = descricao;
        Gestor = gestor;
        Status = PesquisaStatus.Rascunho;
        _perguntas = [];
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Pesquisa Criar(string titulo, string descricao, Usuario gestor)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("O título é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("A descrição é obrigatória.");

        if (gestor is null)
            throw new DomainException("O gestor é obrigatório.");

        return new Pesquisa(titulo, descricao, gestor);
    }

    public void AtualizarDados(string titulo, string descricao)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("O título é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("A descrição é obrigatória.");

        Titulo = titulo;
        Descricao = descricao;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdicionarPergunta(Pergunta pergunta)
    {
        if (pergunta is null)
            throw new DomainException("A pergunta não pode ser nula.");
        
        _perguntas.Add(pergunta);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoverPergunta(Pergunta pergunta)
    {
        if (!_perguntas.Contains(pergunta))
            throw new DomainException("Pergunta não encontrada para remoção.");
            
        _perguntas.Remove(pergunta);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publicar()
    {
        if (_perguntas.Count == 0)
            throw new DomainException("A pesquisa precisa de perguntas para ser publicada.");
            
        Status = PesquisaStatus.Ativa;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Encerrar()
    {
        if (Status != PesquisaStatus.Ativa)
            throw new DomainException("Apenas pesquisas ativas podem ser encerradas.");

        Status = PesquisaStatus.Encerrada; // Assumindo que este valor existe no Enum
        UpdatedAt = DateTime.UtcNow;
    }

    public void AlterarGestor(Usuario novoGestor)
    {
        Gestor = novoGestor ?? throw new DomainException("O novo gestor não pode ser nulo.");
        UpdatedAt = DateTime.UtcNow;
    }
}