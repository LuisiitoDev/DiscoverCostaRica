using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscoverCostaRica.Domain.Entities;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
namespace DiscoverCostaRica.Functions.Services;


public class CrawlerBeachService(IHttpClientFactory factory)
{
    private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";
    private const string REQUEST_URI = "https://costaricatravelblog.com/costa-rica-beaches-and-where-to-find-them/#playa-arrecife-cb";

    public async Task<IEnumerable<Beach>> FetchBeachesAsync()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        using var response = await client.GetAsync(REQUEST_URI);
        if (!response.IsSuccessStatusCode) return [];

        var content = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        List<Beach> beaches = [];

        // titles
        var titles = doc.DocumentNode.SelectNodes("//h2[contains(@class, 'wp-block-heading')]//span")
        ?.Skip(1)
        .Select(node => RemoveParathensisContent(node.InnerText))
        .Distinct();

        var descriptions = doc.DocumentNode.SelectNodes("//div[contains(@class, 'single-content')]//p[not(@class) and not(@style)]")
        ?.Skip(1)
        .Select(node => node.InnerText)
        .Distinct();

        foreach (var title in titles!)
        {
            var match = descriptions
            ?.Where(d => d.Contains(title, StringComparison.InvariantCultureIgnoreCase));
            if (match?.Any() ?? false) beaches.Add(new()
            {
                Name = title,
                Description = string.Join(".", match),
            });
        }

        return beaches;

        return [];
    }

    private static string RemoveParathensisContent(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        string pattern = @"\s*\([^)]*\)";
        return Regex.Replace(input, pattern, "").Trim();
    }
}