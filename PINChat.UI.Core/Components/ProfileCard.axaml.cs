using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class ProfileCard : UserControl
{
    public static readonly StyledProperty<bool> IsEditableProperty = AvaloniaProperty.Register<ProfileCard, bool>(
        nameof(IsEditable));

    public bool IsEditable
    {
        get => GetValue(IsEditableProperty);
        set => SetValue(IsEditableProperty, value);
    }

    public static readonly StyledProperty<UserModel> UserProperty = AvaloniaProperty.Register<ProfileCard, UserModel>(
        nameof(User));

    public UserModel User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public static readonly StyledProperty<ICommand?> ChangeAvatarCommandProperty = AvaloniaProperty.Register<ProfileCard, ICommand?>(
        nameof(ChangeAvatarCommand));

    public ICommand? ChangeAvatarCommand
    {
        get => GetValue(ChangeAvatarCommandProperty);
        set => SetValue(ChangeAvatarCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand?> UpdateCommandProperty = AvaloniaProperty.Register<ProfileCard, ICommand?>(
        nameof(UpdateCommand));

    public ICommand? UpdateCommand
    {
        get => GetValue(UpdateCommandProperty);
        set => SetValue(UpdateCommandProperty, value);
    }
    
    public ProfileCard()
    {
        InitializeComponent();
    }
}