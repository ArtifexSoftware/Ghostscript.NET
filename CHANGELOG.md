# Changelog



### [1.3.4] - 2026-07-20

- **Bundled native library discovery:** `GhostscriptVersionInfo` can locate app-local Ghostscript binaries (for example from `Ghostscript.NativeAssets`) via `TryGetBundledVersion`, `GetBundledVersion`, and `GetPreferredVersion`. `GetLastInstalledVersion` now prefers bundled assets before falling back to a system installation.

- **Ghostscript.NativeAssets:** New optional companion NuGet package ships Ghostscript 10.07.1 native libraries for `win-x64`, `win-x86`, and `linux-x64`, with MSBuild targets to copy binaries into the application output.

- **Linux/macOS fromMemory fallback:** Passing `fromMemory: true` to `GhostscriptLibrary` / `GhostscriptProcessor` no longer throws on non-Windows platforms; the native library is loaded from disk instead.

### [1.3.3] - 2026-04-06

- **Unicode in arguments and file paths:** Ghostscript is invoked with native-encoded argv (UTF-16LE on Windows, UTF-8 elsewhere) via pointer-based `gsapi_init_with_args` when available, and `gsapi_run_file` uses the same encoding model where supported. Non-ASCII characters in paths and switches are preserved instead of being mangled by the legacy string-based API. New bindings: `gsapi_init_with_args_ptr`, `gsapi_run_file_ptr`; `GhostscriptProcessor`, `GhostscriptInterpreter`, and related helpers route through these when the loaded Ghostscript library reports support.

- **Samples (Ghostscript.NET.Samples):** `RunMultipleInstancesSample` now waits for both thread-pool jobs, passes `-f` and the input file as separate argv entries, uses load-from-memory on Windows for parallel processors, and points at a shared test PDF under `TestFiles`. `ViewerSample` writes the first page to `Output/ViewerSample.png`, synchronizes on raster completion, and uses `Console.ReadLine()` before teardown so work is not cut off; default input is the repo’s `PipedOutputSample.ps` instead of a missing PDF.

- **Sample inputs in `TestFiles`:** All PDFs and other inputs referenced by the samples (for example `ProcessorSample1.pdf`, `ProcessorSample2.pdf`, `RasterizerSample1.pdf`, `RasterizerSample2.pdf`, `GetInkCoverageSample.pdf`, `DeviceUsageSample.pdf`, `AddWatermarkSample.pdf`, `SendToPrinterSample.pdf`, and the Unicode test PDFs) are collected under `Ghostscript.NET.Samples/TestFiles/`, together with PostScript such as `PipedOutputSample.ps`, so every demo reads from one shared folder.

### [1.3.2] - 2025-12-10

- Upgraded SkiaSharp version (2.88.8 -> 3.119.1)

- Included SkiaSharp.NativeAssets.Linux



### [1.3.2-rc.3] - 2025-12-05

- Converted from System.Drawing.Bitmap to SkiaSharp.SKBitmap for cross-platform support

- Replaced System.Drawing.Common package with SkiaSharp (2.88.8)

- Updated memory operations to work on both Windows and Linux

- Fixed color accuracy by correctly handling BGR format from Ghostscript

- Updated platform-specific structures for Linux compatibility

