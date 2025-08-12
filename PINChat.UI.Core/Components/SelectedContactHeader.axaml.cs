using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class SelectedContactHeader : UserControl
{
    public static readonly StyledProperty<UserModel> SelectedContactProperty =
        AvaloniaProperty.Register<SelectedContactHeader, UserModel>(
            nameof(SelectedContact));

    public UserModel SelectedContact
    {
        get => GetValue(SelectedContactProperty);
        set => SetValue(SelectedContactProperty, value);
    }

    public static readonly StyledProperty<ICommand?> ExportMessagesCommandProperty =
        AvaloniaProperty.Register<SelectedContactHeader, ICommand?>(
            nameof(ExportMessagesCommand));

    public ICommand? ExportMessagesCommand
    {
        get => GetValue(ExportMessagesCommandProperty);
        set => SetValue(ExportMessagesCommandProperty, value);
    }

    public SelectedContactHeader()
    {
        InitializeComponent();
    }
}