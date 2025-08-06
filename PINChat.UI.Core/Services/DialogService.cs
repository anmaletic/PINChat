using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace PINChat.UI.Core.Services;

public class DialogService(Func<TopLevel?> topLevel)
{
    public async Task<IStorageFile?> ShowFilePicker()
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return null;
        
        var files = await topLevelVisual.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                FilePickerFileTypes.ImageAll
            }
        });

        
        return files.Count == 0 ? null : files[0];
    }
}