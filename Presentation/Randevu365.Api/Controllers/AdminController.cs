using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Randevu365.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _mediator.Send(new GetAllUsersRequest());
        return Ok(response);
    }

}