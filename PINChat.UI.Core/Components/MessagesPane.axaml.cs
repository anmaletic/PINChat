using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class MessagesPane : UserControl
{   
    public static readonly StyledProperty<ObservableCollection<ChatMessageModel>> MessagesProperty =
        AvaloniaProperty.Register<ContactsPane, ObservableCollection<ChatMessageModel>>(nameof(Messages), []);

    public ObservableCollection<ChatMessageModel> Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }
    
    public MessagesPane()
    {
        InitializeComponent();
    }
}