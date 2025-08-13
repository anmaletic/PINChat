using Microsoft.EntityFrameworkCore;

namespace PINChat.Api.Features.Users;

public class UpdateUserEndpoint : Endpoint<UpdateUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Put(ApiEndpoints.Users.Update);
        AuthSchemes("Bearer");
        Summary(s =>
        {
            s.Summary = "Updates the current user's profile.";
            s.Description =
                "Allows the user to update their profile information such as first name, last name, and avatar.";
        });
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId) || currentUserId != req.Id)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, ct);

        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.FirstName != null) user.FirstName = req.FirstName;
        if (req.LastName != null) user.LastName = req.LastName;
        if (req.Avatar != null) user.Avatar = req.Avatar;

        await _userManager.UpdateAsync(user);

        await Send.ResponseAsync(new UpdateUserResponse
        {
            Message = "Updated profile successfully"
        }, cancellation: ct);
    }
}