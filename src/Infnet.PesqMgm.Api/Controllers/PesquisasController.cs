using Microsoft.AspNetCore.Mvc;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Api.Dtos;
using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Domain.Shared;

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
            var gestor = await _usuarioRepository.ObterPorIdAsync(request.GestorId);
            
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
}

// DTO atualizado para receber o ID do Gestor em vez dos dados de criação
public record CriarPesquisaComIdRequest(string Titulo, string Descricao, Guid GestorId);
public record UpdatePesquisaRequest(string Titulo, string Descricao);
public record AdicionarPerguntaRequest(string Texto, List<string> Opcoes);
public record PesquisaResponse(Guid Id, string Titulo, string Descricao, Guid GestorId, string Status, List<PerguntaResponse> Perguntas, DateTime CreatedAt);
public record PerguntaResponse(string Texto, List<string> Opcoes);

// Interface do Repositório (Deveria estar na camada de Domain, definida aqui para compilação do exemplo)
public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    void Adicionar(Usuario usuario);
}