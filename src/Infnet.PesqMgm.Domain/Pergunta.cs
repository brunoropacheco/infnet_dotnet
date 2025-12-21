namespace Infnet.PesqMgm.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class Pergunta
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Texto { get; set; } = string.Empty;
        public List<string> Opcoes { get; set; } = new();

    }
}
