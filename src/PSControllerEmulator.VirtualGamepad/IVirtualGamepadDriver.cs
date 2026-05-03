using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.VirtualGamepad;

public interface IVirtualGamepadDriver
{
    Task<VirtualGamepadSession> ConnectAsync(
        VirtualControllerTarget target,
        CancellationToken cancellationToken = default);

    Task SubmitInputAsync(
        VirtualGamepadSession session,
        ControllerInputState inputState,
        CancellationToken cancellationToken = default);

    Task DisconnectAsync(
        VirtualGamepadSession session,
        CancellationToken cancellationToken = default);
}
