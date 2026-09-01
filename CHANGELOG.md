# Changelog

### [1.3.5] - 2026-08-28

- **Office files:** Word, Excel, and PowerPoint can be converted and rasterized when a licensed GhostPDL library is present. Without it, Office APIs throw `GhostscriptPdlLibraryNotFoundException` and point users to Artifex for a commercial Ghostscript.NET license.
- **Existing processor code:** `GhostscriptProcessor` detects an Office path in the argument list, loads `gpdldll` if it is on the search path, and treats `-dSAFER` as `-dNOSAFER` for that job. Callers can keep `GetLastInstalledVersion()`.
- **32-bit GhostPDL:** `gpdldll32.dll` exports stdcall names (`_gsapi_revision@8`). Ghostscript.NET now resolves those in x86 processes. NativeAssets `gsdll32.dll` already used undecorated names, so it was unaffected.
- GhostPDL/SmartOffice is **commercial**. It is not in `Ghostscript.NativeAssets` or on nuget.org. Licensed users copy the library from Ghostscript.NET.Office into their project.

### [1.3.4] - 2026-07-20

- **Bundled native library discovery:** `GhostscriptVersionInfo` can locate app-local Ghostscript binaries (for example from `Ghostscript.NativeAssets`) via `TryGetBundledVersion`, `GetBundledVersion`, and `GetPreferredVersion`. `GetLastInstalledVersion` now prefers bundled assets before falling back to a system installation.

- **Ghostscript.NativeAssets:** New optional companion NuGet package ships Ghostscript 10.07.1 native libraries for `win-x64`, `win-x86`, and `linux-x64`, with MSBuild targets to copy binaries into the application output.

- **Linux/macOS fromMemory fallback:** Passing `fromMemory: true` to `GhostscriptLibrary` / `GhostscriptProcessor` no longer throws on non-Windows platforms; the native library is loaded from disk instead.

- Centralizes package versions in `Versions.props` / `Directory.Build.props`.

- Adds `GhostscriptDiscoverySource` (`Bundled` / `System` / `Custom`) on `GhostscriptVersionInfo`.

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

### [1.3.1] - 2025-10-28

- Adds support for Linux by loading `libgs.so`.

### [1.3.0] - 2025-03-18

- Adds support for PDF/A conversion and ZUGFeRD samples with the `PDFA3Converter` module.

---

### [1.2.3] - 2021-03-09

- Fixed GhostscriptRasterizer/GhostscriptViewer and Ghostscript v.9.50+ compatibility issues.

### [1.2.2] - 2021-02-04

- Fixed Ghostscript 9.26+ compatibility.
- Fixed problem when opening path/file that contains non-ASCII characters.
- Fixed "Arithmetic operation resulted in an overflow" when using multithread instance.
- Changed Y and Y DPI settings to match GhostscriptViewer.
- Fixed CurrentPage -> TotalPages logging.
- Fixed watermark transparency bug for PDF.

### [1.2.1] - 2016-12-30

- Fixed offset error in StdIn buffering.
- Fixed problem with keeping whitespace return from the StdIn callbacks.
- Fixed problem with GhostscriptProcessor and current page handling event.
- Fixed problem with PDF signature/marker not being within first 32 bytes.
- Fixed problem with System.ArgumentOutOfRangeException when StdOut message does not occur at beginning of the line.
- Fixed problem with Amazon.Runtime.Internal.Util.HashStream.Position throwing NotSupportedException.
- Added ability to keep native Ghostscript instance within GhostscriptProcessor and reuse it, avoiding multiple library load/free especially when loaded from memory.
- GhostscriptRasterizer — added a constructor to allow capture of IO using GhostscriptStdIO (Rasterizer samples updated).
- GhostscriptViewer — added GridFitTT=0 to improve text quality.
- GhostscriptViewer — added public setter for DPI settings.
- GhostscriptViewer — made Zoom method public; set test to false by default in Zoom.
- GhostscriptViewer — changed ShowAfterOpen to public to set parameters without generating a PDF.

### [1.2.0] - 2015-02-05

- Fixed problem with checking for PDF file header signature in files that have extra bytes before the actual signature.
- Fixed problem with the page rasterized to Image object in memory being disposed after GhostscriptRasterizer is closed.
- Added ability to set custom switches for GhostscriptRasterizer and GhostscriptViewer.
- Added more usage examples.

### [1.1.9] - 2014-07-30

- Fixed problem with PDF invisible layers (optional content groups left unmarked if process_trailer_attrs is not executed).
- Fixed text rasterization problem for some PDFs by replacing `pdfopen begin` with `runpdfopen`.
- Changed GhostscriptRasterizer methods to support Stream instead of MemoryStream.
- Fixed handling files without extension in GhostscriptViewer and Rasterizer.

### [1.1.8] - 2014-05-08

- Fixed incompatibility with `gsapi_set_arg_encoding` in Ghostscript releases prior to 9.10.
- Fixed older-version incompatibility with `-dMaxBitmap=1g` switch which in some cases turned on text antialiasing for Ghostscript 9.14.
- Added better initialization checking.

### [1.1.7] - 2014-04-29

- Implemented Ghostscript native library verification with a friendly error message when the library is not compatible with the running process.
- Fixed pipe client handle disposal bug when GhostscriptPipedOutput is used.
- Fixed applying PDF page orientation for GhostscriptViewer and GhostscriptRasterizer.

### [1.1.6] - 2014-04-23

