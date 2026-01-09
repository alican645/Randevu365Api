
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Services;


public class HubService : IHubService
{
    private readonly IUnitOfWork _unitOfWork;
    public HubService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SaveMessage(int senderId, int receiverId, string message, string conversationId)
    {
        await _unitOfWork.GetWriteRepository<Message>().AddAsync(new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = message,
            ConversationId = conversationId
        });
        await _unitOfWork.SaveAsync();
    }
}
