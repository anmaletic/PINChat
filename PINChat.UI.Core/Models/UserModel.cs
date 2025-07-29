using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using PINChat.UI.Core.Common;

namespace PINChat.UI.Core.Models;

public class UserModel
{
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public byte[]? Avatar { get; set; }
    public string? AvatarPath { get; set; }
    public ObservableCollection<UserModel> Contacts { get; set; } = [];
    public ObservableCollection<ChatMessageModel> Messages { get; set; } = [];

    public Bitmap? AvatarBitmap
    {
        get
        {
            if (Avatar is null)
            {
                var initials = $"{FirstName![0]}{LastName![0]}";
                return InitialsImageGenerator.CreateInitialsBitmap(initials);
            }
            
            using var memory = new MemoryStream(Avatar);
            return new Bitmap(memory);
        }
    }
}