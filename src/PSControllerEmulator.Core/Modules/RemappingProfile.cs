using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.Core.Modules;

public sealed class RemappingProfile
{
    private readonly Dictionary<string, string> _buttonMap = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, string> ButtonMap => _buttonMap;

    public void MapButton(string sourceButton, string targetButton)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceButton);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetButton);

        _buttonMap[sourceButton.Trim()] = targetButton.Trim();
    }

    public ControllerInputState Apply(ControllerInputState inputState)
    {
        var mappedButtons = inputState.PressedButtons
            .Select(button => _buttonMap.TryGetValue(button, out var mappedButton) ? mappedButton : button)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return inputState with { PressedButtons = mappedButtons };
    }
}
