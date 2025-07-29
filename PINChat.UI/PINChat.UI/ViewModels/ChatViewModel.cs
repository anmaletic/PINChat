using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserModel _user = new();
    
    [ObservableProperty]
    private UserModel _selectedContact = new();

    [ObservableProperty]
    private string _contact = "";

    public ChatViewModel()
    {
        GenerateDemoContacts();
    }

    partial void OnSelectedContactChanged(UserModel value)
    {
    }

    private void GenerateDemoContacts()
    {
        User = new UserModel()
        {
            FirstName = "Antonio",
            LastName = "Maletic",
            DisplayName = "Anmal",
            Contacts =
            [
                new UserModel()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DisplayName = "JohnD",
                },
                new UserModel()
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    DisplayName = "JaneS",
                },
                new UserModel()
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    DisplayName = "AliceJ",
                },
                new UserModel()
                {
                    FirstName = "Bob",
                    LastName = "Brown",
                    DisplayName = "BobB",
                },
                new UserModel()
                {
                    FirstName = "Charlie",
                    LastName = "Davis",
                    DisplayName = "CharlieD",
                },
            ]
        };
    }

}