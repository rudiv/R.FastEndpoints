using MessagePack;

namespace R.FastEndpoints.TestWeb.Endpoints;

[MessagePackObject]
public class MessagePackStandardOutputResponse
{
    [Key(0)]
    public string Test { get; set; }
}