using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.Core.Services;

public class DialogService(Func<TopLevel?> topLevel, IViewManager viewManager, ErrorMessageHelper errorMessageHelper)
    : IDialogService
{
    private Border? _currentOverlay;
    private Grid? _mainGrid;
    
    private List<string> _pendingErrors = [];

    private bool _isDialogVisible = false;
    private TaskCompletionSource<bool>? _currentTcs;

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

    public async Task<IStorageFile?> ShowSaveFilePicker(string title, string defaultFileName, string fileExtension,
        string mimeType)
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

    public async Task ShowErrorDialog(string title, string message)
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return;

        _mainGrid = FindMainViewGrid(topLevelVisual);
        if (_mainGrid == null) return;

        _pendingErrors.Add(message);
        
        if (_isDialogVisible)
        {
            UpdateExistingDialog();
        }
        else
        {
            await ShowNewDialog();
        }
    }

    private void UpdateExistingDialog()
    {
        if (_currentOverlay?.Child is not UserControl dialogView)
        {
            return;
        }

        var viewModel = dialogView.DataContext as IUpdateable;
        viewModel?.Update(_pendingErrors.ToList());
    }

    private async Task ShowNewDialog()
    {
        _isDialogVisible = true;
        _currentTcs = new TaskCompletionSource<bool>();

        var parameters = new Dictionary<string, object>
        {
            { "Errors", _pendingErrors.ToList() }
        };

        Action onDialogClosed = () => { CloseDialog(); };

        var errorDialogView = viewManager.GetView("ErrorDialogViewModel", parameters, onDialogClosed);
        _currentOverlay = CreateOverlayHost(errorDialogView);
        _mainGrid!.Children.Add(_currentOverlay);

        await _currentTcs.Task;
    }
    
    private void CloseDialog()
    {
        if (_currentOverlay != null && _mainGrid != null)
        {
            _mainGrid.Children.Remove(_currentOverlay);
            _currentOverlay = null;
        }

        _isDialogVisible = false;
        _pendingErrors.Clear();
        _currentTcs?.SetResult(true);
        _currentTcs = null;
        
    }
    private Border CreateOverlayHost(UserControl child)
    {
        return new Border
        {
            Name = "ErrorDialogOverlay",
            ZIndex = 9999,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Child = child
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