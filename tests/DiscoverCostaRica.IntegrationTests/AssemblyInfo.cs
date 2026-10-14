using DiscoverCostaRica.Testing.Containers;

[assembly: AssemblyFixture(typeof(SqlServerFixture))]
[assembly: AssemblyFixture(typeof(RedisFixture))]
[assembly: Trait("Category", "Integration")]
