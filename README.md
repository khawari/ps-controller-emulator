# PS Controller Emulator

PS Controller Emulator is a clean-room, open-source Windows utility for PlayStation controller discovery, diagnostics, and future virtual gamepad emulation.

The first milestone focuses on a reliable foundation: USB-first controller discovery, mockable hardware boundaries, a simple WPF desktop status view, CLI diagnostics, and automated tests that do not require physical controllers.

## Clean-Room Notice

This project is a clean-room implementation. It does not copy DSX code, assets, branding, UI, text, or proprietary behavior. Design, architecture, naming, documentation, tests, and implementation are original to this repository.

## Current MVP

- Enumerates Windows HID devices and detects supported Sony PlayStation controllers by VID/PID metadata for USB, and for Bluetooth where Windows exposes usable HID path metadata.
- Falls back to a mock DualSense in the desktop app when no hardware is available.
- Prints deterministic CLI diagnostics with `psce devices --mock`.
- Defines core modules for remapping, lightbar color, rumble/haptics, adaptive trigger effects, and virtual controller targets.
- Runs automated tests without controller hardware.

Input report parsing, battery extraction from real hardware, Bluetooth discovery, and virtual gamepad emulation are future work.

Transport is inferred conservatively from Windows HID device paths. Ambiguous HID devices are skipped rather than mislabeled.

## Architecture

```text
src/
  PSControllerEmulator.App              WPF desktop app
  PSControllerEmulator.Cli              CLI diagnostics, executable name psce
  PSControllerEmulator.Core             Domain models, input state, module contracts
  PSControllerEmulator.Devices          USB/HID discovery boundaries and mock devices
  PSControllerEmulator.VirtualGamepad   Future virtual controller abstraction
tests/
  PSControllerEmulator.Tests            Hardware-free unit tests
docs/
  PRD.md                                Product scope and delivery plan
  QA.md                                 Manual QA checklist
```

WPF was chosen over WinUI 3 for the initial skeleton because it is faster to bootstrap, easy to build in CI, and stable for Windows 10/11 desktop utilities.

## Requirements

- Windows 10 or Windows 11
- .NET 8 SDK

## Run The Desktop App

```powershell
dotnet run --project src/PSControllerEmulator.App
```

If no supported USB controller is found, the app displays a mock DualSense so the UI remains testable.

## Run The CLI

```powershell
dotnet run --project src/PSControllerEmulator.Cli -- devices
dotnet run --project src/PSControllerEmulator.Cli -- devices --mock
```

After building, the CLI executable is named `psce.exe`, so the direct command is:

```powershell
src\PSControllerEmulator.Cli\bin\Debug\net8.0\psce.exe devices --mock
```

## Run Tests

```powershell
dotnet test
```

## Roadmap

1. Parse USB input reports and extract real battery status where available.
2. Add Bluetooth discovery and pairing diagnostics.
3. Integrate a user-selectable virtual gamepad backend for Xbox 360, DualShock 4, and DualSense targets.
4. Add persisted remapping profiles and import/export.
5. Expand hardware compatibility reporting with anonymized USB metadata.

## License

MIT. See [LICENSE](LICENSE).
