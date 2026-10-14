using System.Reflection;
using DiscoverCostaRica.ArchitectureTests.Model;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using NetArchTest.Rules;

namespace DiscoverCostaRica.ArchitectureTests.Rules;

/// <summary>The Dependency Rule at type level, checked on the compiled IL.</summary>
public sealed class TypeRules
{
    /// <summary>Frameworks and drivers that must stay outside the use-case circle (Constitution I).</summary>
    private static readonly string[] FrameworkNamespaces =
    [
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "StackExchange.Redis",
        "Refit",
        "MongoDB",
        "Azure",
        "AutoMapper",
    ];

    [Fact]
    public void Application_DoesNotUseFrameworkTypes() =>
        Baseline.Verify("application-no-framework-types",
            from project in Solution.InLayer(Layer.Application)
            from type in Failing(Types.InAssembly(Solution.LoadAssembly(project)).Should().NotHaveDependencyOnAny(FrameworkNamespaces))
            select type);

    [Fact]
    public void InnerCircles_DependOnTheBclOnly() =>
        Baseline.Verify("inner-circles-bcl-only",
            from project in Solution.Projects.Where(p => p.Layer is Layer.Domain or Layer.BuildingBlocksApplication or Layer.Contracts)
            let assembly = Solution.LoadAssembly(project)
            from type in Failing(Types.InAssembly(assembly).Should().OnlyHaveDependencyOn(["System", .. OwnNamespaces(assembly)]))
            select type);

    [Fact]
    public void Ports_AreInterfaces() =>
        Baseline.Verify("ports-are-interfaces",
            from project in Solution.InLayer(Layer.Application)
            from type in Failing(Types.InAssembly(Solution.LoadAssembly(project))
                .That().ResideInNamespaceEndingWith(".Ports")
                .Should().BeInterfaces())
            select type);

    [Fact]
    public void UseCases_LiveInTheApplicationCircle() =>
        Baseline.Verify("use-cases-in-application",
            from project in Solution.Projects.Where(p => p.Layer is not (Layer.Application or Layer.Tooling))
            from type in Solution.LoadAssembly(project).GetTypes()
            where type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUseCase<,>))
            select type.FullName!);

    private static IEnumerable<string> Failing(ConditionList conditions)
    {
        var result = conditions.GetResult();
        return result.IsSuccessful ? [] : result.FailingTypes.Select(t => t.FullName);
    }

    private static IEnumerable<string> OwnNamespaces(Assembly assembly) =>
        assembly.GetTypes().Select(t => t.Namespace).OfType<string>().Distinct();
}
