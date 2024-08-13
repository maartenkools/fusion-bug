using HotChocolate.Fusion.Composition;
using HotChocolate.Language;
using HotChocolate.Skimmed.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<SchemaObservable>();

builder
    .Services
    .AddFusionGatewayServer()
    .RegisterGatewayConfiguration(sp => sp.GetRequiredService<SchemaObservable>());

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGraphQL();
app.Run();

internal class SchemaObservable : IObservable<GatewayConfiguration>
{
    private readonly IHttpClientFactory _clientFactory;

    public SchemaObservable(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public IDisposable Subscribe(IObserver<GatewayConfiguration> observer)
    {
        var session = new RemoteConfigurationSession(observer, _clientFactory.CreateClient());
        Task.Run(session.Observe);

        return session;
    }

    private sealed class RemoteConfigurationSession : IDisposable
    {
        private readonly IObserver<GatewayConfiguration> _observer;
        private readonly HttpClient _client;

        public RemoteConfigurationSession(IObserver<GatewayConfiguration> observer, HttpClient client)
        {
            _observer = observer;
            _client = client;
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public async Task Observe()
        {
            var configs = new List<SubgraphConfiguration>();

            string[] subgraphNames = ["subgraph1", "subgraph2"];

            foreach (string name in subgraphNames)
            {
                string url = $"http://{name}/graphql";
                var response = await _client.GetAsync($"{url}?sdl");
                response.EnsureSuccessStatusCode();
                
                string content = await response.Content.ReadAsStringAsync();

                var clientConfig = new HttpClientConfiguration(new Uri(url), "Fusion");
                var config = new SubgraphConfiguration(name, content, "", clientConfig, null);
                
                configs.Add(config);
            }
            
            var composer = new FusionGraphComposer();
            var result = await composer.ComposeAsync(configs);
            
            var fusionGraphDoc = Utf8GraphQLParser.Parse(SchemaFormatter.FormatAsString(result));
            
            _observer.OnNext(new GatewayConfiguration(fusionGraphDoc));
        }
    }
}

