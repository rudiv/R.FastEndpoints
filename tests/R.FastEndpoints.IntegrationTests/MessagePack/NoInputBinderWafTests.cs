using System.Net;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.TestWeb.Endpoints;

namespace R.FastEndpoints.IntegrationTests.MessagePack;

public class NoInputBinderWafTests : NoInputBinderWafTest
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
        Assert.Equal(HttpStatusCode.BadRequest, mp.StatusCode);
        Assert.Equal("application/problem+json", mp.Content.Headers.ContentType?.ToString());
        
        var resp = JsonSerializer.Deserialize<TempErrors>(await mp.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Assert.NotNull(resp);
        Assert.Equal("test", resp.Errors.First().Key);
        Assert.Equal("'test' must not be empty.", resp.Errors.First().Value.First());
    }

    public class TempErrors
    {
        public Dictionary<string, List<string>> Errors { get; set; } = [];
    }
}