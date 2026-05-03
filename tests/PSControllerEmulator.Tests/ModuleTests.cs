using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Core.Modules;
using PSControllerEmulator.VirtualGamepad;

namespace PSControllerEmulator.Tests;

public sealed class ModuleTests
{
    [Fact]
    public void RemappingProfileMapsPressedButtons()
    {
        var profile = new RemappingProfile();
        profile.MapButton("Cross", "Circle");
        var input = ControllerInputState.Neutral with { PressedButtons = ["Cross", "Square"] };

        var mapped = profile.Apply(input);

        Assert.Equal(["Circle", "Square"], mapped.PressedButtons);
    }

    [Fact]
    public async Task NoopVirtualGamepadDriverReportsInactiveSession()
    {
        var driver = new NoopVirtualGamepadDriver();

        var session = await driver.ConnectAsync(VirtualControllerTarget.Xbox360);

        Assert.False(session.IsActive);
        Assert.Equal(VirtualControllerTarget.Xbox360, session.Target);
    }
}
