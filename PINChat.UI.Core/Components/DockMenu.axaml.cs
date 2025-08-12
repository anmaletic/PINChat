using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PINChat.Core.Domain.Enums;

namespace PINChat.UI.Core.Components;

public partial class DockMenu : UserControl
{
    public static readonly StyledProperty<Bitmap> AvatarProperty =
        AvaloniaProperty.Register<DockMenu, Bitmap>(nameof(Avatar));

    public Bitmap Avatar
    {
        get => GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> SelectedItemCommandProperty =
        AvaloniaProperty.Register<DockMenu, ICommand?>(nameof(SelectedItemCommand));

    public ICommand? SelectedItemCommand
    {
        get => GetValue(SelectedItemCommandProperty);
        set => SetValue(SelectedItemCommandProperty, value);
    }
    
    
    public DockMenu()
    {
        InitializeComponent();
    }
}