- Simplified GetInstalledVersions and GetLastInstalledVersion functions.
- Fixed CropBox problem in GhostscriptViewer and GhostscriptRasterizer.
- License changed to AGPL.

### [1.1.5] - 2014-03-20

- Fixed default cropping to BoundingBox for EPS in GhostscriptViewer and GhostscriptRasterizer.
- Exposed GhostscriptViewer.EPSClip and GhostscriptRasterizer.EPSClip (default true; EPS only).
- Fixed paths containing diacritics.
- Added GhostscriptProcessor.Started and GhostscriptProcessor.Completed events.
- Methods that worked with MemoryStream changed to use generic Stream.

### [1.1.4] - 2014-02-14

- Fixed applying GraphicsAlphaBits and TextAlphaBits (improved antialiasing in Viewer/Rasterizer).
- Fixed output through main stderr callback handler.
- Added MemoryStream support in GhostscriptPdfInfo.GetInkCoverage.
- Added GraphicsAlphaBits and TextAlphaBits properties on Viewer and Rasterizer.

### [1.1.3] - 2014-02-05

- Added GhostscriptPdfInfo.GetInkCoverage for CMYK ink coverage per page (RGB converted internally).
- Fixed opening MemoryStream EPS files with EPS Preview Header.
- Fixed empty %%BoundingBox when handling PostScript files.
- Fixed rasterizing EPS files created with Adobe Illustrator.
- Fixed retrieving exported function handle in DynamicNativeLibrary on Server 2012 R2 with large memory.
- Assembly now signed with a strong name key.

### [1.1.2] - 2013-12-13

- Fixed GhostscriptPipedOutput.Data getter race condition.
- Added GhostscriptPipedOutput to the library.
- Fixed GhostscriptException error code text message resolving.
- Improved parameter checking and exception handling.
- Changed ImageMemoryHelper from public to internal.
- Implemented opening MemoryStream files from GhostscriptRasterizer and GhostscriptViewer.

### [1.1.1] - 2013-10-09

- Fixed MediaBox handling with negative llx/lly in Rasterizer and Viewer.
- Added GhostscriptPngDevice (png16m, pngalpha, pnggray, png256, png16, pngmono, pngmonod).
- Added GhostscriptJpegDevice (jpeg, jpeggray).
- Extended GhostscriptProcessor.StartProcessing to support GhostscriptDevice.
- Added constructors/methods that auto-use the last installed Ghostscript version.
- Added samples: AddWatermarkSample, ProcessorSample, DeviceUsageSample.

### [1.1.0] - 2013-10-07

- Added GhostscriptViewer state handling (SaveState, RestoreState).
- Extended GhostscriptRasterizer constructor to support an existing GhostscriptViewer instance.
- Fixed 32-bit assembly with 32-bit Ghostscript on 64-bit Windows registry lookup.

### [1.0.9] - 2013-09-27

- Implemented EPS support for GhostscriptViewer.
- Added GhostscriptRasterizer for exporting PDF/PS/EPS pages to System.Drawing.Image in memory with per-page DPI.
- Fixed gsapi_stdin callback value passing.
- Added ProgressiveUpdate property on GhostscriptViewer.

### [1.0.8] - 2013-09-20

- Renamed GhostscriptProcessor.Process to StartProcessing.
- Implemented StopProcessing for multithread abort of gsapi_init_with_args.
- Fixed ZoomIn/ZoomOut when decimal separator is comma.
- Added page navigation and zoom checker properties on GhostscriptViewer.
- Fixed viewing PostScript files without DSC header.

### [1.0.7] - 2013-09-17

- Implemented multi-page PostScript support for GhostscriptViewer.
- Included Microsoft.WinAny.Helper sources for single-DLL deployment.
- Added Processing event (TotalPages / CurrentPage) and Error event on GhostscriptProcessor.
- Added DisplayPdfSample.

### [1.0.6] - 2013-09-03

- Added zoom-in and zoom-out functionality.
- Fixed ImageMemoryHelper.Set24bppRgbImageColor when stride is not a multiple of 3.
- Fixed displayed page size.

### [1.0.5] - 2013-09-02

- Implemented progressive display update while Ghostscript is drawing/rasterizing.
- Fixed 64-bit vs 32-bit raster (stride) line size mismatch.
- Changed GhostscriptViewer event logic; updated Viewer and DisplayTest samples.

### [1.0.4] - 2013-08-30

- Fixed display_device callback for 64-bit systems (32-bit and 64-bit Ghostscript compatibility).
- Added GhostscriptViewer PostScript file handler.

### [1.0.3] - 2013-08-28

- Fixed GhostscriptInterpreter.Run string limit (strings larger than 64 KB supported).
- Added GhostscriptViewer* classes for PDF on-screen rendering and navigation.
- Added Ghostscript.NET.Viewer project.

### [1.0.2] - 2013-08-26

- Added GhostscriptInterpreter with StdIO and Display callbacks (PostScript from memory).
- Added Ghostscript.NET.DisplayTest sample.
- Changed samples logic for easier runs.
- Fixed GetInstalledVersion to search GPL and AFPL and match process architecture.

### [1.0.1] - 2013-08-26

- Changed implementation logic.
- Added GhostscriptProcessor with StdIO for processing via init arguments and output handling.

### [1.0.0] - 2013-08-22

- Initial release: Ghostscript functions implemented and base wrapper created.
- Ability to run multiple Ghostscript instances within a single process.
