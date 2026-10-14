using System.Text.RegularExpressions;
using DiscoverCostaRica.ArchitectureTests.Model;

namespace DiscoverCostaRica.ArchitectureTests.Rules;

/// <summary>
/// The Dependency Rule at component level, read from the .csproj files. Reading the project files
/// (not compiled metadata) also catches references the compiler drops because nothing uses them yet.
/// </summary>
public sealed partial class ProjectGraphRules
{
    private const string BuildingBlocksApplication = "DiscoverCostaRica.BuildingBlocks.Application";
    private const string BuildingBlocksInfrastructure = "DiscoverCostaRica.BuildingBlocks.Infrastructure";
    private const string LegacyShared = "DiscoverCostaRica.Shared";

    /// <summary>Policy-neutral abstractions an Application project may use (Constitution V).</summary>
    [GeneratedRegex(@"^Microsoft\.Extensions\.[A-Za-z.]+\.Abstractions$")]
    private static partial Regex AbstractionPackage();

    /// <summary>
    /// Scrutor is needed by the generated DI registration for decorators (Decorate&lt;,&gt;).
    /// Accepted deviation, tracked in plan.md Complexity Tracking.
    /// </summary>
    private static readonly HashSet<string> ApplicationPackageExceptions = ["Scrutor"];

    [Fact]
    public void Domain_ReferencesNothing() =>
        Baseline.Verify("domain-references-nothing",
            from project in Solution.InLayer(Layer.Domain)
            from reference in project.ProjectReferences.Where(r => r != LegacyShared).Concat(project.PackageReferences)
            select Edge(project, reference));

    [Fact]
    public void Application_ReferencesOnlyItsDomainAndPureBuildingBlocks() =>
        Baseline.Verify("application-allowed-references",
            from project in Solution.InLayer(Layer.Application)
            from reference in Disallowed(project,
                allowedProject: r => IsOwn(project, r, Layer.Domain) || r == BuildingBlocksApplication || IsContract(r),
                allowedPackage: p => AbstractionPackage().IsMatch(p) || ApplicationPackageExceptions.Contains(p))
            select Edge(project, reference));

    [Fact]
    public void Infrastructure_PointsInward() =>
        Baseline.Verify("infrastructure-allowed-references",
            from project in Solution.InLayer(Layer.Infrastructure)
            from reference in Disallowed(project,
                allowedProject: r => IsOwn(project, r, Layer.Domain) || IsOwn(project, r, Layer.Application)
                    || r is BuildingBlocksApplication or BuildingBlocksInfrastructure || IsContract(r),
                allowedPackage: _ => true)
            select Edge(project, reference));

    [Fact]
    public void Services_ReferenceEachOtherOnlyThroughContracts() =>
        Baseline.Verify("no-cross-service-references",
            from project in Solution.Projects.Where(p => p.Service is not null)
            from reference in project.ProjectReferences
            let target = Solution.Projects.SingleOrDefault(p => p.Name == reference)
            where target?.Service is not null && target.Service != project.Service
            select Edge(project, reference));

    [Fact]
    public void SharedComponents_DoNotDependOnServices() =>
        Baseline.Verify("shared-components-independent-of-services",
            from project in Solution.Projects.Where(p => p.Service is null)
            from reference in project.ProjectReferences
            where Solution.Projects.Any(p => p.Name == reference && p.Service is not null)
            select Edge(project, reference));

    [Fact]
    public void PureComponents_HaveNoDependencies() =>
        Baseline.Verify("pure-components-reference-nothing",
            from project in Solution.Projects.Where(p => p.Layer is Layer.BuildingBlocksApplication or Layer.Contracts)
            from reference in project.ProjectReferences.Concat(project.PackageReferences)
            select Edge(project, reference));

    [Fact]
    public void LegacySharedLibrary_IsNotReferenced() =>
        Baseline.Verify("no-legacy-shared",
            from project in Solution.Projects
            where project.ProjectReferences.Contains(LegacyShared)
            select Edge(project, LegacyShared));

    private static IEnumerable<string> Disallowed(ProjectInfo project, Func<string, bool> allowedProject, Func<string, bool> allowedPackage) =>
        project.ProjectReferences
            .Where(r => r != LegacyShared && !IsOtherService(project, r) && !allowedProject(r))
            .Concat(project.PackageReferences.Where(p => !allowedPackage(p)));

    private static bool IsOwn(ProjectInfo project, string reference, Layer layer) =>
        Solution.Projects.Any(p => p.Name == reference && p.Service == project.Service && p.Layer == layer);

    // Reported once, by Services_ReferenceEachOtherOnlyThroughContracts.
    private static bool IsOtherService(ProjectInfo project, string reference) =>
        Solution.Projects.Any(p => p.Name == reference && p.Service is not null && p.Service != project.Service);

    private static bool IsContract(string reference) =>
        reference.StartsWith("DiscoverCostaRica.Contracts.", StringComparison.Ordinal);

    private static string Edge(ProjectInfo project, string reference) => $"{project.Name} -> {reference}";
}
