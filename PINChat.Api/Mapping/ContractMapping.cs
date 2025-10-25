namespace PINChat.Api.Mapping;

public static class ContractMapping
{
    public static LoginResponse ToResponse(this ApplicationUser user)
    {
        return new LoginResponse()
        {
            UserId = user.Id,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = user.Avatar ?? [],
            AvatarPath = user.AvatarPath,
            Message = string.Empty,
            Contacts = user.MyContacts.Select(c => new LoginResponse()
            {
                UserId = c.ContactUser.Id,
                UserName = c.ContactUser.UserName!,
                FirstName = c.ContactUser.FirstName,
                LastName = c.ContactUser.LastName,
                Avatar = c.ContactUser.Avatar ?? [],
                AvatarPath = c.ContactUser.AvatarPath,
                Message = string.Empty
            }),
            AddedByOthers = user.AddedByOthers.Select(c => new LoginResponse()
            {
                UserId = c.ContactUser.Id,
                UserName = c.ContactUser.UserName!,
                FirstName = c.ContactUser.FirstName,
                LastName = c.ContactUser.LastName,
                Avatar = c.ContactUser.Avatar ?? [],
                AvatarPath = c.ContactUser.AvatarPath,
                Message = string.Empty
            }),
        };
    }

    public static UserResponse ToUserResponse(this ApplicationUser user)
    {
        return new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = user.Avatar
        };
    }

    public static IEnumerable<UserResponse> ToResponse(this IEnumerable<ApplicationUser> users)
    {
        return users.Select(u => u.ToUserResponse());
    }

    public static MessageResponse ToResponse(this Message msg)
    {
        return new MessageResponse()
        {
            Id = msg.Id,
            Timestamp = msg.Timestamp,
            SenderId = msg.SenderId,
            RecipientId = msg.RecipientId,
            ImagePath = msg.ImagePath,
            MessageType = msg.MessageType,
            Content = msg.Content,
            IsSent = msg.IsSent,
            IsReceived = msg.IsReceived,
            IsRead = msg.IsRead
        };
    }
    
    public static IEnumerable<MessageResponse> ToResponse(this IEnumerable<Message> messages)
    {
        return messages.Select(m => m.ToResponse());
    }
}