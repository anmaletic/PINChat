using System.Collections.ObjectModel;
using PINChat.Contracts.Responses;

namespace PINChat.UI.Core.Models;

public static class Mapping
{
    public static UserModel ToModel(this LoginResponse user)
    {
        return new UserModel()
        {
            Token = user.Token ?? string.Empty,
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = user.Avatar,
            AvatarPath = user.AvatarPath,
            Contacts = new ObservableCollection<UserModel>(
                user.Contacts.Select(c => new UserModel
            {
                UserId = c.UserId,
                UserName = c.UserName,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Avatar = c.Avatar,
                AvatarPath = c.AvatarPath
            }))
        };
    }
}