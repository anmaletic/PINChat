using Avalonia.Platform.Storage;

namespace PINChat.UI.Core.Interfaces;

public interface IDialogService
{
    Task<IStorageFile?> ShowFilePicker(string title, params FilePickerFileType[] fileTypes);
    Task<IStorageFile?> ShowSaveFilePicker(string title, string defaultFileName, string fileExtension, string mimeType);
    Task ShowErrorDialog(string title, string message);
}