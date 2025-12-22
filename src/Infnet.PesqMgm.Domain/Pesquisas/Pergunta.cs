using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class Pergunta : ValueObject
{
    public string Texto { get; set; } = string.Empty;
    public List<string> Opcoes { get; set; } = new();
}