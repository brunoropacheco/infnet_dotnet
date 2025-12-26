namespace Infnet.PesqMgm.Api.Dtos
{
    public class RespostaRequest
    {
        public Guid PesquisaId { get; set; }
        public List<RespostaItemRequest> Itens { get; set; } = new List<RespostaItemRequest>();
    }

    public class RespostaResponse
    {
        public DateTime DataHora { get; set; }
        public List<RespostaItemResponse> Itens { get; set; } = new List<RespostaItemResponse>();
    }
}