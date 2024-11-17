using HtmlAgilityPack;

namespace DiscoverCostaRica.Functions.Services;

public class VolcanoService
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<Dictionary<string, string>> StartCrawler()
    {
        string url = "https://costarica.org/es/volcanes/";
        client.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, como Gecko) Chrome/58.0.3029.110 Safari/537.3");
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        return GetVolcanoInformationFromHtml(content);
    }


    private static Dictionary<string,string> GetVolcanoInformationFromHtml(string html)
    {
        var output = new Dictionary<string, string>();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var titles = htmlDoc.DocumentNode.SelectNodes("//div[@id='below_the_fold']//h2");
        var paragraphs = htmlDoc.DocumentNode.SelectNodes("//div[@id='below_the_fold']//p");

        for(var i = 0; i < titles.Count && i < paragraphs.Count; i++)
        {
            output[titles[i].InnerText] = paragraphs[i].InnerText;
        }

        return output;
    }
}
