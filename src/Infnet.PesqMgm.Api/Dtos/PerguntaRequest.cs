namespace Infnet.PesqMgm.Api.Dtos
{
    public class PerguntaRequest
    {
        public string Texto { get; set; } = string.Empty;
        public List<string> Opcoes { get; set; } = new List<string>();
    }

    public class PerguntaResponse
    {
        public string Texto { get; set; } = string.Empty;
        public List<string> Opcoes { get; set; } = new List<string>();
    }
}