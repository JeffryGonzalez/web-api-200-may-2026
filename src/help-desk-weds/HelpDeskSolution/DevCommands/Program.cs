using SharedTypes;
using Wolverine;
using Wolverine.Nats;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.UseWolverine(options =>
{
    options.UseNats(builder.Configuration.GetConnectionString("nats")!).AutoProvision().UseJetStream(js =>
    {
        js.MaxDeliver = 80;
    }).DefineStream("SOFTWARE", stream =>
    {
        stream.WithSubject("software.>")
            .WithLimits(maxMessages: 100, maxBytes: 10_000_000, maxAge: TimeSpan.FromDays(7));
    });
    
    options.PublishMessage<AddSoftwareItem>().ToNatsSubject("software.added");
    options.PublishMessage<RetireSoftwareItem>().ToNatsSubject("software.retired");

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var software = new List<AddSoftwareItem>(
     [
    
        new(Guid.NewGuid(), "Visual Studio", "Microsoft"),
        new(Guid.NewGuid(), "ReSharper", "JetBrains"),
        new(Guid.NewGuid(), "PostgreSQL", "PostgreSQL Global Development Group")
    ]);


app.MapPost("/seed", async (IMessageBus bus) =>
{
    foreach (var sw in software)
    {
        await bus.PublishAsync(sw);
    }
    return Results.Ok();
});


app.Run();

