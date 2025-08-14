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

        var response = await _userManager.Users
            .Where(u => u.Id == user.Id)
            .Select(u => new LoginResponse
            {
                Token = token,
                UserId = u.Id,
                UserName = u.UserName!,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Avatar = u.Avatar!,
                AvatarPath = u.AvatarPath,
                Message = string.Empty,
                Contacts = u.MyContacts.Select(c => new LoginResponse()
                {
                    UserId = c.ContactUser.Id,
                    UserName = c.ContactUser.UserName!,
                    FirstName = c.ContactUser.FirstName,
                    LastName = c.ContactUser.LastName,
                    Avatar = c.ContactUser.Avatar!,
                    AvatarPath = c.ContactUser.AvatarPath,
                    Message = string.Empty
                }),
                AddedByOthers = u.AddedByOthers.Select(c => new LoginResponse()
                {
                    UserId = c.User.Id,
                    UserName = c.User.UserName!,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Avatar = c.User.Avatar!,
                    AvatarPath = c.User.AvatarPath,
                    Message = string.Empty
                }),
            })
            .SingleAsync(cancellationToken: ct);

        await Send.ResponseAsync(response, cancellation: ct);
    }

}