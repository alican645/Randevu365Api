using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommandRequest, ApiResponse<SendMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConversationIdHelper _conversationIdHelper;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IConversationIdHelper conversationIdHelper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _conversationIdHelper = conversationIdHelper;
    }

    public async Task<ApiResponse<SendMessageCommandResponse>> Handle(SendMessageCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<SendMessageCommandResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var senderId = _currentUserService.UserId.Value;
        var conversationId = await _conversationIdHelper.GenerateConversationId(senderId, request.ReceiverId);

        // Ensure conversation exists
        var conversation = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAsync(c => c.ConversationId == conversationId && !c.IsDeleted);

        if (conversation == null)
        {
            var newConversation = new Conversation
            {
                UserId = senderId,
                OtherUserId = request.ReceiverId,
                ConversationId = conversationId
            };
            await _unitOfWork.GetWriteRepository<Conversation>().AddAsync(newConversation);
        }

        var message = new Message
        {
            Content = request.Content,
            ConversationId = conversationId,
            SenderId = senderId,
            ReceiverId = request.ReceiverId
        };

        await _unitOfWork.GetWriteRepository<Message>().AddAsync(message);
        await _unitOfWork.SaveAsync();

        return ApiResponse<SendMessageCommandResponse>.CreatedResult(
            new SendMessageCommandResponse { MessageId = message.Id, ConversationId = conversationId },
            "Mesaj gonderildi.");
    }
}
