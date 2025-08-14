using Microsoft.EntityFrameworkCore;

namespace PINChat.Api.Features.Users;

public class RemoveUserContactEnpoint : Endpoint<UserContactRequest, UserContactResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public RemoveUserContactEnpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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

        var user = await _userManager.Users
            .Include(u => u.MyContacts)
            .FirstOrDefaultAsync(u => u.Id == currentUserId, ct);

        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var contactToRemove = user.MyContacts.FirstOrDefault(c => c.ContactUserId == req.ContactId);
        
        if (contactToRemove == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        user.MyContacts.Remove(contactToRemove);

        await _userManager.UpdateAsync(user);

        await Send.OkAsync(new UserContactResponse
        {
            Message = "Contact removed successfully."
        }, cancellation: ct);
    }
    
}