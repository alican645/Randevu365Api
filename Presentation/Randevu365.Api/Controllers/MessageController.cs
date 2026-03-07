using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.Messaging.Commands.SendMessage;
using Randevu365.Application.Features.Messaging.Queries.GetConversations;
using Randevu365.Application.Features.Messaging.Queries.GetMessagesByConversation;

namespace Randevu365.Api.Controllers;

[Route("api/messages")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var response = await _mediator.Send(new GetConversationsQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("conversation/{conversationId}")]
    public async Task<IActionResult> GetMessages(string conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var response = await _mediator.Send(new GetMessagesByConversationQueryRequest
        {
            ConversationId = conversationId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}
