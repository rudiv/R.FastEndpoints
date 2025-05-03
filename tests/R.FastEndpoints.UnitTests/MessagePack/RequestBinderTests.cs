using FastEndpoints;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.MessagePack.Internal;

namespace R.FastEndpoints.UnitTests.MessagePack;

public class RequestBinderTests : HttpContextTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RequestBinder_WillThrow(bool willThrow)
    {
        var httpReq = GetHttpRequest(withContentTypeHeader: HeaderType.Valid);
        var opts = new MessagePackOptions
        {
            Resolver = StandardResolver.Instance, // To ensure it fails
            ThrowOnInvalid = willThrow
        };
        var ctx = new BinderContext
        {
            HttpContext = HttpContext!,
        };
        Assert.Throws<TypeInitializationException>(() => new MessagePackRequestBinder<object>(new NullLogger<MessagePackRequestBinder<object>>(), opts));
        /*if (willThrow)
        {
            await Assert.ThrowsAsync<MessagePackSerializationException>(async () => await binder.BindAsync(ctx, CancellationToken.None));
        }
        else
        {
            // This relies on the internal FE binder throwing a NullReferenceException
            await Assert.ThrowsAsync<NullReferenceException>(async () => await binder.BindAsync(ctx, CancellationToken.None));
        }*/
        
    }
}