using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace PINChat.UI.Core.Services;

public class DialogService(Func<TopLevel?> topLevel)
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
}