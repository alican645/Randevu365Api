using MediatR;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.Login;
using Randevu365.Application.Features.Register;

namespace Randevu365.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Kullanıcı girişi yapar ve JWT token döner
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Yeni kullanıcı kaydı oluşturur
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _mediator.Send(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}
