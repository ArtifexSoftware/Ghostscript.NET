# Ghostscript.NativeAssets

Optional NuGet package that ships Ghostscript native libraries for use with [Ghostscript.NET](https://www.nuget.org/packages/Ghostscript.NET/).

## Included assets

This package tracks Ghostscript **10.07.1**. NuGet represents the package version as `10.7.1` because numeric version components cannot contain leading zeroes.

| RID | Included files | Status |
|---|---|---|
| `linux-x64` | `libgs.so` | Included |
| `win-x64` | `gsdll64.dll` | Included |
| `win-x86` | `gsdll32.dll` | Included |

The `ghostscript.version` metadata file next to each native library allows `GhostscriptVersionInfo` to report the bundled Ghostscript product version.

## Usage

```xml
<PackageReference Include="Ghostscript.NET" Version="1.3.4" />
<PackageReference Include="Ghostscript.NativeAssets" Version="10.7.1" />
```

> Package versions are managed in the repository `Versions.props` file.
Ghostscript.NET resolves libraries in this order:

1. Explicit path / `GhostscriptVersionInfo` supplied by the caller
2. Bundled NativeAssets (app-local)
3. System Ghostscript installation

```csharp
var version = GhostscriptVersionInfo.GetPreferredVersion();
// or
if (GhostscriptVersionInfo.TryGetBundledVersion(out var bundled))
{
    // use bundled
}
```

## Licensing

This package distributes Ghostscript native binaries. Use is subject to the [GNU AGPL v3](https://www.gnu.org/licenses/agpl-3.0.html) or a commercial license from [Artifex](https://artifex.com/contact/ghostscript).
