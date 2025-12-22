using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain;

public class RespostaItem : ValueObject
{
    public string OpcaoSelecionada { get; set; } = string.Empty;
}