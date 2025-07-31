using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PINChat.ChatServer.Mapping;
using PINChat.Persistence.Db.Contexts;
using PINChat.Persistence.Db.Entities;

namespace PINChat.ChatServer.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    private static readonly ConcurrentDictionary<string, string> ConnectedUsers = [];

    public ChatHub(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            ConnectedUsers.AddOrUpdate(userId, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);
            Console.WriteLine($"User {userId} connected with ConnectionId: {Context.ConnectionId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            ConnectedUsers.TryRemove(userId, out _);
            Console.WriteLine($"User {userId} disconnected from ConnectionId: {Context.ConnectionId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string recipientId, string content, string tempMsgId)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null)
        {
            throw new HubException("User is not authenticated.");
        }
        
        var senderUser = await _userManager.FindByIdAsync(senderId);
        if (senderUser == null)
        {
            throw new HubException("Sender user not found.");
        }
        
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            SenderId = senderId,
            RecipientId = recipientId,
            Content = content,
            IsSent = true,
            IsReceived = false,
            IsRead = false
        };
        
        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        
        var response = message.ToResponse();
        response.TempId = tempMsgId;
        
        // Notify the sender
        await Clients.Caller.SendAsync("ReceiveMessage", response);
        
        // Notify the recipient if they are connected
        if (ConnectedUsers.TryGetValue(recipientId, out var recipientConnectionId))
        {
            await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", response);
        }
    }
    
    public async Task MessageReceived(string messageId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return;
        }
        
        var message = await _dbContext.Messages.FindAsync(messageId);
        if (message == null || message.RecipientId != userId || message.IsReceived)
        {
            return;
        }
        
        message.IsReceived = true;
        await _dbContext.SaveChangesAsync();
        
        // Notify the sender that the message has been received
        if (ConnectedUsers.TryGetValue(message.SenderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("UpdateMessageStatus", message.Id, "Received");
        }
    }
    
    public async Task MessageRead(string messageId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return;
        }
        
        var message = await _dbContext.Messages.FindAsync(messageId);
        if (message == null || message.RecipientId != userId || message.IsRead)
        {
            return;
        }
        
        message.IsRead = true;
        await _dbContext.SaveChangesAsync();
        
        // Notify the sender that the message has been read
        if (ConnectedUsers.TryGetValue(message.SenderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("UpdateMessageStatus", message.Id, "Read");
        }
    }
}