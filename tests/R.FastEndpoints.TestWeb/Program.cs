using System.Runtime.CompilerServices;
using System.Text.Json;
using FastEndpoints;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.TestWeb;

[assembly: InternalsVisibleTo("R.FastEndpoints.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Services.GetService<ShouldBeGlobal>() != null)
{
    app.UseFastEndpoints(o =>
    {
        o.Serializer.ResponseSerializer = FastEndpointsResponseSerializer.MessagePack;
        o.Endpoints.Configurator = ep => ep.ConfigureInboundMessagePack();
    });
}
else
{
    // FE internal Config is static, so we need to reset them
    app.UseFastEndpoints(o =>
    {
        o.Serializer.ResponseSerializer = (rsp, dto, contentType, jCtx, cancellation) =>
        {
            return dto is null
                ? Task.CompletedTask
                : rsp.WriteAsJsonAsync(
                    value: dto,
                    type: dto.GetType(),
                    options: jCtx?.Options ?? new JsonSerializerOptions(),
                    contentType: contentType,
                    cancellationToken: cancellation);
        };
        o.Endpoints.Configurator = null;
    });
}

app.Run();

public partial class Program { }