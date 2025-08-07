using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PINChat.UI.Core.Components;

public partial class LoadingControl : UserControl
{
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<LoadingControl, bool>(nameof(IsLoading));

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public LoadingControl()
    {
        InitializeComponent();
    }
}