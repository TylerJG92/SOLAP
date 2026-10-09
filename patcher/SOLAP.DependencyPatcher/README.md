# SOLAP Dependency Patcher

`SOLAP.DependencyPatcher` is a BepInEx preloader patcher used by the SOLAP fork to resolve dependency conflicts before the main SOLAP plugin is initialized.

Its primary purpose is to provide the version of `Newtonsoft.Json` required by Archipelago without permanently modifying the files included with West of Loathing.

## Project Separation

`SOLAP.DependencyPatcher` is intentionally maintained as a separate C# project from the main `SOLAP` plugin.

The two projects produce separate assemblies:

- `SOLAP.csproj` builds `SOLAP.dll`
- `SOLAP.DependencyPatcher.csproj` builds `SOLAP.DependencyPatcher.dll`

The patcher must remain separate because BepInEx loads preloader patchers before normal plugins are initialized.

The patcher's source files therefore should not be compiled into `SOLAP.dll`. The main `SOLAP.csproj` excludes C# source files contained under the `patcher` directory.

## Why the Patcher Is Needed

West of Loathing includes:

- `Newtonsoft.Json` version `9.0.0.0`

The Archipelago client currently used by SOLAP requires:

- `Newtonsoft.Json` version `11.0.0.0`

Loading the game's bundled version causes Archipelago to fail because the required Newtonsoft.Json types are not available.

Previously, SOLAP installation required replacing the game's copy of `Newtonsoft.Json.dll`.

The dependency patcher avoids this by replacing the assembly used by the game at runtime while leaving the original West of Loathing installation untouched.

## How It Works

BepInEx gives the dependency patcher West of Loathing's `Newtonsoft.Json.dll` as a Mono.Cecil `AssemblyDefinition`.

The patcher then:

1. Locates the replacement `Newtonsoft.Json.dll`.
2. Reads the replacement assembly using Mono.Cecil.
3. Replaces BepInEx's target `AssemblyDefinition` with the replacement assembly.
4. Allows the game and SOLAP to continue loading using the required Newtonsoft.Json version.

This replacement occurs only at runtime.

The original file located in:

`West of Loathing_Data/Managed/Newtonsoft.Json.dll`

is not modified.

## Patcher File Layout

`Newtonsoft.Json.dll` must be located in the **same folder** as `SOLAP.DependencyPatcher.dll`.

For example:

```text
BepInEx/
└── patchers/
    └── SOLAP/
        ├── SOLAP.DependencyPatcher.dll
        └── Newtonsoft.Json.dll
```
The folder containing these files does not need to be named `SOLAP`.

The patcher determines the location of its own DLL at runtime and searches for `Newtonsoft.Json.dll` beside it. This allows the patcher to work inside package-specific folders created by mod managers without depending on a particular author, team, or package name.

## Special Instructions for Patcher Updates

When updating the dependency patcher or `Archipelago.MultiClient.Net`, verify which `Newtonsoft.Json.dll` build is required by that version of the Archipelago client.

For the current SOLAP build, the required replacement reports:

`Newtonsoft.Json` assembly version `11.0.0.0`

However, this should not be treated as interchangeable with any stock Newtonsoft.Json v11 release.

Archipelago.MultiClient.Net uses a Unity-compatible Newtonsoft.Json build maintained alongside the Archipelago client, so the replacement DLL should come from the same Archipelago-compatible source or release that matches the client version being used.

The required `Newtonsoft.Json.dll` must be distributed alongside `SOLAP.DependencyPatcher.dll` as described above.

If Archipelago changes its Newtonsoft.Json dependency in the future, update both the packaged replacement DLL and this documentation to match the new Archipelago-compatible build.