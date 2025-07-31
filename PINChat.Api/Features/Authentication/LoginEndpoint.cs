using Microsoft.EntityFrameworkCore;
using PINChat.Api.Mapping;

namespace PINChat.Api.Features.Authentication;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly JwtService _jwtService;

    public LoginEndpoint(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IConfiguration config, JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _jwtService = jwtService;
    }

    public override void Configure()
    {
        Post(ApiEndpoints.Auth.Login);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Logs in a user.";
            s.Description = "Authenticates user and returns a JWT token using ASP.NET Core Identity.";
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.Users
            .Include(u => u.MyContacts)
            .ThenInclude(c => c.ContactUser)
            .SingleOrDefaultAsync(u => u.UserName == req.UserName, cancellationToken: ct);

        if (user == null)
        {
            AddError("Invalid credentials.", "InvalidCredentials");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);

        if (!result.Succeeded)
        {
            AddError("Invalid credentials.", "InvalidCredentials");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        var token = _jwtService.GenerateToken(user);

        var response = user.ToResponse();
        response.Token = token;

        await Send.ResponseAsync(response, cancellation: ct);
    }

}