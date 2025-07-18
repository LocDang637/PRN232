using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Components;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register services first
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<CoachService>();
// Configure HttpClient with a name and proper base address
builder.Services.AddHttpClient("GraphQLClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7045");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register GraphQLService with explicit HttpClient factory
builder.Services.AddScoped<GraphQLService>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("GraphQLClient");
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new GraphQLService(httpClient, configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();