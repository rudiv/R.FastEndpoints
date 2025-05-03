using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace R.FastEndpoints.UnitTests.Generators;

public static class GeneratorHelpers
{
    public static Compilation GetCompilationForFeSource(string src)
    {
        var ns = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "netstandard");
        var rt = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "System.Runtime");
        var coll = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "System.Collections");
        return CSharpCompilation.Create("TestAssembly",
            [
                CSharpSyntaxTree.ParseText(src, cancellationToken: TestContext.Current.CancellationToken)
            ],
            [
                MetadataReference.CreateFromFile(ns.Location),
                MetadataReference.CreateFromFile(rt.Location), 
                MetadataReference.CreateFromFile(coll.Location), 
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(FluentValidation.IValidator).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(JsonSerializerContext).Assembly.Location), 
                MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(EndpointWithoutRequest).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}