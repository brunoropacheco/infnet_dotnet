using System;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Usuario : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public PerfilUsuario Perfil { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Usuario(string nome, string email, PerfilUsuario perfil)
    {
        Nome = nome;
        Email = email;
        Perfil = perfil;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Usuario Criar(string nome, string email, PerfilUsuario perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new DomainException("O email é inválido.");

        return new Usuario(nome, email, perfil);
    }

    public void AlterarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new DomainException("O nome é obrigatório.");

        Nome = novoNome;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AlterarEmail(string novoEmail)
    {
        if (string.IsNullOrWhiteSpace(novoEmail) || !novoEmail.Contains("@"))
            throw new DomainException("O email é inválido.");

        Email = novoEmail;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AlterarPerfil(PerfilUsuario novoPerfil)
    {
        Perfil = novoPerfil;
        UpdatedAt = DateTime.UtcNow;
    }
}