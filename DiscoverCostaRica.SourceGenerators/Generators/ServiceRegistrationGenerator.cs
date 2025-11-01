using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoverCostaRica.SourceGenerators.Generators;

[Generator(LanguageNames.CSharp)]
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclaration = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static cls => cls.AttributeLists.Count > 0);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclaration.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            var (compilation, classList) = source;
            var registrations = new List<string>();

            foreach (var classDeclarationSyntax in classList)
            {
                var model = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;

                if (classSymbol == null) continue;

                foreach (var attributeData in classSymbol.GetAttributes())
                {
                    var attributeName = attributeData.AttributeClass?.Name;
                    string registration = attributeName switch
                    {
                        "TransientServiceAttribute" => $"services.AddTransient<{classSymbol.Interfaces.FirstOrDefault()?.ToDisplayString() ?? classSymbol.Name},{classSymbol.ToDisplayString()}>();",
                        "ScopedServiceAttribute" => $"services.AddScoped<{classSymbol.Interfaces.FirstOrDefault()?.ToDisplayString() ?? classSymbol.Name},{classSymbol.ToDisplayString()}>();",
                        "SingletonServiceAttribute" => $"services.AddSingleton<{classSymbol.Interfaces.FirstOrDefault()?.ToDisplayString() ?? classSymbol.Name},{classSymbol.ToDisplayString()}>();",
                        _ => null
                    };

                    if (registration != null)
                    {
                        registrations.Add(registration);
                    }
                }

                var sourceText = $@"
                    using Microsoft.Extensions.DependencyInjection;

                    namespace DiscoverCostaRica.Generated
                    {{
                        public static class ServiceRegistrationExtensions
                        {{
                            public static IServiceCollection AddGeneratedServices(this IServiceCollection services)
                            {{
                                // Auto-generated registration code
                                {string.Join("\n            ", registrations)}
                                return services;
                            }}
                        }}
                    }}
                ";

                spc.AddSource("ServiceRegistrationExtensions.g.cs", SourceText.From(sourceText, Encoding.UTF8));
            }
        });
    }
}
