using DiscoverCostaRica.SourceGenerators.Generators;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.SourceGenerators;

public sealed class AuthorizationPolicyGeneratorTests
{
    [Fact]
    public void Policies_AreGeneratedFromAttributes_AndCheckTheRolesClaim()
    {
        const string source = """
            public sealed class AuthorizationPolicyAttribute(string policyName, string requiredScope) : System.Attribute { }

            [AuthorizationPolicy("Geo.Read", "Geo.Read")]
            public sealed class Policies { }
            """;

        var (sources, diagnostics) = GeneratorHarness.Run(new AuthorizationPolicyGenerator(), source);

        Assert.Empty(diagnostics);
        var generated = Assert.Single(sources);
        Assert.Contains(".AddPolicy(\"Geo.Read\"", generated);
        Assert.Contains("global::DiscoverCostaRica.BuildingBlocks.Web.Authentication.AuthConstants.ClaimTypes.Roles", generated);
    }

    [Fact]
    public void AssemblyWithoutPolicies_GetsNoRegistrationClass()
    {
        var (sources, _) = GeneratorHarness.Run(new AuthorizationPolicyGenerator(), "public sealed class Plain { }");

        Assert.Empty(sources);
    }
}
