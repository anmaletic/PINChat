using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PINChat.UI.Core.Common;
using PINChat.UI.Core.Extensions;

namespace PINChat.UI.Core.Models;

public partial class UserModel : ObservableObject
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public byte[]? Avatar { get; set; }
    public string? AvatarPath { get; set; }

    [ObservableProperty]
    private bool _isTyping;
    
    public ObservableCollection<UserModel> Contacts { get; set; } = [];
    public SmartObservableCollection<ChatMessageModel> Messages { get; set; } = [];

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