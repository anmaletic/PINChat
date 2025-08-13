using Microsoft.EntityFrameworkCore;
using PINChat.Api.Mapping;

namespace PINChat.Api.Features.Users;

public class GetAllUsersEndpoint : Endpoint<EmptyRequest, GetAllUsersResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public override void Configure()
    {
        Get(ApiEndpoints.Users.GetAll);
        AuthSchemes("Bearer");
        Summary(s=> 
        {
            s.Summary = "Retrieves all users in the system.";
            s.Description = "This endpoint returns a list of all registered users, including their IDs, usernames, and avatars.";
        });
    }

    public override async Task HandleAsync(EmptyRequest emptyRequest, CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken: ct);
        
        if (users.Count == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        
        var response = new GetAllUsersResponse
        {
            Users = users.ToResponse().ToList()
        };

        await Send.ResponseAsync(response, cancellation: ct);
    }
}