![logo](https://artifex.com/images/logos/ghostscript-github-icon.png)

**Ghostscript.NET** - (written in C#) is the most completed managed wrapper library around the Ghostscript library (32-bit & 64-bit), an interpreter for the PostScript language, PDF, related software and documentation.

[**NuGet: PM> Install-Package Ghostscript.NET**](http://nuget.org/packages/Ghostscript.NET/)

**Contains**
 * GhostscriptViewer - View PDF, EPS or multi-page PostScript files on the screen
 * GhostscriptRasterizer - Rasterize PDF, EPS or multi-page PostScript files to any common image format.
 * GhostscriptProcessor - An easy way to call a Ghostscript library with a custom arguments / switches.
 * GhostscriptInterpreter - The PostScript interpreter.

**Other features**
 * allows you to rasterize files in memory without storing the output to disk.
 * supports zoom-in and zoom-out.
 * supports progressive update.
 * allows you to run multiple Ghostscript instances simultaneously within a single process.
 * compatible with 32-bit and 64-bit Ghostscript native library.

**Latest changes - 2021-03-09 - v.1.2.3.**
* fixed GhostscriptRasterizer/GhostscriptViewer and Ghostscript v.9.50+ compatibility issues.

**Latest changes - 2021-02-04 - v.1.2.2.**
 * fixed Ghostscript v.9.26 + (all later versions) compatibility.
 * fixed problem when opening path/file that contains non ASCII characters.
 * fixed "Arithmetic operation resulted in an overflow" when using multithread instance.
 * changed Y and Y DPI settings to match GhostscriptViewer.
 * fixed CurrentPage -> TotalPages logging.
 * fixed watermark transparency bug for PDF.
 
**Samples built on the top of the Ghostscript.NET library**

Direct postscript interpretation via Ghostscript.NET:

![Ghostscript.NET.Display](https://i.ibb.co/Fnk8rFP/ss-jj-1899.png)

Ghostscript.NET.Viewer (supports viewing of the PDF, EPS and multi-page PS files):

![Ghostscript.NET.Viewer](http://a.fsdn.com/con/app/proj/ghostscriptnet/screenshots/gs-net-render.png)


# License and Copyright

Available under both, open-source AGPL and commercial license agreements.

Please read the full text of the [AGPL license agreement](https://www.gnu.org/licenses/agpl-3.0.html) (which is also included here in file COPYING) to ensure that your use case complies with the guidelines of this license. If you determine you cannot meet the requirements of the AGPL, please contact [Artifex](https://artifex.com/contact/ghostscript-inquiry.php) for more information regarding a commercial license.

Artifex is the exclusive commercial licensing agent for Ghostscript.


