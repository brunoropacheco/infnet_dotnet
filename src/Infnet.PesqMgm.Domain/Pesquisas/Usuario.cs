using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain;

public class Usuario : Entity
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}