using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private readonly IChatService _chatService;

    [ObservableProperty]
    private UserModel _user = new();

    [ObservableProperty]
    private UserModel _selectedContact = new();

    [ObservableProperty]
    private string _newMsgContent = "";

    public ChatViewModel() : this(null!, null!)
    {
    }

    public ChatViewModel(ILoggedInUserService loggedInUserService, IChatService chatService)
    {
        _chatService = chatService;

        User = loggedInUserService.User!;

        // Subscribe to chat service events
        _chatService.MessageReceived += OnMessageReceived;
        _chatService.MessageStatusUpdated += OnMessageStatusUpdated;
        _chatService.ConnectionStatusChanged += OnConnectionStatusChanged;

        _chatService.ConnectAsync();
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        var backendMessage = e.Message;

        var existingMsg = SelectedContact.Messages.FirstOrDefault(m => m.Id == backendMessage.TempId);

        if (existingMsg != null && existingMsg.IsOrigin)
        {
            existingMsg.Id = backendMessage.Id;
            existingMsg.IsSent = backendMessage.IsSent;
        }
        else if (backendMessage.RecipientId == User.UserId &&
                 backendMessage.SenderId == SelectedContact.UserId)
        {
            var incomingMsg = new ChatMessageModel
            {
                Id = backendMessage.Id,
                Content = backendMessage.Content,
                Sender = backendMessage.SenderId, 
                IsOrigin = false,
                Timestamp = backendMessage.Timestamp,
                IsSent = backendMessage.IsSent,
                IsReceived = backendMessage.IsReceived,
                IsRead = backendMessage.IsRead
            };
            SelectedContact.Messages.Add(incomingMsg);

            _ = _chatService.SendMessageReceivedStatusAsync(incomingMsg.Id);

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _ = _chatService.SendMessageReadStatusAsync(incomingMsg.Id);
            });
        }
    }

    private void OnMessageStatusUpdated(object? sender, MessageStatusUpdatedEventArgs e)
    {
        var msgToUpdate = SelectedContact.Messages.FirstOrDefault(m => m.Id == e.MessageId);
        if (msgToUpdate == null)
        {
            return;
        }

        if (e.Status.Equals("Received", StringComparison.OrdinalIgnoreCase))
        {
            msgToUpdate.IsReceived = true;
        }
        else if (e.Status.Equals("Read", StringComparison.OrdinalIgnoreCase))
        {
            msgToUpdate.IsRead = true;
        }
    }

    private void OnConnectionStatusChanged(object? sender, bool isConnected)
    {
        Console.WriteLine($"Chat Service Connection Status: {isConnected}");
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
        if (string.IsNullOrWhiteSpace(NewMsgContent) || SelectedContact == null || !_chatService.IsConnected)
        {
            Console.WriteLine(
                "Cannot send message: content empty, no contact selected, or chat service not connected.");
            return;
        }

        var tempMsgId = Guid.NewGuid().ToString();
        var msg = new ChatMessageModel()
        {
            Id = tempMsgId,
            Timestamp = DateTime.Now,
            Sender = User.UserName!,
            Content = NewMsgContent,
            IsSent = true,
            IsReceived = false,
            IsRead = false,
            IsOrigin = true
        };

        SelectedContact.Messages.Add(msg);
        NewMsgContent = string.Empty;

        try
        {
            await _chatService.SendMessageAsync(SelectedContact!.UserId, msg.Content, tempMsgId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message via chat service: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(() =>
                SelectedContact!.Messages.Remove(msg));
        }
    }


    [RelayCommand]
    private void ChangeTheme()
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == ThemeVariant.Light
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }
    }
}