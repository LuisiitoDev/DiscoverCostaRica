using DiscoverCostaRica.SourceGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.SourceGenerators;

public sealed class ServiceRegistrationGeneratorTests
{
    private const string Attributes = """
        namespace DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection
        {
            public sealed class TransientServiceAttribute : System.Attribute { }
            public sealed class DecoratorServiceAttribute : System.Attribute { }
        }
        namespace Sample
        {
            public interface IUseCase<in TRequest, TResponse> { }
            public sealed record GetProvinces;
            public sealed record ProvinceResponse(int Id, string Name);
        }
        """;

    [Fact]
    public void ClosedGenericUseCase_IsRegisteredAndDecoratedWithFullyQualifiedNames()
    {
        const string source = Attributes + """
            namespace Sample
            {
                using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
                using System.Collections.Generic;

                [TransientService]
                public sealed class GetProvincesInteractor : IUseCase<GetProvinces, IReadOnlyList<ProvinceResponse>> { }

                [DecoratorService]
                public sealed class CachedGetProvinces : IUseCase<GetProvinces, IReadOnlyList<ProvinceResponse>> { }
            }
            """;

        var (sources, diagnostics) = GeneratorHarness.Run(new ServiceRegistrationGenerator(), source);

        Assert.Empty(diagnostics);
        var generated = Assert.Single(sources);
        const string useCase = "global::Sample.IUseCase<global::Sample.GetProvinces, global::System.Collections.Generic.IReadOnlyList<global::Sample.ProvinceResponse>>";
        Assert.Contains($"services.AddTransient<{useCase}, global::Sample.GetProvincesInteractor>();", generated);
        Assert.Contains($"services.Decorate<{useCase}, global::Sample.CachedGetProvinces>();", generated);
        Assert.Contains("AddGeneratedServices_Sample_Assembly", generated);
        Assert.True(
            generated.IndexOf("AddTransient", StringComparison.Ordinal) < generated.IndexOf("Decorate", StringComparison.Ordinal),
            "Decorators must be applied after the service they wrap is registered.");
    }

    [Fact]
    public void DecoratorDerivingFromGenericBase_DecoratesTheInheritedInterface()
    {
        const string source = Attributes + """
            namespace Sample
            {
                using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;

                public abstract class CachedUseCase<TRequest, TResponse> : IUseCase<TRequest, TResponse> { }

                [DecoratorService]
                public sealed class CachedGetProvinces : CachedUseCase<GetProvinces, ProvinceResponse> { }
            }
            """;

        var (sources, diagnostics) = GeneratorHarness.Run(new ServiceRegistrationGenerator(), source);

        Assert.Empty(diagnostics);
        Assert.Contains(
            "services.Decorate<global::Sample.IUseCase<global::Sample.GetProvinces, global::Sample.ProvinceResponse>, global::Sample.CachedGetProvinces>();",
            Assert.Single(sources));
    }

    [Fact]
    public void DecoratorWithoutInterface_ReportsDcr001()
    {
        const string source = Attributes + """
            namespace Sample
            {
                [DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection.DecoratorService]
                public sealed class Orphan { }
            }
            """;

        var (sources, diagnostics) = GeneratorHarness.Run(new ServiceRegistrationGenerator(), source);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("DCR001", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Empty(sources);
    }
}
