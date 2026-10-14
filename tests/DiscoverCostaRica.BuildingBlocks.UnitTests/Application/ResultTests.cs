using DiscoverCostaRica.BuildingBlocks.Application.Results;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.Application;

public sealed class ResultTests
{
    [Fact]
    public void Value_ConvertsToSuccess()
    {
        Result<string> result = "Arenal";

        Assert.True(result.IsSuccess);
        Assert.Equal("Arenal", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Error_ConvertsToFailure()
    {
        Result<string> result = Error.NotFound("Volcano was not found");

        Assert.False(result.IsSuccess);
        Assert.Equal(new Error(ErrorType.NotFound, "Volcano was not found"), result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Success_RejectsNull() =>
        Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));

    [Fact]
    public void Match_RunsTheBranchOfTheOutcome()
    {
        Result<int> success = 7;
        Result<int> failure = Error.Validation("bad");

        Assert.Equal("value 7", success.Match(v => $"value {v}", e => e.Message));
        Assert.Equal("bad", failure.Match(v => $"value {v}", e => e.Message));
    }

    [Fact]
    public void Map_TransformsSuccessAndKeepsFailure()
    {
        Result<int> success = 7;
        Result<int> failure = Error.NotFound("missing");

        Assert.Equal("7", success.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture)).Value);
        Assert.Equal(Error.NotFound("missing"), failure.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture)).Error);
    }
}
