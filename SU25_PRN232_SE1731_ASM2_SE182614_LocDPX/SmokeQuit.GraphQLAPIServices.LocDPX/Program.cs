using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmokeQuit.GraphQLAPIServices.LocDPX.GraphQLs;
using SmokeQuit.Services.LocDPX;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// CORS Configuration for Vite dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173", "https://localhost:7172", "http://localhost:5063") // Vite dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Important for JWT cookies if used
    });

    // Optional: More permissive policy for development
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
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
app.UseCors("ReactDevPolicy");
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