# Ghostscript.NET

[![Docs](https://img.shields.io/badge/docs-live-brightgreen)](https://ghostscript.readthedocs.io)
[![NuGet](https://img.shields.io/nuget/v/Ghostscript.NET)](https://www.nuget.org/packages/Ghostscript.NET/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Ghostscript.NET)](https://www.nuget.org/packages/Ghostscript.NET/)
[![License: AGPL | Commercial](https://img.shields.io/badge/license-AGPL%20%7C%20Commercial-orange)](https://www.gnu.org/licenses/agpl-3.0.html)
[![Target: .NET Standard 2.0](https://img.shields.io/badge/.NET-Standard%202.0-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![Discord](https://img.shields.io/discord/770681584617652264?color=6A7EC2&logo=discord&logoColor=ffffff)](https://artifex.com/discord/artifex/)

**Ghostscript.NET** is a managed C# wrapper for the [Ghostscript](https://ghostscript.com) native library (`gsdll64.dll` / `libgs.so`). It lets you rasterize, convert, and process PDF, PostScript, and EPS files from any .NET application without shelling out to a command-line process.

```powershell
Install-Package Ghostscript.NET
```

> **Ghostscript version compatibility:** Ghostscript.NET has been tested with Ghostscript versions up to 9.x. Compatibility with Ghostscript 10+ is not yet fully verified. See [ghostpdl-downloads](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases) for available Ghostscript releases, and install the native library separately before using this package.

---

## Contents

- [Why Ghostscript.NET](#why-ghostscriptnet)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick start](#quick-start)
- [Key capabilities](#key-capabilities)
- [Code examples](#code-examples)
- [API overview](#api-overview)
- [Finding the Ghostscript native library](#finding-the-ghostscript-native-library)
- [PDF/A-3 conversion](#pdfa-3-conversion)
- [Documentation](#documentation)
- [License](#license)

---

## Why Ghostscript.NET

- **In-process** — calls the Ghostscript library directly via P/Invoke; no child process, no shell, no temp-file plumbing
- **High-fidelity rendering** — uses the same Ghostscript engine that powers commercial print workflows
- **Memory-based rasterization** — render pages to `SKBitmap` (via SkiaSharp) without writing to disk
- **Multi-instance** — run multiple Ghostscript instances simultaneously within a single .NET process
- **Cross-platform** — targets .NET Standard 2.0; works on Windows and Linux
- **32-bit and 64-bit** — auto-detects and loads the matching native library
- **PDF/A-3 + ZUGFeRD / Factur-X** — embed XML invoices into PDF/A-3 files for European e-invoicing standards

---

## Requirements

| Requirement | Details |
|---|---|
| .NET | .NET Standard 2.0 or any compatible runtime (.NET 6, 7, 8, Framework 4.6.1+) |
| Ghostscript native library | Must be installed separately on the host machine |
| Ghostscript version | Tested with versions ≤ 9.x; Ghostscript 10+ compatibility not fully verified |
| OS | Windows (32-bit and 64-bit), Linux |
| SkiaSharp | Included as a NuGet dependency; provides `SKBitmap` for rasterized output |

### Installing the Ghostscript native library

**Windows**

[Download and install the Ghostscript installer](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases). The installer registers the DLL path in the Windows registry so `GhostscriptVersionInfo.GetLastInstalledVersion()` can find it automatically.

**Linux (Debian/Ubuntu)**

```bash
sudo apt-get install ghostscript
# Installs libgs.so to /usr/lib or /usr/lib/x86_64-linux-gnu/
```

---

## Installation

**Package Manager Console**

```powershell
Install-Package Ghostscript.NET
```

**.NET CLI**

```bash
dotnet add package Ghostscript.NET
```

**PackageReference**

```xml
<PackageReference Include="Ghostscript.NET" Version="1.3.3" />
```

---

## Quick start

**Rasterize all pages of a PDF to PNG files**

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using SkiaSharp;

// Auto-detect the installed Ghostscript library
var version = GhostscriptVersionInfo.GetLastInstalledVersion();

using var rasterizer = new GhostscriptRasterizer();
rasterizer.Open("input.pdf", version, false);

for (int page = 1; page <= rasterizer.PageCount; page++)
{
    SKBitmap image = rasterizer.GetPage(dpi: 150, pageNumber: page);
    using var stream = File.OpenWrite($"page_{page:D3}.png");
    image.Encode(stream, SKEncodedImageFormat.Png, 100);
}
```

**Convert a PDF to PDF using GhostscriptProcessor**

```csharp
using Ghostscript.NET.Processor;

using var processor = new GhostscriptProcessor();
processor.Process(new[]
{
    "-dBATCH", "-dNOPAUSE", "-dNOSAFER",
    "-sDEVICE=pdfwrite",
    "-sOutputFile=output.pdf",
    "input.pdf"
});
```

---

## Key capabilities

| Area | Description |
|---|---|
| **Page rasterization** | Render any page of a PDF, PS, or EPS file to an `SKBitmap` at any DPI |
| **Image export** | Save pages as PNG, JPEG, TIFF, BMP, or any SkiaSharp-supported format |
| **In-memory rendering** | Rasterize without writing intermediate files to disk |
| **PDF conversion** | Convert PS/EPS to PDF, compress PDFs, apply `pdfwrite` device settings |
| **Custom switches** | Pass any Ghostscript command-line switch directly via `CustomSwitches` or `Process(args[])` |
| **Progress events** | `GhostscriptProcessor` raises `Started`, `Processing` (per-page), `Error`, and `Completed` events |
| **Multi-instance** | Multiple `GhostscriptProcessor` or `GhostscriptRasterizer` instances can run in parallel |
| **Stdin/stdout capture** | Capture or redirect Ghostscript stdout/stderr via `GhostscriptStdIO` callbacks |
| **Version detection** | Auto-discovers installed Ghostscript on Windows (registry) and Linux (common library paths) |
| **PDF/A-3** | Convert PDFs to PDF/A-3b and embed XML invoices (XRechnung, Factur-X / ZUGFeRD) |
| **Anti-aliasing** | Control `GraphicsAlphaBits` and `TextAlphaBits` for rendering quality |
| **EPS cropping** | `EPSClip` property for correct EPS bounding box clipping |
| **Zoom** | `GhostscriptViewer` supports zoom-in / zoom-out and progressive page updates |

---

## Code examples

### Rasterize a single page to `SKBitmap`

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

var version = GhostscriptVersionInfo.GetLastInstalledVersion();

using var rasterizer = new GhostscriptRasterizer();
rasterizer.Open("input.pdf", version, false);

// Page numbers are 1-based
SKBitmap bitmap = rasterizer.GetPage(dpi: 300, pageNumber: 1);
```

### Rasterize with anti-aliasing and custom switches

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

var version = GhostscriptVersionInfo.GetLastInstalledVersion();

using var rasterizer = new GhostscriptRasterizer();
rasterizer.GraphicsAlphaBits = 4;   // 1, 2, or 4
rasterizer.TextAlphaBits = 4;
rasterizer.CustomSwitches.Add("-dUseCropBox");

rasterizer.Open("input.pdf", version, false);

for (int i = 1; i <= rasterizer.PageCount; i++)
{
    SKBitmap page = rasterizer.GetPage(dpi: 150, pageNumber: i);
    // use page...
}
```

### Rasterize from a stream (no temp file)

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

var version = GhostscriptVersionInfo.GetLastInstalledVersion();

using var stream = File.OpenRead("input.pdf");
using var rasterizer = new GhostscriptRasterizer();
rasterizer.Open(stream, version, false);

SKBitmap bitmap = rasterizer.GetPage(dpi: 150, pageNumber: 1);
```

### Convert PDF to PNG files using `GhostscriptPngDevice`

```csharp
using Ghostscript.NET.OutputDevices;

var device = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
device.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
device.TextAlphaBits    = GhostscriptImageDeviceAlphaBits.V_4;
device.ResolutionXY     = new GhostscriptImageDeviceResolution(300, 300);
device.InputFiles.Add("input.pdf");
device.Pdf.FirstPage    = 1;
device.Pdf.LastPage     = 5;
device.OutputPath       = @"output\page_%03d.png";
device.Process();
```

### Convert PDF to JPEG

```csharp
using Ghostscript.NET.OutputDevices;

var device = new GhostscriptJpegDevice(GhostscriptJpegDeviceType.Jpeg);
device.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
device.TextAlphaBits    = GhostscriptImageDeviceAlphaBits.V_4;
device.ResolutionXY     = new GhostscriptImageDeviceResolution(150, 150);
device.JpegQuality      = 85;
device.InputFiles.Add("input.pdf");
device.OutputPath       = @"output\page_%03d.jpg";
device.Process();
```

### Run with custom arguments and capture output

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Processor;

public class MyStdIO : GhostscriptStdIO
{
    public override void StdIn(out string input, int count) => input = string.Empty;
    public override void StdOut(string output) => Console.Write(output);
    public override void StdError(string error) => Console.Error.Write(error);
}

using var processor = new GhostscriptProcessor();
processor.Processing += (sender, e) =>
    Console.WriteLine($"Processing page {e.CurrentPage} of {e.TotalPages}");
processor.Error += (sender, e) =>
    Console.Error.WriteLine($"Error: {e.Message}");

processor.Process(new[]
{
    "-dBATCH", "-dNOPAUSE",
    "-sDEVICE=pdfwrite",
    "-dPDFSETTINGS=/ebook",       // compress for screen/ebook
    "-sOutputFile=compressed.pdf",
    "input.pdf"
}, new MyStdIO());
```

### Specify the Ghostscript DLL explicitly

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

// Point directly to a specific DLL — useful when Ghostscript is not installed system-wide
var version = new GhostscriptVersionInfo(@"C:\gs\gs9.56.1\bin\gsdll64.dll");

using var rasterizer = new GhostscriptRasterizer();
rasterizer.Open("input.pdf", version, false);
```

### Load the DLL from memory (embedded resource)

```csharp
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

byte[] dllBytes = File.ReadAllBytes(@"C:\gs\gs9.56.1\bin\gsdll64.dll");

using var rasterizer = new GhostscriptRasterizer();
rasterizer.Open("input.pdf", dllBytes);
```

---

## API overview

### Core classes

| Class | Namespace | Purpose |
|---|---|---|
| `GhostscriptRasterizer` | `Ghostscript.NET.Rasterizer` | Render PDF/PS/EPS pages to `SKBitmap` |
| `GhostscriptProcessor` | `Ghostscript.NET.Processor` | Run Ghostscript with any argument array; exposes progress events |
| `GhostscriptViewer` | `Ghostscript.NET.Viewer` | Interactive viewer with zoom and progressive rendering |
| `GhostscriptVersionInfo` | `Ghostscript.NET` | Discover installed Ghostscript versions; specify DLL path |
| `GhostscriptLibrary` | `Ghostscript.NET` | Low-level native library loader and P/Invoke surface |
| `GhostscriptStdIO` | `Ghostscript.NET` | Abstract base class for stdin/stdout/stderr callbacks |
| `GhostscriptPngDevice` | `Ghostscript.NET.OutputDevices` | Typed device for PNG output with all PNG switches |
| `GhostscriptJpegDevice` | `Ghostscript.NET.OutputDevices` | Typed device for JPEG output with quality and DPI settings |
| `PDFA3Converter` | `Ghostscript.NET` | Convert PDF to PDF/A-3b; embed XML invoices (ZUGFeRD / Factur-X) |

### `GhostscriptRasterizer` key members

| Member | Type | Description |
|---|---|---|
| `Open(string path)` | Method | Open a file; auto-detects installed Ghostscript |
| `Open(string path, GhostscriptVersionInfo, bool fromMemory)` | Method | Open with explicit version info |
| `Open(Stream stream, ...)` | Method | Open from a stream; no temp file written to disk |
| `Open(string path, byte[] library)` | Method | Open with DLL loaded from a byte array |
| `GetPage(int dpi, int pageNumber)` | Method | Render page to `SKBitmap`; pages are **1-based** |
| `PageCount` | Property | Total number of pages in the open document |
| `GraphicsAlphaBits` | Property | Anti-aliasing for graphics: 1, 2, or 4 |
| `TextAlphaBits` | Property | Anti-aliasing for text: 1, 2, or 4 |
| `EPSClip` | Property | Apply EPS bounding box clip when rasterizing EPS files |
| `CustomSwitches` | Property | `List<string>` of additional Ghostscript switches |
| `Close()` | Method | Release the open document |
| `Dispose()` | Method | Release all resources including the native library instance |

### `GhostscriptProcessor` key members

| Member | Type | Description |
|---|---|---|
| `Process(string[] args)` | Method | Run Ghostscript with a raw argument array |
| `Process(GhostscriptDevice device)` | Method | Run using a typed device object |
| `Process(string[] args, GhostscriptStdIO callback)` | Method | Run with stdout/stderr capture |
| `StartProcessing(...)` | Method | Alias for `Process`; included for API compatibility |
| `StopProcessing()` | Method | Signal Ghostscript to abort the current job |
| `IsRunning` | Property | `true` while a job is in progress |
| `IsStopping` | Property | `true` if `StopProcessing()` has been called but the job hasn't exited yet |
| `Started` | Event | Raised when processing begins |
| `Processing` | Event | Raised once per page; `args.CurrentPage`, `args.TotalPages` |
| `Error` | Event | Raised on Ghostscript error output; `args.Message` |
| `Completed` | Event | Raised when processing finishes (success or error) |

### `GhostscriptVersionInfo` key members

| Member | Type | Description |
|---|---|---|
| `GetLastInstalledVersion()` | Static method | Returns the highest-version Ghostscript found on the system |
| `GetLastInstalledVersion(license, priority)` | Static method | Filter by `GhostscriptLicense`: `GPL`, `AFPL`, or `Artifex` |
| `GetInstalledVersions()` | Static method | Returns all installed Ghostscript versions as a list |
| `IsGhostscriptInstalled` | Static property | `true` if any Ghostscript installation is detected |
| `new GhostscriptVersionInfo(string dllPath)` | Constructor | Point to a specific DLL file path |
| `.DllPath` | Property | Path to the native library file |
| `.Version` | Property | `System.Version` of the detected installation |

---

## Finding the Ghostscript native library

`GhostscriptVersionInfo.GetLastInstalledVersion()` searches for the native library automatically:

- **Windows:** reads the registry keys `HKLM\SOFTWARE\GPL Ghostscript\`, `HKLM\SOFTWARE\AFPL Ghostscript\`, and `HKLM\SOFTWARE\Artifex Ghostscript\`. It matches the DLL bitness (32-bit `gsdll32.dll` / 64-bit `gsdll64.dll`) to the current process.
- **Linux:** searches common paths including `/usr/lib`, `/usr/lib/x86_64-linux-gnu`, and `/usr/local/lib` for `libgs.so.10`, `libgs.so.9`, or `libgs.so`.

If Ghostscript is not installed in a standard location, pass the path directly:

```csharp
// Explicit path
var version = new GhostscriptVersionInfo(@"C:\MyApp\gs\gsdll64.dll");

// From embedded byte array (deploy DLL as an embedded resource)
byte[] dll = GetEmbeddedResource("gsdll64.dll");
rasterizer.Open("input.pdf", dll);
```

---

## PDF/A-3 conversion

The `PDFA3Converter` class converts any PDF to PDF/A-3b format and optionally embeds a ZUGFeRD or Factur-X XML invoice. This is the format required by XRechnung (Germany) and Factur-X (France/EU) electronic invoicing standards.

### Convert to plain PDF/A-3

```csharp
using Ghostscript.NET;

var converter = new PDFA3Converter(@"C:\gs\gs9.56.1\bin\gsdll64.dll");
converter.ConvertToPDFA3("invoice.pdf", "invoice-pdfa3.pdf");
```

### Embed a ZUGFeRD / Factur-X XML invoice

```csharp
using Ghostscript.NET;

var converter = new PDFA3Converter(@"C:\gs\gs9.56.1\bin\gsdll64.dll");
converter.SetZUGFeRDProfile(ZUGFeRDProfile.Comfort);
converter.SetZUGFeRDVersion("2.3");
converter.SetEmbeddedXMLFile("factur-x.xml");
converter.ConvertToPDFA3("invoice.pdf", "invoice-pdfa3-zugferd.pdf");
```

> Issues with `PDFA3Converter` should be tagged to [@stephanstapel](https://github.com/stephanstapel) on GitHub.

---

## Documentation

| Resource | URL |
|---|---|
| NuGet package | https://www.nuget.org/packages/Ghostscript.NET/ |
| GitHub repository | https://github.com/ArtifexSoftware/Ghostscript.NET |
| Sample projects | `Ghostscript.NET.Samples/` in this repository |
| Ghostscript documentation | https://ghostscript.readthedocs.io |
| Ghostscript binary downloads | https://github.com/ArtifexSoftware/ghostpdl-downloads/releases |
| Ghostscript command-line reference | https://ghostscript.readthedocs.io/en/latest/Use.html |
| Bug reports | https://github.com/ArtifexSoftware/Ghostscript.NET/issues |
| Artifex commercial licensing | https://artifex.com/contact/ghostscript-net |

---

## License

Ghostscript.NET is available under two licences:

- **[GNU AGPL v3](https://www.gnu.org/licenses/agpl-3.0.html)** — free for open-source projects. Any application that uses or distributes Ghostscript.NET must release its complete source code under a compatible open-source licence. See `COPYING` in this repository for the full licence text.
- **Commercial licence** — required for proprietary or closed-source applications. [Contact Artifex](https://artifex.com/contact/ghostscript-net) for licensing. Artifex is the exclusive commercial licensing agent for Ghostscript.
