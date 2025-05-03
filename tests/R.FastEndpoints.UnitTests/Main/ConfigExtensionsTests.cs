using System.Collections.Frozen;
using System.Reflection;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;

namespace R.FastEndpoints.UnitTests.Main;

public class ConfigExtensionsTests
{
    private static readonly PropertyInfo MetadataTypeProp = typeof(EndpointDefinition).GetProperty(
        "UserConfigAction",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    )!;
    private static readonly Func<EndpointDefinition, Action<RouteHandlerBuilder>?> GetAction = (ep) => MetadataTypeProp.GetValue(ep) as Action<RouteHandlerBuilder>;

    public ConfigExtensionsTests()
    {
        new Config().Errors.ProducesMetadataType = typeof(ErrorResponse);
        ConfigExtensions.ErrorOptionsTypes = null;
        ConfigExtensions.ImplicitSenderTypes = null;
    }
    
    [Fact]
    public void ConfigureImplicitErrorSending_WithNoValidTarget_AddsNothing()
    {
        var epDef = new EndpointDefinition(typeof(FakeEndpoint), typeof(EmptyRequest), typeof(EmptyResponse));
        epDef.ConfigureImplicitErrorSending(FrozenSet<Type>.Empty);
        Assert.Null(GetAction(epDef));
    }
    
    
    [Fact]
    public void ConfigureImplicitErrorSending_WithNoErrOpts_AddsNothing()
    {
        new Config().Errors.ProducesMetadataType = null;
        var epDef = new EndpointDefinition(typeof(FakeEndpoint), typeof(EmptyRequest), typeof(EmptyResponse));
        epDef.ConfigureImplicitErrorSending(new HashSet<Type> { typeof(FakeEndpoint) }.ToFrozenSet());
        Assert.Null(GetAction(epDef));
    }

    [Fact]
    public void ConfigureImplicitErrorSending_WithValidTargetAndErrOpts_AddsDescriber()
    {
        var epDef = new EndpointDefinition(typeof(FakeEndpoint), typeof(EmptyRequest), typeof(EmptyResponse));
        epDef.ConfigureImplicitErrorSending(new HashSet<Type> { typeof(FakeEndpoint) }.ToFrozenSet());
        Assert.NotNull(GetAction(epDef));
    }
}

public class FakeEndpoint
{
}