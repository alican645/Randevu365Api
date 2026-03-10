using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommandRequest, ApiResponse<SendMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<SendMessageCommandResponse>> Handle(SendMessageCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<SendMessageCommandResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var senderId = _currentUserService.UserId.Value;

        // Conversation'ı AppointmentId ile bul
        var conversation = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAsync(c => c.AppointmentId == request.AppointmentId && !c.IsDeleted);

        if (conversation == null)
            return ApiResponse<SendMessageCommandResponse>.NotFoundResult("Konuşma bulunamadı.");

        if (conversation.IsClosed)
            return ApiResponse<SendMessageCommandResponse>.FailResult("Bu konuşma kapatılmıştır. Mesaj gönderilemez.");

        // Kullanıcının bu konuşmaya erişimi var mı kontrol et
        if (conversation.UserId != senderId && conversation.OtherUserId != senderId)
            return ApiResponse<SendMessageCommandResponse>.ForbiddenResult("Bu konuşmaya erişiminiz yok.");

        var receiverId = conversation.UserId == senderId ? conversation.OtherUserId : conversation.UserId;

        var message = new Message
        {
            Content = request.Content,
            ConversationId = conversation.ConversationId,
            SenderId = senderId,
            ReceiverId = receiverId
        };

        await _unitOfWork.GetWriteRepository<Message>().AddAsync(message);
        await _unitOfWork.SaveAsync();

        return ApiResponse<SendMessageCommandResponse>.CreatedResult(
            new SendMessageCommandResponse { MessageId = message.Id, ConversationId = conversation.ConversationId },
            "Mesaj gonderildi.");
    }
}
