using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Messaging.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQueryRequest, ApiResponse<GetConversationsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetConversationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetConversationsQueryResponse>> Handle(GetConversationsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<GetConversationsQueryResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var userId = _currentUserService.UserId.Value;

        var conversations = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAllAsync(
                predicate: c => (c.UserId == userId || c.OtherUserId == userId) && !c.IsDeleted && !c.IsClosed,
                orderBy: q => q.OrderByDescending(c => c.UpdatedAt));

        var responseItems = new List<GetConversationsQueryResponseItem>();

        foreach (var conv in conversations)
        {
            var otherUserId = conv.UserId == userId ? conv.OtherUserId : conv.UserId;

            var messages = await _unitOfWork.GetReadRepository<Message>()
                .GetAllAsync(
                    predicate: m => m.ConversationId == conv.ConversationId && !m.IsDeleted,
                    orderBy: q => q.OrderByDescending(m => m.CreatedAt));

            var lastMessage = messages.FirstOrDefault();

            responseItems.Add(new GetConversationsQueryResponseItem
            {
                ConversationId = conv.ConversationId,
                OtherUserId = otherUserId,
                AppointmentId = conv.AppointmentId,
                LastMessage = lastMessage?.Content,
                LastMessageDate = lastMessage?.CreatedAt
            });
        }

        var response = new GetConversationsQueryResponse
        {
            Items = responseItems
        };
        return ApiResponse<GetConversationsQueryResponse>.SuccessResult(response);
    }
}
