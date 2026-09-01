**Ghostscript.NET** is the most completed managed wrapper library around the [Ghostscript](https://ghostscript.com) library - an interpreter for PDF and PostScript files. With a licensed GhostPDL library (`gpdldll` / `libgpdl`) it also converts Microsoft Office documents.

Ghostscript can be provided via a system install or the optional `Ghostscript.NativeAssets` NuGet package (app-local binaries). Office/SmartOffice natives are **not** in that package; licensed users obtain them from Ghostscript.NET.Office.

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
