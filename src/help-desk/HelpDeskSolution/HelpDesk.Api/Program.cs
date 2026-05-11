using Marten;

var builder = WebApplication.CreateBuilder(args);
// get the defaults for your "team" here.
builder.AddServiceDefaults(); // From ServiceDefaults

var connectionString = builder.Configuration.GetConnectionString("help-desker-db") ?? 
    throw new Exception("No connection string for help desk database");

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString); // One Way To Do It
}).UseLightweightSessions();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints(); // From ServiceDefaults
app.Run();
