using Microsoft.AspNetCore.Mvc;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Api.Dtos;
using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Domain.Shared;
using Infnet.PesqMgm.Infrastructure.Data.Repositories;

namespace Infnet.PesqMgm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PesquisasController : ControllerBase
{
    private readonly ILogger<PesquisasController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPesquisaRepository _pesquisaRepository;

    public PesquisasController(ILogger<PesquisasController> logger, IUsuarioRepository usuarioRepository, IPesquisaRepository pesquisaRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
        _pesquisaRepository = pesquisaRepository;
    }

    private PesquisaResponse MapToPesquisaResponse(Pesquisa pesquisa)
    {
        return new PesquisaResponse(
            pesquisa.Id,
            pesquisa.Titulo,
            pesquisa.Descricao,
            pesquisa.Gestor?.Id ?? Guid.Empty,
            pesquisa.Status.ToString(),
            pesquisa.Perguntas.Select(p => new PerguntaResponse(p.Texto, p.Opcoes.ToList())).ToList(),
            pesquisa.CreatedAt
        );
    }

    /// <summary>
    /// Cria uma nova pesquisa vinculada a um Gestor.
    /// </summary>
    /// <param name="request">Dados da pesquisa e ID do gestor.</param>
    /// <returns>Retorna a pesquisa criada.</returns>
    /// <response code="201">Pesquisa criada com sucesso.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarPesquisa([FromBody] CriarPesquisaComIdRequest request)
    {
         if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
        try
        {
            var gestor = await _usuarioRepository.GetById(request.GestorId);
            
            if (gestor == null)
            {
                return BadRequest(new { message = "Gestor não encontrado. Crie o usuário antes de criar a pesquisa." });
            }

            var pesquisa = Pesquisa.Criar(request.Titulo, request.Descricao, gestor);
            
            await _pesquisaRepository.Add(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            var response = MapToPesquisaResponse(pesquisa);
            return CreatedAtAction(nameof(GetPesquisaById), new { id = response.Id }, response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar pesquisa: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar pesquisa.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }
    
    /// <summary>
    /// Obtém os detalhes de uma pesquisa pelo ID.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <returns>Detalhes da pesquisa.</returns>
    /// <response code="200">Retorna a pesquisa solicitada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPesquisaById(Guid id)
    {
        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null)
            {
                return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });
            }

            var response = MapToPesquisaResponse(pesquisa);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao buscar pesquisa {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Lista todas as pesquisas cadastradas.
    /// </summary>
    /// <returns>Lista de pesquisas.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllPesquisas()
    {
        try
        {
            var pesquisas = await _pesquisaRepository.GetAll();
            var response = pesquisas.Select(MapToPesquisaResponse).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao listar pesquisas.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Atualiza os dados básicos de uma pesquisa (título e descrição).
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <param name="request">Novos dados da pesquisa.</param>
    /// <returns>Pesquisa atualizada.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePesquisa(Guid id, [FromBody] UpdatePesquisaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            pesquisa.AtualizarDados(request.Titulo, request.Descricao);

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            return Ok(MapToPesquisaResponse(pesquisa));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar pesquisa {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao atualizar pesquisa {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Publica a pesquisa, alterando seu status para Ativa.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <returns>Pesquisa atualizada com novo status.</returns>
    [HttpPost("{id:guid}/publicar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublicarPesquisa(Guid id)
    {
        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            pesquisa.Publicar();

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            return Ok(MapToPesquisaResponse(pesquisa));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao publicar pesquisa {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao publicar pesquisa {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Adiciona uma pergunta à pesquisa.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <param name="request">Dados da pergunta e opções.</param>
    /// <returns>Pesquisa atualizada com a nova pergunta.</returns>
    [HttpPost("{id:guid}/perguntas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarPergunta(Guid id, [FromBody] AdicionarPerguntaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            var pergunta = Pergunta.Criar(request.Texto, request.Opcoes);
            pesquisa.AdicionarPergunta(pergunta);

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            return Ok(MapToPesquisaResponse(pesquisa));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao adicionar pergunta: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao adicionar pergunta.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Registra uma resposta para a pesquisa.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <param name="request">Escolhas selecionadas.</param>
    [HttpPost("{id:guid}/respostas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarResposta(Guid id, [FromBody] AdicionarRespostaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            var resposta = Resposta.Criar(pesquisa, request.Escolhas);
            pesquisa.AdicionarResposta(resposta);

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            return Ok(new { message = "Resposta registrada com sucesso." });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao adicionar resposta: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao adicionar resposta.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Encerra a pesquisa, impedindo novas respostas.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <returns>Pesquisa com status Encerrada.</returns>
    [HttpPost("{id:guid}/encerrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EncerrarPesquisa(Guid id)
    {
        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            pesquisa.Encerrar();

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            return Ok(MapToPesquisaResponse(pesquisa));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao encerrar pesquisa: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao encerrar pesquisa.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Gera o relatório sumarizado da pesquisa.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <param name="request">Lista de IDs de usuários que podem ler o resultado.</param>
    [HttpPost("{id:guid}/resultado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GerarResultado(Guid id, [FromBody] GerarResultadoRequest request)
    {
        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            var leitores = new List<Usuario>();
            if (request.LeitoresIds != null)
            {
                foreach (var leitorId in request.LeitoresIds)
                {
                    var leitor = await _usuarioRepository.GetById(leitorId);
                    if (leitor != null) leitores.Add(leitor);
                }
            }

            pesquisa.GerarResultado(leitores);

            await _pesquisaRepository.Update(pesquisa);
            await _pesquisaRepository.SaveChangesAsync();

            var resultado = pesquisa.ResultadoSumarizado;
            var response = new ResultadoSumarizadoResponse(
                pesquisa.Id,
                resultado!.DataApuracao,
                resultado.ContagemVotos,
                resultado.Leitores.Select(l => l.Nome).ToList()
            );

            return Ok(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao gerar resultado: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao gerar resultado.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }

    /// <summary>
    /// Obtém o resultado sumarizado da pesquisa, se já tiver sido gerado.
    /// </summary>
    /// <param name="id">ID da pesquisa.</param>
    /// <returns>Resultado sumarizado.</returns>
    [HttpGet("{id:guid}/resultado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetResultado(Guid id)
    {
        try
        {
            var pesquisa = await _pesquisaRepository.GetById(id);
            if (pesquisa == null) return NotFound(new { message = $"Pesquisa com ID {id} não encontrada." });

            if (pesquisa.ResultadoSumarizado == null)
            {
                return NotFound(new { message = "O resultado desta pesquisa ainda não foi gerado." });
            }

            var resultado = pesquisa.ResultadoSumarizado;
            var response = new ResultadoSumarizadoResponse(
                pesquisa.Id,
                resultado.DataApuracao,
                resultado.ContagemVotos,
                resultado.Leitores.Select(l => l.Nome).ToList()
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao obter resultado da pesquisa {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }
}

// DTO atualizado para receber o ID do Gestor em vez dos dados de criação
public record CriarPesquisaComIdRequest(string Titulo, string Descricao, Guid GestorId);
public record UpdatePesquisaRequest(string Titulo, string Descricao);
public record AdicionarPerguntaRequest(string Texto, List<string> Opcoes);
public record PesquisaResponse(Guid Id, string Titulo, string Descricao, Guid GestorId, string Status, List<PerguntaResponse> Perguntas, DateTime CreatedAt);
public record PerguntaResponse(string Texto, List<string> Opcoes);
public record AdicionarRespostaRequest(List<string> Escolhas);
public record GerarResultadoRequest(List<Guid> LeitoresIds);
public record ResultadoSumarizadoResponse(Guid PesquisaId, DateTime DataApuracao, IReadOnlyDictionary<string, Dictionary<string, int>> Votos, List<string> Leitores);