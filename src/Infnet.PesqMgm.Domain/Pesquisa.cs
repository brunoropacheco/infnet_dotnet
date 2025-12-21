namespace Infnet.PesqMgm.Domain
{
    using System;
    using System.Collections.Generic;

    public class Pesquisa
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public List<Pergunta> Perguntas { get; set; } = new();
        public Gestor Gestor { get; set; } = new();
    }
}

   