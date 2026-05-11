using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var scalar = builder.AddScalarApiReference(options =>
{
    options.PreferHttpsEndpoint();
    options.AllowSelfSignedCertificates();
})
    .WithLifetime(ContainerLifetime.Persistent);

var pgServer = builder.AddPostgres("pg-server") // so is this!
    .WithLifetime(ContainerLifetime.Persistent);

var softwareDb = pgServer.AddDatabase("software-db");

var notificationApi = builder.AddProject<Projects.Notification_Api>("notification-api");

var softwareApi = builder.AddProject<Projects.Software_Api>("software-api")
    .WithReference(notificationApi)
    .WithReference(softwareDb)
    .WaitFor(softwareDb)
    .WaitFor(scalar)
    .WithExternalHttpEndpoints();


scalar.WithApiReference(softwareApi);
scalar.WithApiReference(notificationApi);


builder.Build().Run();
