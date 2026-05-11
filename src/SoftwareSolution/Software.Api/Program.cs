
using Marten;
using Software.Api.CatalogItems;
using Software.Api.Clients;


var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDataSource("software-db"); // use the configuration api to find me the connection string for software-db
builder.Services.AddValidation(); 
builder.AddServiceDefaults();

// don't leave home without it - boycot all use of DateTime and DateTimeOffset as a "service" in your API. Use this instead.

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System); 

// we'll review some of this - AddAuthentication is backed in, AddJwtBearer comes from a nuget package.
// Also has cookie authentication baked in.
builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorizationBuilder().AddPolicy("SoftwareCenterManager", pol =>
{
    // Chaining RequireRole calls enforces AND logic — the user must have BOTH roles.
    // Note: RequireRole("SoftwareCenter", "Manager") would be OR logic (either role suffices).
    pol.RequireRole("SoftwareCenter");
    pol.RequireRole("Manager");
}).AddPolicy("SoftwareCenter", pol =>
{
    pol.RequireRole("SoftwareCenter");
});

// only need this if you are using controller - not needed with "minimal apis"

builder.Services.AddControllers();

// This is just the ability to generate the OpenApi doc (Swagger)
builder.Services.AddOpenApi();
// appsetting.json, appsettings.ENVIRONMENT.json (aspnetcore_environment), "secrets", environment variables, command line args
var connectionString = builder.Configuration.GetConnectionString("software-db") ??
    throw new Exception("no connection string");

builder.Services.AddMarten(options =>
{


}).UseLightweightSessions().UseNpgsqlDataSource();


builder.Services.AddHttpClient<NotificationsApi>(client =>
{
    // we prefer https, but we'll http if that is availble, and get the address for that api.
    client.BaseAddress = new Uri("https+http://notification-api"); // "Service Discovery" - 
});
// a service is some code that owns some data (state) and the process around that data.
// to add services - you specify the lifetime and the implementation
// lifetime is transient, scoped, singleton
// scoped (your default) - one per http request - anything that has to do with databases and the user that is making the request HAS to be scoped.
// transient - I don't use this very often - just means "a new instance every time"
builder.Services.AddScoped<IDoNotifications>(sp => sp.GetRequiredService<NotificationsApi>());
// above here is configuration of services
var app = builder.Build();
// after this line is middleware - how requests are handled and responses are made.


if (app.Environment.IsDevelopment())
{
    // if we are in development, make it so the developer can read the openapi spec. (defaults to /openapi/v1.json)

    app.MapOpenApi();
}

// if you are exposing both HTTP and HTTPS, this says if someone hits this with http, redirect them to HTTPS.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Reads the attributes off of the controller classes during startup to create the route table
// uses reflection to do that.
app.MapControllers();

app.MapCatalogItemRoutes();

app.MapDefaultEndpoints(); // this comes from service defaults, and this is mostly health checks.
app.Run();
