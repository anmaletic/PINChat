using System;
using System.IO;
using System.Linq;
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

    [ObservableProperty]
    private int _detailsDays;

    [ObservableProperty]
    private int _detailsTotalMessages;

    [ObservableProperty]
    private string _detailsMostSentTo;

    [ObservableProperty]
    private int _requestCount;

    public ProfileViewModel() : this(null!, null!, null!)
    {
    }

    public ProfileViewModel(ILoggedInUserService loggedInUserService, IDialogService dialogService, IChatApi chatApi)
    {
        _dialogService = dialogService;
        _chatApi = chatApi;
        _loggedInUser = loggedInUserService.User!;
        
        LoadDetails();
    }

    [RelayCommand]
    private async Task ChangeAvatarImage()
    {
        var image = await _dialogService.ShowFilePicker("Choose avatar", FilePickerFileTypes.ImageAll);

        if (image is null)
        {
            return;
        }

        await using var stream = await image.OpenReadAsync();
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        
        LoggedInUser.Avatar = memoryStream.ToArray();
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

    private void LoadDetails()
    {
        DetailsDays = (DateTime.Now - LoggedInUser.CreatedAt).Days;
            
        var mostSentToCount = 0;

        foreach (var contact in LoggedInUser.Contacts)
        {
            if (contact.Messages.Count > mostSentToCount)
            {
                mostSentToCount = contact.Messages.Count;
                DetailsMostSentTo = contact.UserName!;
            }
            
            DetailsTotalMessages += contact.Messages.Count;
        }
        
        RequestCount = LoggedInUser.AddedByOthers.Count(u => LoggedInUser.Contacts.All(c => c.UserId != u.UserId));
    }
}