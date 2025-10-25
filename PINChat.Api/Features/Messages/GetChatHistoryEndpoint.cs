using Microsoft.EntityFrameworkCore;
using PINChat.Api.Mapping;
using PINChat.Persistence.Db.Contexts;

namespace PINChat.Api.Features.Messages;

public class GetChatHistoryEndpoint : Endpoint<GetChatHistoryRequest, GetChatHistoryResponse>
{
    private readonly AppDbContext _dbContext;

    public GetChatHistoryEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override void Configure()
    {
        Get(ApiEndpoints.Messages.GetChatHistory);
        AuthSchemes(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme);
        Summary(s =>
        {
            s.Summary = "Retrieves chat history between the authenticated user and a specific contact.";
            s.Description = "Fetches all messages where the authenticated user is either sender or recipient with the specified contact.";
        });
    }
    

    public override async Task HandleAsync(GetChatHistoryRequest req, CancellationToken ct)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Query messages where:
        // (Sender is current user AND Recipient is contact) OR
        // (Sender is contact AND Recipient is current user)
        var messages = await _dbContext.Messages
            .Include(m => m.Sender)
            .Where(m =>
                (m.SenderId == currentUserId && m.RecipientId == req.ContactId) ||
                (m.SenderId == req.ContactId && m.RecipientId == currentUserId))
            .OrderBy(m => m.Timestamp)
            .Select(m => m.ToResponse())
            .ToListAsync(ct);

        await Send.ResponseAsync(new GetChatHistoryResponse
        {
            Messages = messages
        }, cancellation: ct);
    }
}