**Ghostscript.NET** is the most completed managed wrapper library around the [Ghostscript](https://ghostscript.com) library - an interpreter for PDF and PostScript files. With GhostPDL (`gpdldll` / `libgpdl`) it also converts Microsoft Office documents.

Ghostscript and GhostPDL can be provided via a system install or the optional `Ghostscript.NativeAssets` NuGet package (app-local binaries, including `gpdldll` / `libgpdl`). Call `GhostscriptOffice.Unlock(key)` for a full Office document; without a key, conversion is limited to the first 3 pages.

### Features

- View PDF, EPS or multi-page PostScript files on the screen.
- Rasterize PDF, EPS or multi-page PostScript files to any common image format.
- An easy way to call a Ghostscript library with a custom arguments / switches.
- Allows you to rasterize files in memory without storing the output to disk.
- Supports zoom-in and zoom-out.
- Supports progressive update.
- Allows you to run multiple Ghostscript instances simultaneously within a single process.
- Compatible with 32-bit and 64-bit Ghostscript native library.
- Optional bundled native libraries via `Ghostscript.NativeAssets`.
