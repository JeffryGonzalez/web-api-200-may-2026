using HelpDesk.Api.Clients;
using HelpDesk.Api.Employee;
using HelpDesk.Api.Services;
using Marten;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using JasperFx;
using Wolverine;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();
builder.Host.UseWolverine(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.Durability.Mode = DurabilityMode.Solo;
    }
});

//builder.Services.AddControllers().AddJsonOptions(options =>
//{

//});
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // optional - ask don't just do this.
    options.SerializerOptions.DefaultIgnoreCondition =  JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; 
});
builder.Services.AddValidation();

var configedUri = builder.Configuration.GetValue<string>("SOFTWARE_CENTER_HTTP");
var address = new Uri(configedUri);
builder.Services.AddHttpClient<SoftwareCenterHttpClient>(client =>
{
    // do all your client configuration here, URI, proxy stuff, whatever.
    client.BaseAddress = address;
    //var uri = builder.Configuration.Get()

});

builder.Services.AddProblemDetails();
// get the defaults for your "team" here.
builder.AddServiceDefaults(); // From ServiceDefaults

var connectionString = builder.Configuration.GetConnectionString("help-desk-db") ?? 
    throw new Exception("No connection string for help desk database");

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString); // One Way To Do It
}).UseLightweightSessions().IntegrateWithWolverine();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<EmployeeSubMapper>();

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
}
app.MapDefaultEndpoints(); // From ServiceDefaults
return await app.RunJasperFxCommands(args);
