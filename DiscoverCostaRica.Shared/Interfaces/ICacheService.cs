namespace DiscoverCostaRica.Shared.Interfaces
{
    public interface ICacheService
    {
        Task Set<TSource>(string key, TSource source, CancellationToken cancellationToken);
        Task<TSource?> Get<TSource>(string key, CancellationToken cancellationToken);
    }
}
