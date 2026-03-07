using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Messaging.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQueryRequest, ApiResponse<List<GetConversationsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetConversationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<GetConversationsQueryResponse>>> Handle(GetConversationsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<List<GetConversationsQueryResponse>>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var userId = _currentUserService.UserId.Value;

        var conversations = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAllAsync(
                predicate: c => (c.UserId == userId || c.OtherUserId == userId) && !c.IsDeleted,
                orderBy: q => q.OrderByDescending(c => c.UpdatedAt));

        var response = new List<GetConversationsQueryResponse>();

        foreach (var conv in conversations)
        {
            var otherUserId = conv.UserId == userId ? conv.OtherUserId : conv.UserId;

            var messages = await _unitOfWork.GetReadRepository<Message>()
                .GetAllAsync(
                    predicate: m => m.ConversationId == conv.ConversationId && !m.IsDeleted,
                    orderBy: q => q.OrderByDescending(m => m.CreatedAt));

            var lastMessage = messages.FirstOrDefault();

            response.Add(new GetConversationsQueryResponse
            {
                ConversationId = conv.ConversationId,
                OtherUserId = otherUserId,
                LastMessage = lastMessage?.Content,
                LastMessageDate = lastMessage?.CreatedAt
            });
        }

        return ApiResponse<List<GetConversationsQueryResponse>>.SuccessResult(response);
    }
}
