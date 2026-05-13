using HelpDesk.Api.Clients;
using HelpDesk.Api.Endpoints.Employee;
using HelpDesk.Api.Endpoints.Employees;
using HelpDesk.Api.Endpoints.Techs;
using HelpDesk.Api.ReadModels;
using HelpDesk.Api.Services;
using JasperFx;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Wolverine;
using Wolverine.Marten;
using Wolverine.Nats;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();
builder.Host.UseWolverine(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.Durability.Mode = DurabilityMode.Solo;
    }
    options.UseNats(builder.Configuration.GetConnectionString("nats")!).AutoProvision().UseJetStream(js =>
    {
        js.MaxDeliver = 80;
    });

    options.ListenToNatsSubject("software.>").ProcessInline();

});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // optional - ask don't just do this.
    options.SerializerOptions.DefaultIgnoreCondition =  JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; 
});
builder.Services.AddValidation();

//var configedUri = builder.Configuration.GetValue<string>("SOFTWARE_CENTER_HTTP") ?? "";
//var address = new Uri(configedUri);
//builder.Services.AddHttpClient<SoftwareCenterHttpClient>(client =>
//{
//    // do all your client configuration here, URI, proxy stuff, whatever.
//    client.BaseAddress = address;
//    //var uri = builder.Configuration.Get()

//});

builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();
// get the defaults for your "team" here.
builder.AddServiceDefaults(); // From ServiceDefaults

var connectionString = builder.Configuration.GetConnectionString("help-desk-db") ?? 
    throw new Exception("No connection string for help desk database");
builder.AddNpgsqlDataSource("help-desk-db");
builder.Services.AddMarten(options =>
{
  //  options.Connection(connectionString); // One Way To Do It
    options.Projections.Add<EmployeeProblemProjection>(ProjectionLifecycle.Inline); // Transactionally Consistent.
}).UseLightweightSessions().IntegrateWithWolverine().UseNpgsqlDataSource();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IMapEmployeeSubsToInternalIds, FakeEmployeeSubMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment()) // simulating a kind of feature flag
{

    app.MapEmployeeEndpoints();
    app.MapEmployeesEndpoints();
    app.MapTechEndpoints();
}
app.MapDefaultEndpoints(); // From ServiceDefaults
return await app.RunJasperFxCommands(args);

