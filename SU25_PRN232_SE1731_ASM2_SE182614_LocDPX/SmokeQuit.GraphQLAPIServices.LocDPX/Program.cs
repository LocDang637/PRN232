using SmokeQuit.GraphQLAPIServices.LocDPX.GraphQLs;
using SmokeQuit.Services.LocDPX;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add HttpContextAccessor for accessing user claims
builder.Services.AddHttpContextAccessor();

// GraphQL Configuration with Authorization
builder.Services.AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddMutationType<Mutations>()
    .AddAuthorization() // This requires HotChocolate.AspNetCore.Authorization package
    .BindRuntimeType<DateTime, DateTimeType>();

builder.Services.AddScoped<IServiceProviders, ServiceProviders>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map GraphQL endpoint - FIXED: Removed duplicate routing
app.MapGraphQL();

app.MapControllers();

app.Run();

// JwtSettings class
public class JwtSettings
{
    public string Secret { get; set; } = "YourSuperSecretJwtKeyThatShouldBeAtLeast32CharactersLong!";
    public string Issuer { get; set; } = "SmokeQuit.GraphQLAPIServices.LocDPX";
    public string Audience { get; set; } = "SmokeQuit.GraphQLClients.BlazorWAS.LocDPX";
    public int ExpirationDays { get; set; } = 7;
}