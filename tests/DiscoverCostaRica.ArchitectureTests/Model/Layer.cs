namespace DiscoverCostaRica.ArchitectureTests.Model;

/// <summary>Clean Architecture circle a project belongs to.</summary>
public enum Layer
{
    /// <summary>Entities: enterprise rules.</summary>
    Domain,

    /// <summary>Use cases: application rules and the ports they own.</summary>
    Application,

    /// <summary>Frameworks and drivers: gateway implementations.</summary>
    Infrastructure,

    /// <summary>Interface adapters (endpoints, presenters) and the Main component.</summary>
    Api,

    BuildingBlocksApplication,
    BuildingBlocksWeb,
    BuildingBlocksInfrastructure,
    Contracts,
    ServiceDefaults,
    LegacyShared,
    Tooling,
}
