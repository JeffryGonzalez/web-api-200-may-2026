using AppHost;
using WireMock.Client.Builders;
var builder = DistributedApplication.CreateBuilder(args);

var natsTransport = builder.AddNats("nats")
    .WithJetStream() // event streaming support
    .WithLifetime(ContainerLifetime.Persistent);

var postgresServer = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
.WithPgAdmin();

//var softwareCenterService = builder.AddExternalService("software-center", "http://software-center-dev.someurl.com");

var devCommands = builder.AddProject<Projects.DevCommands>("dev-commands")
    .WithReference(natsTransport)
    .WithHttpCommand("/seed", "Seed Nats With Software")
    .WithHttpCommand("/retire", "Retire a Piece of Softwre");

var mappingPath = Path.Combine(".", "wiremock-mappings");
if(!Directory.Exists(mappingPath))
{
    throw new Exception("Create the mappings folder");
}

//var softwareCenterService = builder.AddWireMock("software-center")

//    .WithMappingsPath(mappingPath)
//    .WithReadStaticMappings()
//    .WithWatchStaticMappings()
//    .WithApiMappingBuilder(SoftwareApiMock.Build);

// Above this line is Infrastructure Stuff - Tech stack choices.

var helpdeskDatabase = postgresServer.AddDatabase("help-desk-db"); // You can add initialization scripts, etc. 

// Below is our "app";

var helpdeskApi = builder.AddProject<Projects.HelpDesk_Api>("help-desk")
    .WithReference(helpdeskDatabase)
    //.WithReference(softwareCenterService)
    .WithReference(natsTransport)
    .WaitFor(helpdeskDatabase)
    .WaitFor(postgresServer);

var vipsApi = builder.AddProject<Projects.Vips_Api>("vips-api")
    .WithReference(natsTransport);

var newVips = builder.AddProject<Projects.Vips_New_Api>("vips-new-api");
var gateway = builder.AddProject<Projects.Gateway>("gateway")
    .WithReference(helpdeskApi)
    .WithReference(vipsApi)
    .WithReference(newVips)
    .WaitFor(helpdeskApi)
    .WaitFor(vipsApi)
    .WithChildRelationship(helpdeskApi)
    .WithChildRelationship(vipsApi)
    .WithChildRelationship(newVips);

builder.Build().Run();
