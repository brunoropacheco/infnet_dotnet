using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain;

public class ResultadoSumarizado : Entity
{
    public Guid Id { get; set; }
    public Pesquisa Pesquisa { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public TipoResultado Tipo { get; set; }
    public List<Usuario> UsuariosLeitores { get; set; } = new();
    public string DadosConsolidados { get; set; } = string.Empty;
}