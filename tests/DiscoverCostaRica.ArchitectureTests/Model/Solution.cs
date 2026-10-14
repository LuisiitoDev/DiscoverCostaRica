using System.Reflection;
using System.Xml.Linq;

namespace DiscoverCostaRica.ArchitectureTests.Model;

/// <summary>The production projects of the repository, read straight from their .csproj files.</summary>
public static class Solution
{
    public const string Prefix = "DiscoverCostaRica.";

    private static readonly Lazy<string> LazyRoot = new(FindRoot);
    private static readonly Lazy<IReadOnlyList<ProjectInfo>> LazyProjects = new(LoadProjects);

    public static string Root => LazyRoot.Value;

    public static IReadOnlyList<ProjectInfo> Projects => LazyProjects.Value;

    public static IEnumerable<ProjectInfo> InLayer(Layer layer) => Projects.Where(p => p.Layer == layer);

    public static ProjectInfo Get(string name) => Projects.Single(p => p.Name == name);

    public static Assembly LoadAssembly(ProjectInfo project) => Assembly.Load(new AssemblyName(project.Name));

    private static IReadOnlyList<ProjectInfo> LoadProjects()
    {
        var files = Directory.EnumerateFiles(Path.Combine(Root, "src"), "*.csproj", SearchOption.AllDirectories)
            .Append(Path.Combine(Root, "DiscoverCostaRica.ServiceDefaults", "DiscoverCostaRica.ServiceDefaults.csproj"))
            .Where(File.Exists);

        return [.. files.Select(Read).OrderBy(p => p.Name, StringComparer.Ordinal)];
    }

    private static ProjectInfo Read(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var xml = XDocument.Load(path);

        var projectReferences = xml.Descendants("ProjectReference")
            .Where(r => (string?)r.Attribute("OutputItemType") != "Analyzer")
            .Select(r => Path.GetFileNameWithoutExtension(((string)r.Attribute("Include")!).Replace('\\', '/')))
            .ToList();

        var packageReferences = xml.Descendants("PackageReference")
            .Select(r => (string)r.Attribute("Include")!)
            .ToList();

        return new ProjectInfo(name, ServiceOf(path), LayerOf(name), projectReferences, packageReferences);
    }

    private static string? ServiceOf(string path)
    {
        var relative = Path.GetRelativePath(Path.Combine(Root, "src"), path).Replace('\\', '/');
        var folder = relative.Split('/')[0];
        return folder.StartsWith(Prefix, StringComparison.Ordinal) ? folder[Prefix.Length..] : null;
    }

    private static Layer LayerOf(string name) => name switch
    {
        "DiscoverCostaRica.BuildingBlocks.Application" => Layer.BuildingBlocksApplication,
        "DiscoverCostaRica.BuildingBlocks.Web" => Layer.BuildingBlocksWeb,
        "DiscoverCostaRica.BuildingBlocks.Infrastructure" => Layer.BuildingBlocksInfrastructure,
        "DiscoverCostaRica.ServiceDefaults" => Layer.ServiceDefaults,
        "DiscoverCostaRica.Shared" => Layer.LegacyShared,
        "DiscoverCostaRica.SourceGenerators" => Layer.Tooling,
        _ when name.StartsWith("DiscoverCostaRica.Contracts.", StringComparison.Ordinal) => Layer.Contracts,
        _ when name.EndsWith(".Domain", StringComparison.Ordinal) => Layer.Domain,
        _ when name.EndsWith(".Application", StringComparison.Ordinal) => Layer.Application,
        _ when name.EndsWith(".Infrastructure", StringComparison.Ordinal) => Layer.Infrastructure,
        _ when name.EndsWith(".Api", StringComparison.Ordinal) => Layer.Api,
        _ => throw new InvalidOperationException($"Project '{name}' does not follow the layer naming convention."),
    };

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "DiscoverCostaRica.sln")))
                return directory.FullName;
        }

        throw new InvalidOperationException("Could not find the repository root (DiscoverCostaRica.sln).");
    }
}
