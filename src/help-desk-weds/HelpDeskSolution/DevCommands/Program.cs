using SharedTypes;
using Wolverine;
using Wolverine.Nats;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// This is going to create (provision) the stream SOFTWARE, software.> on nats.
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
var vsCodeId = Guid.Parse("25ff5f2d-cdaa-4a6b-96f7-f018d3f27e59");
var resharperId = Guid.Parse("d2a03441-2444-4453-bab3-0ad872068ff3");
var software = new List<AddSoftwareItem>(
     [
    

        new(vsCodeId, "Visual Studio", "Microsoft"),
        new(resharperId, "ReSharper", "JetBrains"),
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
app.MapPost("/retire", async (IMessageBus bus) =>
{
    await bus.PublishAsync(new RetireSoftwareItem(resharperId, DateTimeOffset.UtcNow));
    return Results.Ok();
});


app.Run();

