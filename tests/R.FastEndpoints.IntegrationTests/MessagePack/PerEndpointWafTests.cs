using System.Net;
using MessagePack;
using MessagePack.Resolvers;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.TestWeb.Endpoints;

namespace R.FastEndpoints.IntegrationTests.MessagePack;

public class PerEndpointWafTests : PerEndpointWafTest
{
    [Fact]
    public async Task UnconfiguredEndpoint()
    {
        var request = new MessagePackInputRequest
        {
            Test = "IO Test"
        };
        var ser = new MessagePackSerializerOptions(ContractlessStandardResolver.Instance);
        var requestBytes = MessagePackSerializer.Serialize(request, ser, TestContext.Current.CancellationToken);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Client.SendAsync(req, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, mp.StatusCode);
    }
    
    [Fact]
    public async Task ConfiguredEndpoint()
    {
        var request = new MessagePackInputRequest
        {
            Test = "IO Test"
        };
        var ser = new MessagePackSerializerOptions(ContractlessStandardResolver.Instance);
        var requestBytes = MessagePackSerializer.Serialize(request, ser, TestContext.Current.CancellationToken);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input-pe");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Client.SendAsync(req, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackInputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), ser, TestContext.Current.CancellationToken);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
}