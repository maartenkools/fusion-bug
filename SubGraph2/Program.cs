using SubGraph2;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGraphQL();
app.Run();