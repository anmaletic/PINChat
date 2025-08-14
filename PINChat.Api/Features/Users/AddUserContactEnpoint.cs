using Microsoft.EntityFrameworkCore;

namespace PINChat.Api.Features.Users;

public class AddUserContactEnpoint : Endpoint<UserContactRequest, UserContactResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AddUserContactEnpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post(ApiEndpoints.Users.AddContact);
        AuthSchemes("Bearer");
        Summary(s =>
        {
            s.Summary = "Adds a contact to a user.";
            s.Description = "Allows a user to add another user as a contact by providing the contact id.";
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

        user.MyContacts.Add(new Contact()
        {
            UserId = currentUserId,
            ContactUserId = req.ContactId
        });

        await _userManager.UpdateAsync(user);

        await Send.OkAsync(new UserContactResponse
        {
            Message = "Contact added successfully."
        }, cancellation: ct);
    }
}