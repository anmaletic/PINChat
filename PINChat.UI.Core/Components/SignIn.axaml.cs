using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class SignIn : UserControl
{
    public static readonly StyledProperty<LoginUserModel> UserProperty =
        AvaloniaProperty.Register<SignIn, LoginUserModel>(nameof(User), new LoginUserModel());
    
    public LoginUserModel User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }
    
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<SignIn, string>(nameof(Message), string.Empty);
    
    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<SignIn, bool>(nameof(IsLoading), false);
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> LoginCommandProperty =
        AvaloniaProperty.Register<SignIn, ICommand>(
            nameof(LoginCommand));
    
    public ICommand LoginCommand
    {
        get => GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }
    
    public SignIn()
    {
        InitializeComponent();
    }
}