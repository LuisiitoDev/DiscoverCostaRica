namespace DiscoverCostaRica.ArchitectureTests.Model;

/// <summary>
/// Known violations that existed when the rules were introduced (spec 006 US1).
/// The baseline is a ratchet: new violations fail, and so do fixed violations that are still listed,
/// so the file can only shrink until every service is migrated.
/// </summary>
public static class Baseline
{
    private const string FileName = "architecture-baseline.txt";

    private static readonly Lazy<ILookup<string, string>> Entries = new(Load);

    public static void Verify(string ruleId, IEnumerable<string> violations)
    {
        var current = violations.ToHashSet(StringComparer.Ordinal);
        var known = Entries.Value[ruleId].ToHashSet(StringComparer.Ordinal);

        var introduced = current.Except(known).Order(StringComparer.Ordinal).ToList();
        var resolved = known.Except(current).Order(StringComparer.Ordinal).ToList();

        var messages = new List<string>();
        if (introduced.Count > 0)
        {
            messages.Add($"Rule '{ruleId}' has new violations. Fix them; do not add them to {FileName}:");
            messages.AddRange(introduced.Select(v => $"  + {v}"));
        }

        if (resolved.Count > 0)
        {
            messages.Add($"Rule '{ruleId}' violations were fixed. Remove these lines from {FileName} to lock the gain in:");
            messages.AddRange(resolved.Select(v => $"  - {ruleId} | {v}"));
        }

        Assert.True(messages.Count == 0, string.Join(Environment.NewLine, messages));
    }

    private static ILookup<string, string> Load() =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, FileName))
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .Select(line => line.Split('|', 2, StringSplitOptions.TrimEntries))
            .ToLookup(parts => parts[0], parts => parts[1], StringComparer.Ordinal);
}
