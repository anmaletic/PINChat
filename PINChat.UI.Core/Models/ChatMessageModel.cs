using CommunityToolkit.Mvvm.ComponentModel;

namespace PINChat.UI.Core.Models;

public partial class ChatMessageModel : ObservableObject
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Sender { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsSent { get; set; } = false;
    
    [ObservableProperty]
    private bool _isReceived = false;
    
    [ObservableProperty]
    private bool _isRead = false;

    [ObservableProperty]
    private bool _isOrigin;
}