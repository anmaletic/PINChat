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
        var user = await _userManager.FindByNameAsync(req.UserName);
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

        await Send.ResponseAsync(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            UserName = user.UserName,
            Avatar = user.Avatar,
            Message = "Login successful!"
        }, cancellation: ct);
    }

}