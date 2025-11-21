using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver()
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(async context =>
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                const string headerName = "Bearer ";
                var tokenValue = token.ToString()[headerName.Length..].Trim();
                context.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenValue);
            }
        });
    });

var app = builder.Build();

app.MapReverseProxy();

app.Run();