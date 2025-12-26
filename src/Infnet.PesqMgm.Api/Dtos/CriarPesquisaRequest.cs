using System.ComponentModel.DataAnnotations;

namespace Infnet.PesqMgm.Api.Dtos
{
    public class CriarPesquisaRequest
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 500 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do gestor é obrigatório.")]
        public Guid GestorId { get; set; }
    }

    public class PesquisaResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public Guid GestorId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<PerguntaResponse> Perguntas { get; set; } = new List<PerguntaResponse>();
        public DateTime CreatedAt { get; set; }
    }
}