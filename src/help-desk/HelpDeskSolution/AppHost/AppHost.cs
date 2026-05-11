var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);



// Above this line is Infrastructure Stuff - Tech stack choices.

var helpdeskDatabase = postgresServer.AddDatabase("help-desk-db"); // You can add initialization scripts, etc. 

// Below is our "app";

var helpdeskApi = builder.AddProject<Projects.HelpDesk_Api>("help-desk")
    .WithReference(helpdeskDatabase)
    .WaitFor(postgresServer);

builder.Build().Run();
