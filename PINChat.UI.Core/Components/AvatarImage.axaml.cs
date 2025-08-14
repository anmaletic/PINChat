using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PINChat.UI.Core.Components;

public partial class AvatarImage : UserControl
{
    public static readonly StyledProperty<IImage?> AvatarSourceProperty = AvaloniaProperty.Register<AvatarImage, IImage?>(
        nameof(AvatarSource));

    public IImage? AvatarSource
    {
        get => GetValue(AvatarSourceProperty);
        set => SetValue(AvatarSourceProperty, value);
    }

    public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<AvatarImage, double>(
        nameof(Size));

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly StyledProperty<double> RadiusProperty = AvaloniaProperty.Register<AvatarImage, double>(
        nameof(Radius));

    public double Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public static readonly StyledProperty<Rect> RectProperty = AvaloniaProperty.Register<AvatarImage, Rect>(
        nameof(Rect));

    public Rect Rect
    {
        get => GetValue(RectProperty);
        set => SetValue(RectProperty, value);
    }

    public AvatarImage()
    {
        InitializeComponent();
    }
}
