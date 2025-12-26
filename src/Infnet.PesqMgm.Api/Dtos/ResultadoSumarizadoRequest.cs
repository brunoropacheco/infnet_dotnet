namespace Infnet.PesqMgm.Api.Dtos
{
    public class ResultadoSumarizadoRequest
    {
        public Guid PesquisaId { get; set; }
        public List<Guid> LeitoresIds { get; set; } = new List<Guid>();
    }

    public class ResultadoSumarizadoResponse
    {
        public DateTime DataApuracao { get; set; }
        public List<UsuarioResponse> Leitores { get; set; } = new List<UsuarioResponse>();
        public Dictionary<string, Dictionary<string, int>> ContagemVotos { get; set; } = new Dictionary<string, Dictionary<string, int>>();
    }
}