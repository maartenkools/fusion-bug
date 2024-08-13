var builder = DistributedApplication.CreateBuilder(args);

var sg1 = builder.AddProject<Projects.SubGraph1>("subgraph1");
var sg2 = builder.AddProject<Projects.SubGraph2>("subgraph2");

builder.AddProject<Projects.Gateway>("gateway")
    .WithReference(sg1)
    .WithReference(sg2);

builder.Build().Run();
