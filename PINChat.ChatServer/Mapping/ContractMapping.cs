using PINChat.Contracts.Responses;
using PINChat.Persistence.Db.Entities;

namespace PINChat.ChatServer.Mapping;

public static class ContractMapping
{
    public static MessageResponse ToResponse(this Message message)
    {
        return new MessageResponse()
        {
            Id = message.Id,
            Timestamp = message.Timestamp,
            SenderId = message.SenderId,
            RecipientId = message.RecipientId,
            Content = message.Content,
            MessageType = message.MessageType,
            ImagePath = message.ImagePath,
            IsSent = message.IsSent,
            IsReceived = message.IsReceived,
            IsRead = message.IsRead
        };
    }
}