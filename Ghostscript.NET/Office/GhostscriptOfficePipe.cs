//
// GhostscriptOfficePipe.cs
// This file is part of Ghostscript.NET library
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Ghostscript.NET
{
    /// <summary>
    /// One GhostPDL <c>-sSOPIPE</c> record: a JSON object, a terminating 0,
    /// then one extra 0 per Office file opened in that gsapi call.
    /// </summary>
    /// <remarks>
    /// Document count is extra zeros after the JSON terminator, not the total
    /// number of zeros. <c>{json}\0</c> is 0 files; <c>{json}\0\0</c> is 1 file.
    /// </remarks>
    internal sealed class OfficePipeReport
    {
        internal static readonly OfficePipeReport Empty = new OfficePipeReport(string.Empty, -1, 0, -1, 0, false);

        private OfficePipeReport(string message, int keyOk, int flags, int product, int documentCount, bool hasStatus)
        {
            Message = message ?? string.Empty;
            KeyOk = keyOk;
            Flags = flags;
            Product = product;
            DocumentCount = documentCount;
            HasStatus = hasStatus;
        }

        internal string Message { get; private set; }

        internal int KeyOk { get; private set; }

        internal int Flags { get; private set; }

        internal int Product { get; private set; }

        internal int DocumentCount { get; private set; }

        internal bool HasStatus { get; private set; }

        internal static OfficePipeReport Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return Empty;
            }

            int nul = Array.IndexOf(data, (byte)0);
            string text = nul >= 0
                ? Encoding.UTF8.GetString(data, 0, nul)
                : Encoding.UTF8.GetString(data);

            int zeros = 0;
            if (nul >= 0)
            {
                for (int i = nul; i < data.Length; i++)
                {
                    if (data[i] == 0)
                    {
                        zeros++;
                    }
                }
            }

            // First 0 ends JSON. Each 0 after that is one Office file.
            int documentCount = zeros > 0 ? zeros - 1 : 0;
            string json = PrepareJson(text);
            if (json.Length == 0)
            {
                return new OfficePipeReport(string.Empty, -1, 0, -1, documentCount, nul >= 0);
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement;
                    string message = ReadString(root, "message").TrimEnd('\r', '\n');
                    int keyOk = ReadInt(root, "key_ok", -1);
                    int flags = ReadInt(root, "flags", 0);
                    int product = ReadInt(root, "product", -1);
                    bool hasStatus = nul >= 0 || root.TryGetProperty("message", out _)
                        || root.TryGetProperty("key_ok", out _)
                        || root.TryGetProperty("flags", out _)
                        || root.TryGetProperty("product", out _);
                    return new OfficePipeReport(message, keyOk, flags, product, documentCount, hasStatus);
                }
            }
            catch (JsonException)
            {
                bool hasStatus = nul >= 0
                    || text.IndexOf("\"key_ok\"", StringComparison.Ordinal) >= 0
                    || text.IndexOf("\"flags\"", StringComparison.Ordinal) >= 0
                    || text.IndexOf("\"message\"", StringComparison.Ordinal) >= 0;
                return new OfficePipeReport(string.Empty, -1, 0, -1, documentCount, hasStatus);
            }
        }

        /// <summary>
        /// GhostPDL sometimes omits the opening <c>{ "message" : "</c> and puts a raw
        /// newline inside the message string. Repair that, then let the JSON parser read the object.
        /// </summary>
        private static string PrepareJson(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int start = 0;
            while (start < text.Length && char.IsWhiteSpace(text[start]))
            {
                start++;
            }

            if (start >= text.Length)
            {
                return string.Empty;
            }

            string body = start == 0 ? text : text.Substring(start);
            if (body[0] != '{')
            {
                body = "{ \"message\" : \"" + body;
            }

            return EscapeControlsInsideStrings(body);
        }

        private static string EscapeControlsInsideStrings(string json)
        {
            StringBuilder builder = null;
            bool inString = false;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (inString)
                {
                    if (c == '\\' && i + 1 < json.Length)
                    {
                        if (builder != null)
                        {
                            builder.Append(c);
                            builder.Append(json[i + 1]);
                        }

                        i++;
                        continue;
                    }

                    if (c == '"')
                    {
                        inString = false;
                    }
                    else if (c < ' ')
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(json.Length + 8);
                            builder.Append(json, 0, i);
                        }

                        if (c == '\n')
                        {
                            builder.Append("\\n");
                        }
                        else if (c == '\r')
                        {
                            builder.Append("\\r");
                        }
                        else if (c == '\t')
                        {
                            builder.Append("\\t");
                        }
                        else
                        {
                            builder.Append("\\u");
                            builder.Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        }

                        continue;
                    }
                }
                else if (c == '"')
                {
                    inString = true;
                }

                if (builder != null)
                {
                    builder.Append(c);
                }
            }

            return builder == null ? json : builder.ToString();
        }

        private static string ReadString(JsonElement root, string name)
        {
            JsonElement value;
            if (!root.TryGetProperty(name, out value) || value.ValueKind != JsonValueKind.String)
            {
                return string.Empty;
            }

            return value.GetString() ?? string.Empty;
        }

        private static int ReadInt(JsonElement root, string name, int missing)
        {
            JsonElement value;
            if (!root.TryGetProperty(name, out value) || value.ValueKind != JsonValueKind.Number)
            {
                return missing;
            }

            int parsed;
            return value.TryGetInt32(out parsed) ? parsed : missing;
        }
    }

    /// <summary>
    /// Incremental SOPIPE reader. Bytes can arrive in any chunk size.
    /// The first 0 ends the JSON block. Each later 0 is one Office file.
    /// Non-zero bytes after the header are ignored.
    /// </summary>
    internal sealed class OfficePipeFeed
    {
        private readonly MemoryStream _header = new MemoryStream();
        private bool _headerDone;
        private int _documents;

        internal int DocumentCount
        {
            get { return _documents; }
        }

        internal byte[] HeaderBytes()
        {
            return _header.ToArray();
        }

        internal void Write(byte[] buffer, int count, Action onHeader, Action onDocument)
        {
            for (int i = 0; i < count; i++)
            {
                byte value = buffer[i];
                if (!_headerDone)
                {
                    if (value == 0)
                    {
                        _headerDone = true;
                        if (onHeader != null)
                        {
                            onHeader();
                        }
                    }
                    else
                    {
                        _header.WriteByte(value);
                    }

                    continue;
                }

                if (value == 0)
                {
                    _documents++;
                    if (onDocument != null)
                    {
                        onDocument();
                    }
                }
            }
        }
    }
}
