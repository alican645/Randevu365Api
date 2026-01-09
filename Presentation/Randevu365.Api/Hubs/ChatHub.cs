using Microsoft.AspNetCore.SignalR;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IHubService _hubService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConversationIdHelper _conversationIdHelper;

    public ChatHub(IHubService hubService, ICurrentUserService currentUserService, IConversationIdHelper conversationIdHelper)
    {
        _hubService = hubService;
        _currentUserService = currentUserService;
        _conversationIdHelper = conversationIdHelper;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int receiverId, string message)
    {
        var conversationId = await _conversationIdHelper.GenerateConversationId(_currentUserService.UserId ?? 0, receiverId);

        await _hubService.SaveMessage(
            _currentUserService.UserId ?? 0,
            receiverId,
            message,
            conversationId
        );

        await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
    }
}

