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
    
    [ObservableProperty]
    private string _content = "";
    
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

    partial void OnImagePathChanged(string? value)
    {
        if (value is null)
        {
            return;
        }
        
        Image = ImageHelper.LoadFromWeb(new Uri(value));
    }

    [ObservableProperty]
    public Task<Bitmap?> _image;

    public bool IsOrigin { get; set; } 
}