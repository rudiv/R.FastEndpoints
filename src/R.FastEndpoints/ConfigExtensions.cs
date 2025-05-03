using System.Collections.Frozen;
using System.Reflection;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace R.FastEndpoints;

public static class ConfigExtensions
{
    /// <summary>
    /// Configure Implicit Error Sending support, should be called within UseFastEndpoints().
    /// </summary>
    /// <example>
    ///     app.UseFastEndpoints(c => {
    ///         c.Endpoints.Configurator = ep => {
    ///             // .. some other config
    ///             ep.ConfigureImplicitErrorSending(ImplicitErrorSenders.Endpoints, /* more assemblies */);
    ///         }
    ///     });
    /// </example>
    /// <param name="def"></param>
    /// <param name="types">Types of implicit error senders</param>
    public static void ConfigureImplicitErrorSending(this EndpointDefinition def, params FrozenSet<Type>[] types)
    {
        // If it's first run, we have to use Reflection, of course, so let's go
        if (ImplicitSenderTypes == null && ErrorOptionsTypes == null)
        {
            if (types.Length == 1)
            {
                ImplicitSenderTypes = types[0];
            }
            else
            {
                var builder = new HashSet<Type>(types[0]);
                for(var i = 1; i < types.Length; i++)
                    builder.UnionWith(types[i]);
                ImplicitSenderTypes = builder.ToFrozenSet();
            }

            // Weird yes, but necessary as the statics are internal
            var errorOptions = new Config().Errors;
            // Reflection time...
            var metadataTypeProp = typeof(ErrorOptions).GetProperty(
                nameof(ErrorOptions.ProducesMetadataType),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            var contentTypeProp = typeof(ErrorOptions).GetProperty(
                nameof(ErrorOptions.ContentType),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            var statusCodeProp = typeof(ErrorOptions).GetProperty(
                nameof(ErrorOptions.StatusCode),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            var metadataType = metadataTypeProp?.GetValue(errorOptions) as Type;
            var contentType = contentTypeProp?.GetValue(errorOptions) as string;
            var statusCode = statusCodeProp?.GetValue(errorOptions) as int?;
            if (metadataType != null && contentType != null && statusCode != null)
            {
                ErrorOptionsTypes = (metadataType, contentType, statusCode.Value);
            }
        }

        if (ImplicitSenderTypes is not { Count: > 0} ||
            !ErrorOptionsTypes.HasValue ||
            !ImplicitSenderTypes.Contains(def.EndpointType))
        {
            return;
        }
        
        def.Description(b => b.Produces(
            ErrorOptionsTypes.Value.StatusCode,
            ErrorOptionsTypes.Value.MetadataType,
            ErrorOptionsTypes.Value.ContentType));
    }
    
    // Yes, we hate statics, but the reality is that FE uses them profusely so a more complex method isn't necessary
    internal static FrozenSet<Type>? ImplicitSenderTypes;
    internal static (Type MetadataType, string ContentType, int StatusCode)? ErrorOptionsTypes;
}