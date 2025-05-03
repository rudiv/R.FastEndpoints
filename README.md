# R.FastEndpoints Extensions

A series of libraries to add additional functionality to [FastEndpoints](https://fast-endpoints.com).

Anything else that would help you but doesn't fit in FE core? Raise an issue.

## Packages

- **Core Extensions** [![R.FastEndpoints](https://img.shields.io/nuget/v/R.FastEndpoints?style=for-the-badge&logo=nuget&label=R.FastEndpoints&labelColor=darkblue&color=lightgreen)](https://nuget.org/packages/R.FastEndpoints)
- **Source Generators** [![R.FastEndpoints.Generators](https://img.shields.io/nuget/v/R.FastEndpoints.Generators?style=for-the-badge&logo=nuget&label=R.FastEndpoints&labelColor=darkblue&color=lightgreen)](https://nuget.org/packages/R.FastEndpoints.Generators)
- **MessagePack Support** [![R.FastEndpoints.MessagePack](https://img.shields.io/nuget/v/R.FastEndpoints.MessagePack?style=for-the-badge&logo=nuget&label=R.FastEndpoints&labelColor=darkblue&color=lightgreen)](https://nuget.org/packages/R.FastEndpoints.MessagePack)

## Functionality

### Implicit Error Sending Support

Add `R.FastEndpoints` and `R.FastEndpoints.Generators` from NuGet.

When an endpoint calls `SendErrorsAsync`, `ThrowError` or `ThrowIfAnyErrors` (up to 2 levels deep to allow for extensions or other methods), this configures Swagger to have the requisite information which wouldn't otherwise be present without manually describing your Endpoint.

When calling UseFastEndpoints, call the `ConfigureImplicitErrorSending` method in your `Configurator`, passing in the appropriate generated types from each assembly where you have Endpoints.

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

There is [more MessagePack documentation](./src/R.FastEndpoints.MessagePack/README.md) (incl. Troubleshooting).
