# 01-prerequisites: Verify toolchain and SDK readiness

Confirm the local environment has the .NET 8 SDK installed and that global.json (if present) is compatible with .NET 8. Verify that no pending tool or environment changes will interfere with the upgrade. This is a fast gate task — no code changes are expected.

**Done when**: `dotnet --list-sdks` shows a .NET 8 SDK installed; any global.json present in the repo root is compatible with .NET 8.0; build environment is confirmed ready.
