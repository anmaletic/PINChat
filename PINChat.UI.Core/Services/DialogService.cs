using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.Core.Services;

public class DialogService(Func<TopLevel?> topLevel, IViewManager viewManager, ErrorMessageHelper errorMessageHelper) : IDialogService
{
    public async Task<IStorageFile?> ShowFilePicker(string title, params FilePickerFileType[] fileTypes)
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return null;
        
        var files = await topLevelVisual.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        });
        
        return files.Count == 0 ? null : files[0];
    }
    
    public async Task<IStorageFile?> ShowSaveFilePicker(string title, string defaultFileName, string fileExtension, string mimeType)
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return null;
            
        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName,
            ShowOverwritePrompt = true,
            DefaultExtension = fileExtension,
            FileTypeChoices =
            [
                new FilePickerFileType($"{fileExtension.ToUpper()} File")
                {
                    Patterns = [$"*.{fileExtension}"],
                    MimeTypes = [mimeType]
                }
            ]
        };
            
        return await topLevelVisual.StorageProvider.SaveFilePickerAsync(options);
    }

    public async Task ShowErrorDialog(string title, string? message = null)
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return;

        var mainGrid = FindMainViewGrid(topLevelVisual);
        if (mainGrid == null) return;
        
        if (message == null)
        {
            message = await errorMessageHelper.BuildMessage(new Exception("eroror"), "temp");
        }

        var parameters = new Dictionary<string, object>
        {
            { "Title", title },
            { "Message", message }
        };
        
        var tcs = new TaskCompletionSource<bool>();
        
        Action onDialogClosed = () => 
        {
            var overlay = mainGrid.Children.OfType<Border>()
                .FirstOrDefault(c => c.Name == "ErrorDialogOverlay");
            if (overlay != null)
            {
                mainGrid.Children.Remove(overlay);
            }
            tcs.SetResult(true);
        };
        
        var errorDialogView = viewManager.GetView("ErrorDialogViewModel", parameters, onDialogClosed);
        
        var overlay = CreateOverlayHost(errorDialogView);
        
        mainGrid.Children.Add(overlay);
        
        await tcs.Task;
    }

    private Border CreateOverlayHost(UserControl child)
    {
        return new Border
        {
            Name = "ErrorDialogOverlay",
            Child = new Border
            {
                ZIndex = 9999,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = child
            }
        };
    }

    private Grid? FindMainViewGrid(TopLevel topLevelVisual)
    {
        if (topLevelVisual is Window mainWindow && 
            mainWindow.Content is UserControl mainView)
        {
            return mainView.Content as Grid;
        }
        
        if (topLevelVisual.Content is UserControl view)
        {
            return view.Content as Grid;
        }
    
        return null;
    }
}