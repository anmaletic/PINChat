using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using PINChat.Api.Sdk;
using PINChat.Contracts.Requests;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class ProfileViewModel : LoadableViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IChatApi _chatApi;

    [ObservableProperty]
    private UserModel _loggedInUser;

    public ProfileViewModel() : this(null!, null!, null!)
    {
    }

    public ProfileViewModel(ILoggedInUserService loggedInUserService, IDialogService dialogService, IChatApi chatApi)
    {
        _dialogService = dialogService;
        _chatApi = chatApi;
        _loggedInUser = loggedInUserService.User!;
    }

    [RelayCommand]
    private async Task ChangeAvatarImage()
    {
        var image = await _dialogService.ShowFilePicker("Choose avatar", FilePickerFileTypes.ImageAll);

        if (image is null)
        {
            return;
        }

        LoggedInUser.Avatar = await File.ReadAllBytesAsync(image.Path.LocalPath);
    }

    [RelayCommand]
    private async Task UpdateProfile()
    {
        var update = new UpdateUserRequest()
        {
            Id = LoggedInUser.UserId,
            FirstName = LoggedInUser.FirstName,
            LastName = LoggedInUser.LastName,
            Avatar = LoggedInUser.Avatar
        };
        
        IsLoading = true;

        var result = await _chatApi.UpdateUser(LoggedInUser.UserId, update, 
            $"Bearer {LoggedInUser.Token}");

        IsLoading = false;
    }
    
}