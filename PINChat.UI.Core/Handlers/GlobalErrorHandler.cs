using Avalonia.Threading;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Interfaces;

namespace PINChat.UI.Core.Handlers;

public class GlobalErrorHandler
{
    private readonly ErrorMessageHelper _errorMessageHelper;
    private readonly IDialogService _dialogService;

    public GlobalErrorHandler(ErrorMessageHelper errorMessageHelper, IDialogService dialogService)
    {
        _errorMessageHelper = errorMessageHelper;
        _dialogService = dialogService;
    }
    
    public void AddHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        TaskScheduler.UnobservedTaskException += UnobservedTaskException;
        Dispatcher.UIThread.UnhandledException += DispatcherUnhandledException;
    }
    
    private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            HandleException(ex, "Unhandled Exception (Non-UI Thread)");
        }
    }
    
    private void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HandleException(e.Exception, "Unhandled Exception (UI Thread)");
        e.Handled = true; // Prevents the app from crashing
    }
    
    private void UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        HandleException(e.Exception, "Unobserved Task Exception");
        e.SetObserved(); // Prevents the process from being terminated
    }
    
    private void HandleException(Exception ex, string context)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            var message = await _errorMessageHelper.BuildMessage(ex, context);
            
            await _dialogService.ShowErrorDialog("Error", message);
        });
    }
}