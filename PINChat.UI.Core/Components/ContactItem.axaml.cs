using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PINChat.UI.Core.Enums;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class ContactItem : UserControl
{
    public static readonly StyledProperty<ContactItemType> TypeProperty = AvaloniaProperty.Register<ContactItem, ContactItemType>(
        nameof(Type));

    public ContactItemType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public static readonly StyledProperty<UserModel> UserProperty = AvaloniaProperty.Register<ContactItem, UserModel>(
        nameof(User));

    public UserModel User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public static readonly StyledProperty<ICommand?> SelectedContactCommandProperty = AvaloniaProperty.Register<ContactItem, ICommand?>(
        nameof(SelectedContactCommand));

    public ICommand? SelectedContactCommand
    {
        get => GetValue(SelectedContactCommandProperty);
        set => SetValue(SelectedContactCommandProperty, value);
    }
    
    public ContactItem()
    {
        InitializeComponent();
    }
}