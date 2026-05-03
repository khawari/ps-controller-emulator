using PSControllerEmulator.Cli;

namespace PSControllerEmulator.Tests;

public sealed class CliTests
{
    [Fact]
    public async Task DevicesMockPrintsOneMockDualSense()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var exitCode = await CliRunner.RunAsync(["devices", "--mock"], output, error);

        var text = output.ToString();
        Assert.Equal(0, exitCode);
        Assert.Contains("PS Controller Emulator devices", text);
        Assert.Contains("Mock DualSense", text);
        Assert.Contains("Connection: Mock USB", text);
        Assert.DoesNotContain("No supported controllers detected", text);
        Assert.Equal(string.Empty, error.ToString());
    }
}
