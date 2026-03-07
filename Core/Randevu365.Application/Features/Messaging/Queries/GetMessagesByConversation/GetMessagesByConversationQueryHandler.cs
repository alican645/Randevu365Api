using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Messaging.Queries.GetMessagesByConversation;

public class GetMessagesByConversationQueryHandler : IRequestHandler<GetMessagesByConversationQueryRequest, ApiResponse<List<GetMessagesByConversationQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMessagesByConversationQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<GetMessagesByConversationQueryResponse>>> Handle(GetMessagesByConversationQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<List<GetMessagesByConversationQueryResponse>>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var userId = _currentUserService.UserId.Value;

        // Verify user is part of this conversation
        var conversation = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAsync(c => c.ConversationId == request.ConversationId
                && (c.UserId == userId || c.OtherUserId == userId)
                && !c.IsDeleted);

        if (conversation == null)
            return ApiResponse<List<GetMessagesByConversationQueryResponse>>.NotFoundResult("Konusma bulunamadi.");

        var messages = await _unitOfWork.GetReadRepository<Message>()
            .GetAllByPagingAsync(
                predicate: m => m.ConversationId == request.ConversationId && !m.IsDeleted,
                orderBy: q => q.OrderByDescending(m => m.CreatedAt),
                currentPage: request.PageNumber,
                pageSize: request.PageSize);

        var response = messages.Select(m => new GetMessagesByConversationQueryResponse
        {
            Id = m.Id,
            Content = m.Content,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            SentAt = m.CreatedAt
        }).ToList();

        return ApiResponse<List<GetMessagesByConversationQueryResponse>>.SuccessResult(response);
    }
}
