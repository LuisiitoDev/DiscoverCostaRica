using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DiscoverCostaRica.SourceGenerators.Generators;

[Generator(LanguageNames.CSharp)]
public class AuthorizationPolicyGenerator : IIncrementalGenerator
{
    private const string AuthorizationPolicyName = "AuthorizationPolicyAttribute";

    internal sealed class ClassPolicyInfo(INamedTypeSymbol classSymbol, ImmutableArray<(string PolicyName, string RequiredScore)> policies)
    {
        public INamedTypeSymbol ClassSymbol { get; } = classSymbol;
        public ImmutableArray<(string PolicyName, string RequiredScore)> Policies { get; } = policies;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0,
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            var (compilation, classes) = source;
            var registrations = new List<string>();
            foreach (var classInfo in classes)
            {
                    if (classInfo is null) continue;

                    var symbol = classInfo.ClassSymbol;

                    foreach (var (name, scope) in classInfo.Policies)
                    {
                        registrations.Add($@"
                                .AddPolicy(""{name}"", policy => 
                                    policy.RequireAssertion(context => 
                                        context.User.Claims
                                            .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Roles)
                                           .Any(c => c.Value.Split(' ',StringSplitOptions.RemoveEmptyEntries).Contains(""{scope}""))))");
                    }
                }

                var requiredAuthorization = registrations.Count > 0 
                    ? $"services.AddAuthorizationBuilder(){string.Concat(registrations)}"
                    : "services.AddAuthorizationBuilder()";

                var sourceText = GenerateExensionsClass(requiredAuthorization);
                spc.AddSource("ServiceRegistrationExtensions.Policies.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        });
    }

    private static string GenerateExensionsClass(string authorizations)
    {
        return $@"
            // <auto-genereated/>
            using Microsoft.AspNetCore.Builder;
            using Microsoft.Extensions.Configuration;
            using Microsoft.Extensions.DependencyInjection;
            using Microsoft.Extensions.Diagnostics.HealthChecks;
            using Microsoft.Extensions.Logging;
            using Microsoft.Identity.Web;
            using Microsoft.AspNetCore.Authorization;

            namespace DiscoverCostaRica.Generated
            {{
                public static class PoliciesRegistration
                {{
                    public static IServiceCollection AddPolicies(this IServiceCollection services)
                    {{
                        {authorizations};
                        return services;
                    }}
                }}
            }}
";
    }

    private static ClassPolicyInfo GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            return null;

        var policies = ImmutableArray.CreateBuilder<(string PolicyName, string RequiredScope)>();

        foreach (var attribute in classSymbol.GetAttributes())
        {
            var attrName = attribute.AttributeClass?.Name;

            if (attrName is not AuthorizationPolicyName) continue;

            var args = attribute.ConstructorArguments;

            if (args.Length >= 2)
            {
                var policyName = args[0].Value?.ToString();
                var scope = args[1].Value?.ToString();

                if (policyName != null && scope != null)
                {
                    policies.Add((policyName, scope));
                }
            }
        }

        if (policies.Count == 0) return null;

        return new ClassPolicyInfo(classSymbol, policies.ToImmutable());
    }
}


