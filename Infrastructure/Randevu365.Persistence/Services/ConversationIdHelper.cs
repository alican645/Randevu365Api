
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Services;

public class ConversationIdHelper : IConversationIdHelper
{
    private readonly IUnitOfWork _unitOfWork;
    public ConversationIdHelper(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateConversationId(int senderId, int receiverId)
    {
        var conversation = await _unitOfWork.GetReadRepository<Conversation>()
            .GetAsync(x => (x.UserId == senderId && x.OtherUserId == receiverId) || (x.UserId == receiverId && x.OtherUserId == senderId));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                UserId = senderId,
                OtherUserId = receiverId,
                ConversationId = $"{senderId}_{receiverId}"
            };
            await _unitOfWork.GetWriteRepository<Conversation>().AddAsync(conversation);
            await _unitOfWork.SaveAsync();
        }

        return conversation.ConversationId;
    }
}
