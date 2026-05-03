# PS Controller Emulator QA

## Without Controller Hardware

1. Build the solution:

   ```powershell
   dotnet build
   ```

2. Run tests:

   ```powershell
   dotnet test
   ```

3. Run CLI mock diagnostics:

   ```powershell
   dotnet run --project src/PSControllerEmulator.Cli -- devices --mock
   ```

4. Confirm output contains one mock DualSense with connection, battery, and neutral input state.
5. Launch the desktop app:

   ```powershell
   dotnet run --project src/PSControllerEmulator.App
   ```

6. Confirm the app opens and shows the mock DualSense fallback.

## With PlayStation Controller Hardware

HID discovery is read-only. USB and Bluetooth transport are inferred conservatively from Windows HID paths; ambiguous paths are skipped rather than mislabeled. Input parsing, battery extraction from real hardware, and virtual emulation are future work.

1. Connect one supported controller over USB:
   - DualSense
   - DualSense Edge
   - DualShock 4
2. Build and run tests.
3. Launch the app and press Refresh.
4. Confirm the controller model and USB connection type are shown.
5. Confirm missing battery data is displayed as unavailable instead of an error.
6. Run CLI diagnostics:

   ```powershell
   dotnet run --project src/PSControllerEmulator.Cli -- devices
   ```

7. Confirm one entry appears for the connected controller with connection type `USB`.
8. Record controller model, Windows version, connection type, product/manufacturer naming, and any incorrect status in a hardware compatibility report.

## Bluetooth Manual Checks (Where Available)

1. Pair one supported controller in Windows Bluetooth settings:
   - DualSense
   - DualSense Edge
   - DualShock 4
2. Run:

   ```powershell
   dotnet run --project src/PSControllerEmulator.Cli -- devices
   ```

3. Confirm the controller appears with connection type `Bluetooth` when path metadata is recognizable.
4. If the device does not appear, capture the environment details and note that path metadata may be ambiguous/unclassified.

## Regression Checklist

- The app does not crash when zero controllers are connected.
- `psce devices --mock` prints exactly one mock DualSense.
- Hardware-facing code remains behind interfaces that tests can replace.
- Clean-room notice remains present in README and PRD.
