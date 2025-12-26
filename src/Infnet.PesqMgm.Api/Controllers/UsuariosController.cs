using Microsoft.AspNetCore.Mvc;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Api.Dtos;
using Infnet.PesqMgm.Infrastructure.Data.Repositories;

namespace Infnet.PesqMgm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly ILogger<UsuariosController> _logger;
    // Injeção do repositório (interface definida no outro arquivo para este exemplo)
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(ILogger<UsuariosController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    /// <summary>
    /// Cria um novo usuário (Gestor).
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado.</param>
    /// <returns>Retorna o ID do usuário criado.</returns>
    [HttpPost]
    public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Cria o usuário (Gestor) isoladamente
        var usuario = Usuario.Criar(request.Nome, request.Email, request.Perfil);
        
        await _usuarioRepository.Add(usuario);
        await _usuarioRepository.SaveChangesAsync();

        _logger.LogInformation("Usuário {Id} criado com sucesso.", usuario.Id);

        return Ok(new { id = usuario.Id, message = "Usuário criado com sucesso" });
    }

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsuarios()
    {
        try
        {
            var usuarios = await _usuarioRepository.GetAll();
            var response = usuarios.Select(u => new { u.Id, u.Nome, u.Email, u.Perfil });
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar usuários.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
        }
    }
}