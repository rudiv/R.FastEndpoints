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
        var requestBytes = MessagePackSerializer.Serialize(request, ser);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Client.SendAsync(req);
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
        var requestBytes = MessagePackSerializer.Serialize(request, ser);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input-pe");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackInputResponse>(await mp.Content.ReadAsStreamAsync(), ser);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
}