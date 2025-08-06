using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PINChat.Core.Domain.Enums;
using PINChat.UI.Core.Helpers;

namespace PINChat.UI.Core.Models;

public partial class ChatMessageModel : ObservableObject
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Sender { get; set; } = "";
    public string Content { get; set; } = "";
    
    [ObservableProperty]
    private MessageType _messageType = MessageType.Text;

    [ObservableProperty]
    private string? _imagePath;
    
    [ObservableProperty]
    private bool _isSent;
    
    [ObservableProperty]
    private bool _isReceived;
    
    [ObservableProperty]
    private bool _isRead;
    
    public Task<Bitmap?> Image => 
        ImagePath is not null 
            ? ImageHelper.LoadFromWeb(new Uri(ImagePath)) 
            : Task.FromResult<Bitmap>(null);

    public bool IsOrigin { get; set; } 
}