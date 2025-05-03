using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using R.FastEndpoints.MessagePack.Internal;

namespace R.FastEndpoints.MessagePack;

public static class FastEndpointsResponseSerializer
{
    // Weird, yes, ignore
    private static Config Config { get; } = new();
    
    /// <summary>
    /// ResponseSerializer for MessagePack formatting (when available).
    /// </summary>
    public static Func<HttpResponse, object?, string, JsonSerializerContext?, CancellationToken, Task> MessagePack = (rsp, dto, contentType, jCtx, cancellation)
            =>
    {
        if (dto is null)
        {
            return Task.CompletedTask;
        }

        // Only override if the client accepts it.
        if (rsp.HttpContext.Request.AcceptsMsgPackContentType())
        {
            return rsp.WriteAsMsgPackAsync(dto, cancellation: cancellation);
        }
        else
        {
            return rsp.WriteAsJsonAsync(
                value: dto,
                type: dto.GetType(),
                options: jCtx?.Options ?? Config.Serializer.Options,
                contentType: contentType,
                cancellationToken: cancellation);
        }
    };
}