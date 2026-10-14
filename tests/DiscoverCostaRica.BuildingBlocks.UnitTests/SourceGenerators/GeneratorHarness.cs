using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.SourceGenerators;

internal static class GeneratorHarness
{
    private static readonly ImmutableArray<MetadataReference> References =
    [
        .. ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path)),
    ];

    public static (IReadOnlyList<string> Sources, IReadOnlyList<Diagnostic> Diagnostics) Run(IIncrementalGenerator generator, string source)
    {
        var compilation = CSharpCompilation.Create(
            "Sample.Assembly",
            [CSharpSyntaxTree.ParseText(source)],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(generator).RunGenerators(compilation, TestContext.Current.CancellationToken);
        var result = driver.GetRunResult();

        return (
            result.GeneratedTrees.Select(tree => tree.ToString()).ToList(),
            result.Diagnostics.ToList());
    }
}
