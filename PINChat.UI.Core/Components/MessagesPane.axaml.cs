using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class MessagesPane : UserControl
{   
    public static readonly StyledProperty<ObservableCollection<ChatMessageModel>> MessagesProperty =
        AvaloniaProperty.Register<MessagesPane, ObservableCollection<ChatMessageModel>>(nameof(Messages), []);

    public ObservableCollection<ChatMessageModel> Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }
    
    private ObservableCollection<ChatMessageModel>? _currentMessages;
    
    public MessagesPane()
    {
        InitializeComponent();
        
        OnMessagesChanged(Messages);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == MessagesProperty)
        {
            OnMessagesChanged(change.NewValue as ObservableCollection<ChatMessageModel>);
        }
    }
    
    private void OnMessagesChanged(ObservableCollection<ChatMessageModel>? messages)
    {
        // Unsubscribe from previous collection if it exists
        if (_currentMessages != null)
        {
            _currentMessages.CollectionChanged -= OnMessagesCollectionChanged;
        }
        
        // Subscribe to new collection
        if (messages != null)
        {
            messages.CollectionChanged += OnMessagesCollectionChanged;
        }
        
        _currentMessages = messages;
    }
    
    private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // Ensure scrolling happens on the UI thread with a small delay for layout
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (MessageListBox.ItemCount > 0)
                {
                    // Small delay to ensure layout is complete
                    await Task.Delay(10);
                    
                    var scrollViewer = MessageListBox.FindDescendantOfType<ScrollViewer>();
                    if (scrollViewer != null)
                    {
                        scrollViewer.ScrollToEnd();
                    }
                    else
                    {
                        // Fallback to ScrollIntoView if ScrollViewer not found
                        MessageListBox.ScrollIntoView(MessageListBox.ItemCount - 1);
                    }
                }
            });
        }
    }
}