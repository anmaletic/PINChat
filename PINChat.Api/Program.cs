using Microsoft.AspNetCore.Authentication.JwtBearer;
using PINChat.Core.Application;
using PINChat.Core.Options;
using PINChat.Persistence.Db.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddFastEndpoints();

builder.Services.AddScoped<JwtService>();

builder.Services.AddOptions<MinioOptions>()
    .Bind(builder.Configuration.GetSection("Minio"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IFileService, MinioService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("PINChat API")
            .WithTheme(ScalarTheme.Alternate)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseCors(builder => builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.Configurator = ep =>
    {
        ep.DontAutoTag();
    };
});

app.Run();