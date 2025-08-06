using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.SignalR.Client;
using PINChat.Contracts.Responses;
using PINChat.Core.Domain.Enums;
using PINChat.UI.Core.Interfaces;

namespace PINChat.UI.Core.Services;

public partial class ChatService : ObservableObject, IChatService
{
    private HubConnection _hubConnection;
    private readonly ILoggedInUserService _loggedInUserService;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<MessageStatusUpdatedEventArgs>? MessageStatusUpdated;
    public event EventHandler<bool>? ConnectionStatusChanged;
    public event EventHandler<TypingStatusEventArgs>? TypingStatusReceived;
    
    [ObservableProperty]
    private bool _isConnected;

    public ChatService(ILoggedInUserService loggedInUserService)
    {
        _loggedInUserService = loggedInUserService;

        // Initialize HubConnection
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7152/chathub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_loggedInUserService.UserToken);
            })
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<MessageResponse>("ReceiveMessage", (message) =>
        {
            Dispatcher.UIThread.InvokeAsync(() => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message)));
        });
        
        _hubConnection.On<string, string>("UpdateMessageStatus", (messageId, status) =>
        {
            Dispatcher.UIThread.InvokeAsync(() => MessageStatusUpdated?.Invoke(this, new MessageStatusUpdatedEventArgs(messageId, status)));
        });

        _hubConnection.On<string, bool>("ReceiveTypingStatus", (userId, isTyping) =>
        {
            Dispatcher.UIThread.InvokeAsync(() => TypingStatusReceived?.Invoke(this, new TypingStatusEventArgs(userId, isTyping)));
        });
        
        _hubConnection.Closed += OnConnectionClosed;
        _hubConnection.Reconnecting += OnConnectionReconnecting;
        _hubConnection.Reconnected += OnConnectionReconnected;
    }

    private Task OnConnectionClosed(Exception? error)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsConnected = false;
            ConnectionStatusChanged?.Invoke(this, false);
            Console.WriteLine($"SignalR Connection Closed: {error?.Message}");
        });
        return Task.CompletedTask;
    }

    private Task OnConnectionReconnecting(Exception? error)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsConnected = false;
            ConnectionStatusChanged?.Invoke(this, false);
            Console.WriteLine($"SignalR Connection Reconnecting: {error?.Message}");
        });
        return Task.CompletedTask;
    }

    private Task OnConnectionReconnected(string? connectionId)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsConnected = true;
            ConnectionStatusChanged?.Invoke(this, true);
            Console.WriteLine($"SignalR Connection Reconnected. New ConnectionId: {connectionId}");
        });
        return Task.CompletedTask;
    }
    
    public async Task ConnectAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StartAsync();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IsConnected = true;
                    ConnectionStatusChanged?.Invoke(this, true);
                    Console.WriteLine("SignalR Connection Started.");
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IsConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                    Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
                }); 
                
                throw;
            }
        }
    }
    
    public async Task DisconnectAsync()
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StopAsync();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IsConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                    Console.WriteLine("SignalR Connection Stopped.");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping SignalR connection: {ex.Message}");
            }
        }
    }
    
    public async Task SendMessageAsync(string recipientId, string content, string tempId, MessageType messageType, string? imagePath)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            Console.WriteLine("Cannot send message: Hub not connected.");
            throw new InvalidOperationException("Chat service is not connected.");
        }
        await _hubConnection.InvokeAsync("SendMessage", recipientId, content, tempId, messageType, imagePath);
    }
    
    public async Task SendMessageReceivedStatusAsync(string messageId)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            return;
        }
        await _hubConnection.InvokeAsync("MessageReceived", messageId);
    }

    public async Task SendMessageReadStatusAsync(string messageId)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            return;
        }
        await _hubConnection.InvokeAsync("MessageRead", messageId);
    }
    
    public async Task SendTypingStatusAsync(string recipientId, bool isTyping) // RecipientId parameter added back
    {
        if (_hubConnection.State != HubConnectionState.Connected) return;
        await _hubConnection.InvokeAsync("SendTypingStatus", recipientId, isTyping);
    }
}