using System.Collections.Generic;
using Infnet.PesqMgm.Domain.Shared;

namespace Infnet.PesqMgm.Domain.Pesquisas;

public class RespostaItem : ValueObject
{
    public string OpcaoSelecionada { get; set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return OpcaoSelecionada;
    }
}