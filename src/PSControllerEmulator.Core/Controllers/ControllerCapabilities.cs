namespace PSControllerEmulator.Core.Controllers;

public sealed record ControllerCapabilities(
    bool HasBattery,
    bool HasLightbar,
    bool HasRumble,
    bool HasHaptics,
    bool HasAdaptiveTriggers,
    bool HasTouchpad,
    bool HasMotionSensors)
{
    public static ControllerCapabilities For(ControllerModel model)
    {
        return model switch
        {
            ControllerModel.DualSense => new(true, true, true, true, true, true, true),
            ControllerModel.DualSenseEdge => new(true, true, true, true, true, true, true),
            ControllerModel.DualShock4 => new(true, true, true, false, false, true, true),
            _ => new(false, false, false, false, false, false, false)
        };
    }

    public string ToDisplayText()
    {
        var features = new List<string>();

        if (HasLightbar)
        {
            features.Add("lightbar");
        }

        if (HasRumble)
        {
            features.Add("rumble");
        }

        if (HasHaptics)
        {
            features.Add("haptics");
        }

        if (HasAdaptiveTriggers)
        {
            features.Add("adaptive triggers");
        }

        if (HasTouchpad)
        {
            features.Add("touchpad");
        }

        if (HasMotionSensors)
        {
            features.Add("motion sensors");
        }

        return features.Count == 0 ? "No feature flags available" : string.Join(", ", features);
    }
}
