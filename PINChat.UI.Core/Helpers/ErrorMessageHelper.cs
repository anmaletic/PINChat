using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using PINChat.UI.Core.Models;
using PINChat.UI.Core.Templates;

namespace PINChat.UI.Core.Helpers;

public class ErrorMessageHelper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;
    
    public ErrorMessageHelper(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
        
    }
    public async Task<string> BuildMessage(Exception ex, string message)
    {
        try
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);
            
            var model = new ErrorMessageModel()
            {
                Username = "admin (" + Environment.UserName + ")",
                Hostname = Environment.MachineName,
                Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                ErrorMessage = ex,
                Message = message
            };
            
            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var dictionary = new Dictionary<string, object?> { { "Model", model } };
                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<ErrorTemplate>(parameters);
                return output.ToHtmlString();
            });
            
            return html;
        }
        catch (Exception e)
        {
            // _loggerFactory.LogError(ex, "Error generating message");
            return $"Error generating message: {ex.Message}";
        }
    }
    
}