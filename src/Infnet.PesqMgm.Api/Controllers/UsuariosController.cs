using Microsoft.AspNetCore.Mvc;
using Infnet.PesqMgm.Domain.Pesquisas;

namespace Infnet.PesqMgm.Api.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpPost]
    public IActionResult CriarUsuario([FromBody] CriarUsuarioRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Cria o usuário (Gestor) isoladamente
        var usuario = Usuario.Criar(request.Nome, request.Email, request.Perfil);
        
        _usuarioRepository.Adicionar(usuario);
        // _usuarioRepository.Commit(); // Em um caso real, persistiríamos aqui

        _logger.LogInformation("Usuário {Id} criado com sucesso.", usuario.Id);

        return Ok(new { id = usuario.Id, message = "Usuário criado com sucesso" });
    }
}

public record CriarUsuarioRequest(string Nome, string Email, PerfilUsuario Perfil);