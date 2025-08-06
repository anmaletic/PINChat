using Avalonia.Data.Converters;
using PINChat.Core.Domain.Enums;

namespace PINChat.UI.Core.Converters;

public static class MessageTypeToVisibilityConverter
{
    public static readonly IValueConverter Text = new FuncValueConverter<MessageType, bool >(
        messageType => messageType == MessageType.Text);

    public static readonly IValueConverter Image = new FuncValueConverter<MessageType, bool>(
        messageType => messageType == MessageType.Image);
}