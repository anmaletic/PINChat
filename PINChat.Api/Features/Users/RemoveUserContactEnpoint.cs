using Microsoft.EntityFrameworkCore;
using PINChat.Persistence.Db.Contexts;

namespace PINChat.Api.Features.Users;

public class RemoveUserContactEnpoint : Endpoint<UserContactRequest, UserContactResponse>
{
    private readonly AppDbContext _dbContext;
    
    public RemoveUserContactEnpoint(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Delete(ApiEndpoints.Users.RemoveContact);
        AuthSchemes("Bearer");
        Summary(s =>
        {
            s.Summary = "Removes a contact from a user.";
            s.Description = "Allows a user to remove another user from their contacts by providing the contact id.";
        });
    }

    public override async Task HandleAsync(UserContactRequest req, CancellationToken ct)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var contactToRemove = await _dbContext.Contacts
            .FirstOrDefaultAsync(c => c.UserId == currentUserId && c.ContactUserId == req.ContactId, ct);

        if (contactToRemove == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        _dbContext.Contacts.Remove(contactToRemove);
        await _dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(new UserContactResponse
        {
            Message = "Contact removed successfully."
        }, cancellation: ct);
    }
    
}