using Microsoft.AspNetCore.SignalR;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IHubService _hubService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(IHubService hubService, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _hubService = hubService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int appointmentId, string message)
    {
        var senderId = _currentUserService.UserId ?? 0;
        if (senderId == 0)
        {
            await Clients.Caller.SendAsync("Error", "Kullanıcı kimliği bulunamadı.");
            return;
        }

        var conversation = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAsync(c => c.AppointmentId == appointmentId && !c.IsDeleted);

        if (conversation == null)
        {
            await Clients.Caller.SendAsync("Error", "Konuşma bulunamadı.");
            return;
        }

        if (conversation.IsClosed)
        {
            await Clients.Caller.SendAsync("Error", "Bu konuşma kapatılmıştır.");
            return;
        }

        if (conversation.UserId != senderId && conversation.OtherUserId != senderId)
        {
            await Clients.Caller.SendAsync("Error", "Bu konuşmaya erişiminiz yok.");
            return;
        }

        var receiverId = conversation.UserId == senderId ? conversation.OtherUserId : conversation.UserId;

        await _hubService.SaveMessage(senderId, receiverId, message, conversation.ConversationId);
        await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
    }
}
