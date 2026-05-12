using AppHost;
using WireMock.Client.Builders;
var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

//var softwareCenterService = builder.AddExternalService("software-center", "http://software-center-dev.someurl.com");

var mappingPath = Path.Combine(".", "wiremock-mappings");
if(!Directory.Exists(mappingPath))
{
    throw new Exception("Create the mappings folder");
}

var softwareCenterService = builder.AddWireMock("software-center")

    .WithMappingsPath(mappingPath)
    .WithReadStaticMappings()
    .WithWatchStaticMappings()
    .WithApiMappingBuilder(SoftwareApiMock.Build);

// Above this line is Infrastructure Stuff - Tech stack choices.

var helpdeskDatabase = postgresServer.AddDatabase("help-desk-db"); // You can add initialization scripts, etc. 

// Below is our "app";

var helpdeskApi = builder.AddProject<Projects.HelpDesk_Api>("help-desk")
    .WithReference(helpdeskDatabase)
    .WithReference(softwareCenterService)
    .WaitFor(helpdeskDatabase)
    .WaitFor(postgresServer);

builder.Build().Run();
