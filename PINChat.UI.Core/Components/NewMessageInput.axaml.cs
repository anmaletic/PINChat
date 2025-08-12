using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace PINChat.UI.Core.Components;

public partial class NewMessageInput : UserControl
{
    public static readonly StyledProperty<string> MessageContentProperty = AvaloniaProperty.Register<NewMessageInput, string>(
        nameof(MessageContent), defaultBindingMode:BindingMode.TwoWay);

    public string MessageContent
    {
        get => GetValue(MessageContentProperty);
        set => SetValue(MessageContentProperty, value);
    }

    public static readonly StyledProperty<ICommand?> HandleKeyDownCommandProperty = AvaloniaProperty.Register<NewMessageInput, ICommand?>(
        nameof(HandleKeyDownCommand));

    public ICommand? HandleKeyDownCommand
    {
        get => GetValue(HandleKeyDownCommandProperty);
        set => SetValue(HandleKeyDownCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand?> SelectAndSendImageCommandProperty = AvaloniaProperty.Register<NewMessageInput, ICommand?>(
        nameof(SelectAndSendImageCommand));

    public ICommand? SelectAndSendImageCommand
    {
        get => GetValue(SelectAndSendImageCommandProperty);
        set => SetValue(SelectAndSendImageCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand?> SendMessageCommandProperty = AvaloniaProperty.Register<NewMessageInput, ICommand?>(
        nameof(SendMessageCommand));

    public ICommand? SendMessageCommand
    {
        get => GetValue(SendMessageCommandProperty);
        set => SetValue(SendMessageCommandProperty, value);
    }
    
    public NewMessageInput()
    {
        InitializeComponent();
    }
}