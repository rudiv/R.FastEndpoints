using System.Net;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.TestWeb.Endpoints;

namespace R.FastEndpoints.IntegrationTests.MessagePack;

public class InputOutputFormatWafTests : GlobalWafTest
{
    [Fact]
    public async Task TestInputOutput()
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
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackInputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), ser, TestContext.Current.CancellationToken);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
    
    [Fact]
    public async Task TestOutputAsMessagePack()
    {
        var mp = await Client.GetByteArrayAsync("mp-output", TestContext.Current.CancellationToken);
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(mp, new MessagePackSerializerOptions(ContractlessStandardResolver.Instance), TestContext.Current.CancellationToken);
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestOutputWithMessagePack(bool withAcceptsHeader)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output");
        if (withAcceptsHeader)
        {
            req.Headers.Add("Accept", MessagePackConstants.ContentType);
        }
        var mp = await Client.SendAsync(req, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        MessagePackOutputResponse? response;
        if (withAcceptsHeader)
        {
            Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
            response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance), TestContext.Current.CancellationToken);
        }
        else
        {
            Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
            response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});            
        }

        Assert.NotNull(response);
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestSendOutputWithMessagePack(bool withAcceptsHeader)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output-send");
        if (withAcceptsHeader)
        {
            req.Headers.Add("Accept", MessagePackConstants.ContentType);
        }
        var mp = await Client.SendAsync(req, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        MessagePackOutputResponse? response;
        if (withAcceptsHeader)
        {
            Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
            response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance), TestContext.Current.CancellationToken);
        }
        else
        {
            Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
            response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});            
        }

        Assert.NotNull(response);
        Assert.Equal("Hello World!", response.Test);
    }
    
}