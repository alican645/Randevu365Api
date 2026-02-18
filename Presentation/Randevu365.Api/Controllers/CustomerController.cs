using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.BusinessComments.Commands.AddBusinessComment;
using Randevu365.Application.Features.BusinessComments.Commands.DeleteBusinessComment;
using Randevu365.Application.Features.BusinessComments.Commands.UpdateBusinessComment;
using Randevu365.Application.Features.BusinessComments.Queries.GetCommentByCommentId;
using Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;
using Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByUserId;
using Randevu365.Application.Features.BusinessRating.Commands.AddBusinessRating;
using Randevu365.Application.Features.BusinessRating.Commands.DeleteBusinessRating;
using Randevu365.Application.Features.BusinessRating.Commands.UpdateBusinessRating;
using Randevu365.Application.Features.BusinessRating.Queries.GetMyRatings;
using Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessBasicInfoByCustomerOwnerId;
using Randevu365.Application.Features.BusinessRating.Queries.GetRatingsByBusinessId;
using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[Route("api/customer")]
[ApiController]
[Authorize(Roles = Roles.Customer)]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }



    #region Business Comments
    [HttpGet("comments/{businessId}")]
    public async Task<IActionResult> GetCommentsByBusinessId(int businessId)
    {
        var response = await _mediator.Send(new GetCommentsByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("comment/{commentId}")]
    public async Task<IActionResult> GetCommentById(int commentId)
    {
        var response = await _mediator.Send(new GetCommentByCommentIdQueryRequest { CommentId = commentId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("mycomments")]
    public async Task<IActionResult> GetMyComments()
    {
        var response = await _mediator.Send(new GetCommentsByUserIdQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("comment/add")]
    public async Task<IActionResult> AddComment(AddBusinessCommentCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("comment/update")]
    public async Task<IActionResult> UpdateComment(UpdateBusinessCommentCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("comment/delete")]
    public async Task<IActionResult> DeleteComment(DeleteBusinessCommentCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    #endregion

    #region Business Ratings

    [HttpGet("ratings/{businessId}")]
    public async Task<IActionResult> GetRatingsByBusinessId(int businessId)
    {
        var response = await _mediator.Send(new GetRatingsByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("myratings")]
    public async Task<IActionResult> GetMyRatings()
    {
        var response = await _mediator.Send(new GetMyRatingsQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("rating/add")]
    public async Task<IActionResult> AddRating(AddBusinessRatingCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("rating/update")]
    public async Task<IActionResult> UpdateRating(UpdateBusinessRatingCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("rating/delete")]
    public async Task<IActionResult> DeleteRating(DeleteBusinessRatingCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion
}
