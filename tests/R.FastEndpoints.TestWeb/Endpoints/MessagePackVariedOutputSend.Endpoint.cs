using FastEndpoints;

namespace R.FastEndpoints.TestWeb.Endpoints;

public class MessagePackVariedOutputSendEndpoint : EndpointWithoutRequest<MessagePackOutputResponse>
{
    public override void Configure()
    {
        Get("/vary-output-send");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new MessagePackOutputResponse
        {
            Test = "Hello World!"
        };
        await SendAsync(response, cancellation: ct);
    }
}