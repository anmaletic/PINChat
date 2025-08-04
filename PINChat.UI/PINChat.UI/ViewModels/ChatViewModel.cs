using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;
using Refit;

namespace PINChat.UI.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private CancellationTokenSource? _typingCancellationTokenSource;
    private const int TypingTimeoutMs = 1500;
    
    private readonly IChatService _chatService;
    private readonly IChatApi _chatApi;

    [ObservableProperty]
    private UserModel _user = new();

    [ObservableProperty]
    private UserModel _selectedContact = new();

    [ObservableProperty]
    private string _newMsgContent = "";

    public ChatViewModel() : this(null!, null!, null!)
    {
    }

    public ChatViewModel(ILoggedInUserService loggedInUserService, IChatService chatService, IChatApi chatApi)
    {
        _chatService = chatService;
        _chatApi = chatApi;

        User = loggedInUserService.User!;

        // Subscribe to chat service events
        _chatService.MessageReceived += OnMessageReceived;
        _chatService.MessageStatusUpdated += OnMessageStatusUpdated;
        _chatService.ConnectionStatusChanged += OnConnectionStatusChanged;
        _chatService.TypingStatusReceived += OnTypingStatusReceived;

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
    
    private void OnTypingStatusReceived(object? sender, TypingStatusEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var contact = User.Contacts.FirstOrDefault(c => c.UserId == e.UserId);
            if (contact != null)
            {
                contact.IsTyping = e.IsTyping; // Update the IsTyping property directly
            }
        });
    }
    
    partial void OnSelectedContactChanged(UserModel value)
    {
        LoadChatHistoryForSelectedContact();
    }

    partial void OnNewMsgContentChanged(string value)
    {
        _ = SendTypingStatusUpdate();
    }

    private async Task LoadChatHistoryForSelectedContact()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Token))
                {
                    Console.WriteLine("No authentication token available to fetch chat history.");
                    return;
                }

                var historyResponse = await _chatApi.GetChatHistory(
                    SelectedContact.UserId,
                    $"Bearer {User.Token}"
                );

                if (historyResponse.IsSuccessStatusCode && historyResponse.Content != null)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        foreach (var msgDto in historyResponse.Content.Messages)
                        {
                            SelectedContact.Messages.Add(new ChatMessageModel
                            {
                                Id = msgDto.Id,
                                Content = msgDto.Content,
                                Sender = msgDto.SenderId,
                                IsOrigin = msgDto.SenderId == User.UserId,
                                Timestamp = msgDto.Timestamp,
                                IsSent = msgDto.IsSent,
                                IsReceived = msgDto.IsReceived,
                                IsRead = msgDto.IsRead
                            });
                        }
                    });
                }
                else
                {
                    Console.WriteLine($"Failed to load chat history: {historyResponse.Error?.Content}");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API Error loading chat history: {ex.Message} - {ex.Content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error loading chat history: {ex.Message}");
            }
        }

    private async Task SendTypingStatusUpdate()
    {
        if (SelectedContact == null || !_chatService.IsConnected)
        {
            return;
        }

        _typingCancellationTokenSource?.Cancel();
        _typingCancellationTokenSource = new CancellationTokenSource();
        var token = _typingCancellationTokenSource.Token;

        if (!string.IsNullOrWhiteSpace(NewMsgContent))
        {
            await _chatService.SendTypingStatusAsync(SelectedContact.UserId!, true);
        }

        try
        {
            await Task.Delay(TypingTimeoutMs, token);
            await _chatService.SendTypingStatusAsync(SelectedContact.UserId!, false);
        }
        catch (TaskCanceledException)
        {
            // User continued typing, so the previous task was cancelled.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending typing status: {ex.Message}");
        }
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