using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
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
    private string _newMsgContent = "";

    public ChatViewModel()
    {
        GenerateDemoMessages();
        GenerateDemoContacts();
    }

    partial void OnSelectedContactChanged(UserModel value)
    {
        GenerateDemoMessages();
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

    private void GenerateDemoMessages()
    {
        if (SelectedContact == null)
            return;

        SelectedContact.Messages = new ObservableCollection<ChatMessageModel>
        {
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-10),
                Sender = User.DisplayName,
                Content = "Hello, how are you?",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = true
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-9),
                Sender = SelectedContact.DisplayName,
                Content = "I'm good, thanks! How about you?",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = false
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-8),
                Sender = User.DisplayName,
                Content = "Doing well, just working on some projects.",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = true
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-7),
                Sender = SelectedContact.DisplayName,
                Content = "Sounds great! Let me know if you need any help.",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = false
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-6),
                Sender = User.DisplayName,
                Content = "Thanks! I appreciate it.",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = true
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-5),
                Sender = SelectedContact.DisplayName,
                Content = "No problem! Let's catch up later.",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = false
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-4),
                Sender = User.DisplayName,
                Content = "Sure, talk to you later!",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = true
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-3),
                Sender = SelectedContact.DisplayName,
                Content = "Take care!",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = false
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-2),
                Sender = User.DisplayName,
                Content = "You too!",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = true
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now.AddMinutes(-1),
                Sender = SelectedContact.DisplayName,
                Content = "Bye!",
                IsSent = true,
                IsReceived = true,
                IsRead = true,
                IsOrigin = false
            },
            new ChatMessageModel
            {
                Timestamp = DateTime.Now,
                Sender = User.DisplayName,
                Content = "Bye!",
                IsSent = true,
                IsReceived = false,
                IsRead = false,
                IsOrigin = true
            }
            
        };
    }
    
    
    [RelayCommand]
    private async Task HandleKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter when !e.KeyModifiers.HasFlag(KeyModifiers.Shift):
            {
                await SendMessage();
                e.Handled = true;
                break;
            }
            case Key.Enter when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
            {
                NewMsgContent += Environment.NewLine;
                var textBox = e.Source as TextBox;
                textBox!.CaretIndex = textBox?.Text?.Length ?? 0;
                e.Handled = true;
                break;
            }
        }
    }

    [RelayCommand]
    private async Task SendMessage()
    {
        var msg = new ChatMessageModel()
        {
            Timestamp = DateTime.Now,
            Sender = User.DisplayName,
            Content = NewMsgContent,
            IsSent = true,
            IsReceived = false,
            IsRead = false,
            IsOrigin = true
        };   
        
        if (SelectedContact != null)
        {
            SelectedContact.Messages.Add(msg);
            NewMsgContent = string.Empty;
        }

        // simulate message received delay
        await Task.Delay(1000);
        msg.IsReceived = true;

        // simulate message read delay
        await Task.Delay(1000);
        msg.IsRead = true;
    }

    
    [RelayCommand]
    private void ChangeTheme()
    {    
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}