using CommunityToolkit.Mvvm.ComponentModel;

namespace PINChat.UI.Core.Models;

public partial class ChatMessageModel : ObservableObject
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Sender { get; set; } = "";
    public string Content { get; set; } = "";
    
    [ObservableProperty]
    private bool _isSent;
    
    [ObservableProperty]
    private bool _isReceived;
    
    [ObservableProperty]
    private bool _isRead;

    public bool IsOrigin { get; set; } 
}