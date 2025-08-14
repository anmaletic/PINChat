using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PINChat.Api.Sdk;
using PINChat.Contracts.Requests;
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
    private UserModel _selectedContact;
    
    [ObservableProperty]
    private bool _isMyContactsTabSelected = true;

    private List<UserModel> _allContacts;
    
    [ObservableProperty]
    private SmartObservableCollection<UserModel> _filteredAllContacts = [];
    
    [ObservableProperty]
    private SmartObservableCollection<UserModel> _addedByOthersContacts = [];

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
            // Filter out the logged-in user from the contacts list
            _allContacts = response.Content.Users
                .Where(u => u.UserId != LoggedInUser.UserId)
                .ToModels()
                .ToList();
        }

        await RefreshContacts();

        IsLoading = false;
    }

    private async Task RefreshContacts()
    {
        var filteredAddedByOthersContacts = FilterContacts(LoggedInUser.AddedByOthers);
        await AddedByOthersContacts.OnUIThread().ClearAsync();
        await AddedByOthersContacts.OnUIThread().AddRangeAsync(filteredAddedByOthersContacts);
        
        var filteredContacts = FilterContacts(_allContacts);
        await FilteredAllContacts.OnUIThread().ClearAsync();
        await FilteredAllContacts.OnUIThread().AddRangeAsync(filteredContacts);
    }

    private IEnumerable<UserModel> FilterContacts(IEnumerable<UserModel> users)
    {
        // Filter out contacts that are already LoggedInUser.Contacts
        return users.
            Where(u => LoggedInUser.Contacts.All(c => c.UserId != u.UserId))
            .OrderBy(x => x.UserName);
    }

    [RelayCommand]
    private async Task AddContact(UserModel user)
    {
        try
        {
            IsLoading = true;
            
            Console.WriteLine($"Adding contact: {user.UserId}, {user.UserName}");
        
            LoggedInUser.Contacts.Add(user);
            await RefreshContacts();

            var response = await _chatApi.AddUserContact(LoggedInUser.UserId, new UserContactRequest() { ContactId = user.UserId },
                $"Bearer {LoggedInUser.Token}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RemoveContact(UserModel user)
    {
        try
        {
            IsLoading = true;

            Console.WriteLine($"Removing contact: {user.UserId}, {user.UserName}");

            LoggedInUser.Contacts.Remove(user);
            await RefreshContacts();

            await _chatApi.RemoveUserContact(LoggedInUser.UserId, new UserContactRequest() { ContactId = user.UserId },
            $"Bearer {LoggedInUser.Token}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            IsLoading = false;
        }

    }
}