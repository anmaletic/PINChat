using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PINChat.Api.Sdk;
using PINChat.Core.Domain.Enums;
using PINChat.UI.Core.Extensions;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;
using PINChat.UI.Core.Services;
using Refit;

namespace PINChat.UI.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private CancellationTokenSource? _cancellationTokenSource = new();
    
    private CancellationTokenSource? _typingCancellationTokenSource;
    private const int TypingTimeoutMs = 1500;

    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IChatService _chatService;
    private readonly IChatApi _chatApi;
    private readonly IDialogService _dialogService;
    private readonly IMinioFrontendService _minioService;

    [ObservableProperty]
    private UserModel _user = new();

    [ObservableProperty]
    private UserModel? _selectedContact;

    [ObservableProperty]
    private string _newMsgContent = "";

    [ObservableProperty]
    private bool _isLoadingMessages;

    [ObservableProperty]
    private bool _isMobileMessagesPaneVisible;

    public ChatViewModel() : this(null!, null!, null!, null!, null!)
    {
    }

    public ChatViewModel(ILoggedInUserService loggedInUserService, IChatService chatService, IChatApi chatApi,
        IDialogService dialogService, IMinioFrontendService minioService)
    {
        StrongReferenceMessenger.Default.Register<OnBackPressedMessage>(this, OnBackPressedMessageReceived);

        _loggedInUserService = loggedInUserService;
        _chatService = chatService;
        _chatApi = chatApi;
        _dialogService = dialogService;
        _minioService = minioService;

        User = _loggedInUserService.User!;

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

        var existingMsg = SelectedContact!.Messages.FirstOrDefault(m => m.Id == backendMessage.TempId);

        if (existingMsg != null && existingMsg.IsOrigin)
        {
            existingMsg.Id = backendMessage.Id;
            existingMsg.IsSent = backendMessage.IsSent;
        }
        else if (backendMessage.RecipientId == User.UserId &&
                 backendMessage.SenderId == SelectedContact.UserId)
        {
            var incomingMsg = backendMessage.ToModel();

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
        var msgToUpdate = SelectedContact!.Messages.FirstOrDefault(m => m.Id == e.MessageId);
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

    partial void OnSelectedContactChanged(UserModel? value)
    {
        if (value is null)
        {
            return;
        }
        
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        _ = LoadChatHistoryForSelectedContact(_cancellationTokenSource.Token);
        IsMobileMessagesPaneVisible = true;
    }

    partial void OnNewMsgContentChanged(string value)
    {
        _ = SendTypingStatusUpdate();
    }

    private async Task LoadChatHistoryForSelectedContact(CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(User.Token))
            {
                Console.WriteLine("No authentication token available to fetch chat history.");
                return;
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            SelectedContact!.Messages.Clear();

            IsLoadingMessages = true;

            var historyResponse = await _chatApi.GetChatHistory(
                SelectedContact.UserId,
                $"Bearer {User.Token}",
                cancellationToken
            );

            if (historyResponse.IsSuccessStatusCode && historyResponse.Content != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var msgs = historyResponse.Content.Messages.ToModels().ToList();
                    msgs.UpdateMessageOrigin(User.UserId);

                    SelectedContact.Messages.AddRange(msgs);
                });
            }
            else
            {
                Console.WriteLine($"Failed to load chat history: {historyResponse.Error?.Content}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Chat history loading was canceled.");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"API Error loading chat history: {ex.Message} - {ex.Content}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error loading chat history: {ex.Message}");
        }
        finally
        {
            IsLoadingMessages = false;
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
            await _chatService.SendTypingStatusAsync(SelectedContact.UserId, true);
        }

        try
        {
            await Task.Delay(TypingTimeoutMs, token);
            await _chatService.SendTypingStatusAsync(SelectedContact.UserId, false);
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
            MessageType = MessageType.Text,
            IsSent = true,
            IsReceived = false,
            IsRead = false,
            IsOrigin = true
        };

        SelectedContact.Messages.Add(msg);
        NewMsgContent = string.Empty;

        try
        {
            await _chatService.SendMessageAsync(SelectedContact!.UserId, msg.Content, tempMsgId, msg.MessageType, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message via chat service: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(() =>
                SelectedContact!.Messages.Remove(msg));
        }
    }

    [RelayCommand]
    private async Task SelectAndSendImage()
    {
        if (SelectedContact == null || !_chatService.IsConnected)
        {
            Console.WriteLine("Cannot send image: no contact selected, not logged in, or chat service not connected.");
            return;
        }

        var file = await _dialogService.ShowFilePicker("Odabir slike", FilePickerFileTypes.ImageAll);

        if (file is not null)
        {
            ChatMessageModel msg = new();

            try
            {
                await using var stream = await file.OpenReadAsync();
                var fileName = file.Name;
                var contentType = ContentTypeHelper.GetType(fileName);

                var tempMsgId = Guid.NewGuid().ToString();
                msg = new ChatMessageModel
                {
                    Id = tempMsgId,
                    Timestamp = DateTime.Now,
                    Sender = User.UserName ?? "Unknown",
                    IsOrigin = true,
                    Content = "Uploading image...",
                    MessageType = MessageType.Image,
                    ImagePath = null,
                    IsSent = false,
                    IsReceived = false,
                    IsRead = false
                };
                SelectedContact.Messages.Add(msg);

                // Upload image to MinIO via backend API
                var imageUrl = await _minioService.UploadImageAsync(stream, fileName, contentType);
                Console.WriteLine($"Image uploaded via backend: {imageUrl}");

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    msg.ImagePath = imageUrl;
                    msg.Content = "";
                    msg.IsSent = true;
                });

                await _chatService.SendMessageAsync(SelectedContact.UserId, string.Empty, tempMsgId, MessageType.Image,
                    imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending image: {ex.Message}");
                Dispatcher.UIThread.InvokeAsync(() => SelectedContact.Messages.Remove(msg));
            }
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

    [RelayCommand]
    private void Logout()
    {
        _loggedInUserService.ClearUser();

        StrongReferenceMessenger.Default.UnregisterAll(this);
        StrongReferenceMessenger.Default.Send(new ChangeViewMessage(nameof(LoginViewModel)));
    }

    [RelayCommand]
    private async Task ExportMessages()
    {
        if (SelectedContact == null || SelectedContact.Messages.Count == 0)
        {
            Console.WriteLine("No messages to export or no contact selected.");
            return;
        }
        
        var file = await _dialogService.ShowSaveFilePicker(
            "Save Chat History",
            $"ChatHistory_{SelectedContact.UserName}",
            "json",
            "application/json"
        );
        
        if (file == null)
        {
            Console.WriteLine("Save operation cancelled by user.");
            return;
        }

        try
        {
            // Serialize the messages collection to a JSON string
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(SelectedContact.Messages, jsonOptions);

            // Write the JSON string to the selected file
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(jsonString);

            Console.WriteLine($"Messages successfully exported to {file.Path.LocalPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting messages to JSON: {ex.Message}");
        }
    }

    private void OnBackPressedMessageReceived(object recipient, OnBackPressedMessage message)
    {
        if (!PlatformHelper.IsMobile())
        {
            return;
        }

        IsMobileMessagesPaneVisible = false;
        SelectedContact = null!;
    }

    [RelayCommand]
    private void HandleSelectedDockItem(DockButton btn)
    {
        
    }
}