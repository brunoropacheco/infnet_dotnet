using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain;

public class Pesquisa : Entity
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public PesquisaStatus Status { get; set; }
    public List<Pergunta> Perguntas { get; set; } = new();
    public Usuario Gestor { get; set; } = null!;
}