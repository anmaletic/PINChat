namespace PINChat.Api.Features.Authentication;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    

    public override void Configure()
    {
        Post(ApiEndpoints.Auth.Register);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Registers a new user.";
            s.Description = "Creates a new user account with ASP.NET Core Identity.";
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _userManager.FindByEmailAsync(req.Email) != null)
        {
            AddError("Email already registered.", "DuplicateEmail");
            await Send.ErrorsAsync(409, ct); // Conflict
            return;
        }
        
        var user = new ApplicationUser
        {
            UserName = req.UserName,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error.Description, error.Code);
            }
            await Send.ErrorsAsync(400, ct); // Bad Request
            return;
        }

        await Send.ResponseAsync(new RegisterResponse
        {
            UserId = user.Id,
            Message = "Registration successful!"
        }, cancellation: ct);
    }
}