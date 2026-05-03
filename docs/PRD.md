# PS Controller Emulator PRD

## Product Vision

PS Controller Emulator helps Windows 10/11 users inspect, configure, and eventually emulate PlayStation controllers through an open-source, clean-room utility.

## MVP Scope

- USB-first discovery for DualSense, DualSense Edge, and DualShock 4 via read-only Windows HID enumeration, with Bluetooth discovery when Windows HID metadata is sufficient.
- Desktop app that shows connected controller model, battery status when available, connection type, and current input state.
- Mock controller mode so UI and CLI workflows can be tested without hardware.
- CLI diagnostics for QA and issue reports.
- Layered architecture with mockable hardware-facing interfaces.
- Abstractions for remapping, lightbar, rumble/haptics, adaptive triggers, and virtual gamepad output.
- CI on Windows that builds and runs tests.

## Non-Goals

- Copying DSX code, assets, branding, UI, text, or proprietary behavior.
- Shipping kernel drivers in the initial skeleton.
- Bluetooth discovery in the first deliverable.
- Production-ready Xbox 360, DS4, or DualSense emulation in the first deliverable.
- Firmware modification or unsupported controller flashing.

## Risks

- Windows HID APIs require careful permission and hotplug handling.
- Battery and feature availability differ across USB, Bluetooth, firmware versions, and controller models.
- HID enumeration may be affected by Windows permissions, exclusive device access, or third-party drivers.
- USB transport inference is conservative: ambiguous HID paths are skipped instead of being labeled USB.
- Bluetooth discovery depends on Windows exposing recognizable HID metadata; some Bluetooth paths may remain unclassified.
- Virtual controller backends may require drivers, elevated setup, or user consent.
- Adaptive trigger and haptic behavior must be implemented conservatively and tested on real hardware.
- Hardware compatibility data needs privacy review before collection or upload.

## Milestones

1. Repository skeleton, mock discovery, WPF status app, CLI diagnostics, tests, docs, and CI.
2. Manual QA pass across DualSense, DualSense Edge, and DualShock 4 over USB.
3. Input report parsing and battery extraction.
4. Remapping profile editor with persisted local settings.
5. Virtual gamepad backend proof of concept with user-visible driver status.
