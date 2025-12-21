namespace Infnet.PesqMgm.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class Gestor
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
