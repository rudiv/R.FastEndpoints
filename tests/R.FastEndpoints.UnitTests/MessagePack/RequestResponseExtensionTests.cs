using Microsoft.Extensions.DependencyInjection;
using R.FastEndpoints.MessagePack;
using R.FastEndpoints.MessagePack.Internal;

namespace R.FastEndpoints.UnitTests.MessagePack;

public class RequestResponseExtensionTests : HttpContextTestBase
{
    [Theory]
    [InlineData(HeaderType.NotSet)]
    [InlineData(HeaderType.Valid)]
    [InlineData(HeaderType.Invalid)]
    public void HttpRequest_AcceptsMsgPackContentType_Functioning(HeaderType acceptHeader)
    {
        var httpReq = GetHttpRequest(acceptHeader);
        Assert.Equal(acceptHeader == HeaderType.Valid, httpReq.AcceptsMsgPackContentType());
    }
    
    [Theory]
    [InlineData(HeaderType.NotSet)]
    [InlineData(HeaderType.Valid)]
    [InlineData(HeaderType.Invalid)]
    public void HttpRequest_HasMsgPackContentType_Functioning(HeaderType contentTypeHeader)
    {
        var httpReq = GetHttpRequest(withContentTypeHeader: contentTypeHeader);
        Assert.Equal(contentTypeHeader == HeaderType.Valid, httpReq.HasMsgPackContentType());
    }
    
    [Theory]
    [InlineData(HeaderType.NotSet)]
    [InlineData(HeaderType.Valid)]
    [InlineData(HeaderType.Invalid)]
    public async Task HttpResponse_WriteWithMsgPackAsync_Negotiating(HeaderType acceptHeader)
    {
        GetHttpRequest(acceptHeader);
        var httpResp = HttpContext.Response;
        await httpResp.WriteWithMsgPackAsync(new { });
        if (acceptHeader == HeaderType.Valid)
        {
            Assert.Equal(MessagePackConstants.ContentType, httpResp.ContentType);
        }
        else
        {
            Assert.Equal("application/json; charset=utf-8", httpResp.ContentType);
        }
    }
    
    [Fact]
    public async Task HttpResponse_WriteAsMsgPackAsync_AlternativeResponseContentType()
    {
        GetHttpRequest();
        HttpContext!.RequestServices.GetRequiredService<MessagePackOptions>().DefaultResponseHeader = MessagePackConstants.VndContentType;
        var httpResp = HttpContext.Response;
        await httpResp.WriteAsMsgPackAsync(new { });
        Assert.Equal(MessagePackConstants.VndContentType, httpResp.ContentType);
    }
}