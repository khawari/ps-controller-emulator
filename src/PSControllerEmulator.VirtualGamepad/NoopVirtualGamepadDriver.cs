using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.VirtualGamepad;

public sealed class NoopVirtualGamepadDriver : IVirtualGamepadDriver
{
    public Task<VirtualGamepadSession> ConnectAsync(
        VirtualControllerTarget target,
        CancellationToken cancellationToken = default)
    {
        var session = new VirtualGamepadSession(
            Guid.NewGuid().ToString("N"),
            target,
            IsActive: false,
            "No virtual gamepad backend is configured.");

        return Task.FromResult(session);
    }

    public Task SubmitInputAsync(
        VirtualGamepadSession session,
        ControllerInputState inputState,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(
        VirtualGamepadSession session,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
