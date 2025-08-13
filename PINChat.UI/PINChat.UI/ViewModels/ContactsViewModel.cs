using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Extensions;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class ContactsViewModel : LoadableViewModelBase
{
    private readonly IChatApi _chatApi;

    [ObservableProperty]
    private UserModel _loggedInUser;

    [ObservableProperty]
    private SmartObservableCollection<UserModel> _allContacts = [];

    public ContactsViewModel() : this(null!, null!)
    {
    }

    public ContactsViewModel(ILoggedInUserService loggedInUserService, IChatApi chatApi)
    {
        _chatApi = chatApi;
        _loggedInUser = loggedInUserService.User!;

        _ = LoadContacts();
    }

    private async Task LoadContacts()
    {
        IsLoading = true;

        var response = await _chatApi.GetAllUsers($"Bearer {LoggedInUser.Token}");

        if (response is { IsSuccessStatusCode: true, Content: not null })
        {
            var users = response.Content.Users.ToModels();
            
            await AllContacts.OnUIThread().AddRangeAsync(users);
        }

        IsLoading = false;
    }
}