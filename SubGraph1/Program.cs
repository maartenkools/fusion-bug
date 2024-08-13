using SubGraph1;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<DegreeProgramme>()
    .AddType<AdministrativeProgramme>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGraphQL();
app.Run();
