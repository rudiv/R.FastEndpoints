# R.FastEndpoints Extensions

## Usage

Add `R.FastEndpoints` and `R.FastEndpoints.Generators` from NuGet.

## What

A (hopefully expanding) set of tools for some additional functionality in [FastEndpoints](https://fast-endpoints.com).

### Implicit Error Sending Support

When an endpoint calls `SendErrorsAsync`, `ThrowError` or `ThrowIfAnyErrors` (up to 2 levels deep to allow for 
extensions or other methods), this configures Swagger to have the requisite information.

When calling UseFastEndpoints, call the `ConfigureImplicitErrorSending` method in your `Configurator`, passing in the
appropriate generated types from each assembly where you have Endpoints.

```csharp
app.UseFastEndpoints(c => {
    c.Endpoints.Configurator = ep => {
        // .. some other config
        ep.ConfigureImplicitErrorSending(ImplicitErrorSenders.Endpoints, /* more assemblies */);
    };
});
```

### MessagePack Support

Add `Rudi.Dev.FastEndpoints.MessagePack` from NuGet.

To add support for input bindings globally, you need to call `.AddMessagePackBinding()` before `.AddFastEndpoints()`, and add `.ConfigureInboundMessagePack()` to a global configurator within FastEndpoints.

For example:
```csharp
builder.Services
    .AddMessagePackBinding()
    .AddFastEndpoints();

// ...

app.UseFastEndpoints(c => {
    c.Endpoints.Configurator = ep => {
        // .. some other config
        ep.ConfigureInboundMessagePack();
    };
});
```

For more documentation, see [R.FastEndpoints.MessagePack readme](./src/R.FastEndpoints.MessagePack/README.md).