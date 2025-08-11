using System.Globalization;

namespace PINChat.UI.Core.Models;

public class ErrorMessageModel
{
    public string Username { get; set; } = Environment.UserName;
    public string Hostname { get; set; } = Environment.MachineName;
    public string Date { get; set; } = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    public Exception ErrorMessage { get; set; } = new Exception();
    public string Message { get; set; } = string.Empty;
}