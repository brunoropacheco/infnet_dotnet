using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain;

public class Resposta : Entity
{
    public Guid Id { get; set; }
    public Pesquisa Pesquisa { get; set; } = null!;
    public Usuario Respondente { get; set; } = null!;
    public List<RespostaItem> Selecao { get; set; } = new();
}