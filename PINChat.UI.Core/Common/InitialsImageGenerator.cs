using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace PINChat.UI.Core.Common;

public static class InitialsImageGenerator
{
    public static Bitmap CreateInitialsBitmap(
        string initials,
        int size = 200,
        Color? backgroundColor = null,
        Color? textColor = null,
        string fontFamily = "Arial",
        double? fontSize = null)
    {
        // Set default colors if not provided
        backgroundColor ??= Colors.Transparent;
        textColor ??= Colors.CadetBlue;

        // Calculate font size if not provided (roughly 40% of the image size)
        fontSize ??= size * 0.4;

        // Create a render target bitmap
        var pixelSize = new PixelSize(size, size);
        var dpi = new Vector(96, 96);

        using var renderTargetBitmap = new RenderTargetBitmap(pixelSize, dpi);

        using (var drawingContext = renderTargetBitmap.CreateDrawingContext())
        {
            // Fill background
            var backgroundBrush = new SolidColorBrush(backgroundColor.Value);
            drawingContext.FillRectangle(backgroundBrush, new Rect(0, 0, size, size));

            // Create text formatting
            var typeface = new Typeface(fontFamily, FontStyle.Normal, FontWeight.Bold);
            var textBrush = new SolidColorBrush(textColor.Value);

            // Create formatted text
            var formattedText = new FormattedText(
                initials.ToUpper(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize.Value,
                textBrush);

            // Calculate position to center the text
            var textWidth = formattedText.Width;
            var textHeight = formattedText.Height;
            var x = (size - textWidth) / 2;
            var y = (size - textHeight) / 2;

            // Draw the text
            drawingContext.DrawText(formattedText, new Point(x, y));
        }

        // Convert to regular Bitmap
        using var stream = new MemoryStream();
        renderTargetBitmap.Save(stream);
        stream.Position = 0;
        return new Bitmap(stream);
    }
}