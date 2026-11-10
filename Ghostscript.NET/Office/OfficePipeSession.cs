//
// OfficePipeSession.cs
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
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Ghostscript.NET
{
    /// <summary>
    /// One anonymous pipe and one reader thread for one <c>gsapi</c> call.
    /// Create this before <c>gsapi_init_with_args</c>, pass <see cref="ClientHandle"/>
    /// as <c>-sSOPIPE</c>, then call <see cref="Finish"/> after <c>gsapi_delete_instance</c>.
    /// </summary>
    /// <remarks>
    /// The reader stays blocked in <c>Read</c> until the write end is closed. It does
    /// not exit when JSON or document zeros arrive. GhostPDL holds a copy of the write
    /// handle; this class also holds a local copy until <see cref="Finish"/>, which is
    /// what produces EOF. The volume POST is a separate thread per call; <see cref="Finish"/>
    /// waits for this reader, not for HTTP.
    /// </remarks>
    internal sealed class OfficePipeSession : IDisposable
    {
        private readonly AnonymousPipeServerStream _pipe;
        private readonly Thread _reader;
        private readonly OfficePipeFeed _feed = new OfficePipeFeed();
        private readonly string _key;
        private readonly Action<string> _printMessage;
        private ActivityMonitor _activity;
        private int _clientHandleReleased;
        private bool _disposed;

        internal OfficePipeSession(string key, Action<string> printMessage)
        {
            _key = key;
            _printMessage = printMessage;
            // Inheritable so GhostPDL can DuplicateHandle / inherit the write end
            // from the decimal HANDLE in -sSOPIPE.
            _pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            GhostscriptOffice.BeginPipeObservation();
            _reader = new Thread(ReadPipe);
            _reader.IsBackground = true;
            _reader.Name = "GhostscriptOfficePipe";
            _reader.Start();
        }

        internal string ClientHandle
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                return _pipe.GetClientHandleAsString();
            }
        }

        /// <summary>
        /// Call after <c>gsapi_delete_instance</c>. GhostPDL has dropped its write handle;
        /// releasing the local copy delivers EOF to <see cref="ReadPipe"/>.
        /// </summary>
        internal void Finish()
        {
            ReleaseLocalClientHandle();
            try
            {
                _reader.Join(TimeSpan.FromSeconds(30));
            }
            catch (ThreadStateException)
            {
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            ReleaseLocalClientHandle();
            try
            {
                _pipe.Dispose();
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void ReadPipe()
        {
            byte[] buffer = new byte[4096];
            try
            {
                int read;
                // Read returns 0 only at EOF. JSON and document zeros are payload.
                while ((read = _pipe.Read(buffer, 0, buffer.Length)) > 0)
                {
                    _feed.Write(
                        buffer,
                        read,
                        delegate { GhostscriptOffice.ObservePipeHeader(_key, _feed.HeaderBytes(), _printMessage); },
                        delegate { _activity = GhostscriptOffice.ObserveOfficeDocument(_activity); });
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (IOException)
            {
            }
            finally
            {
                // EOF for this call only. Ends this reporter, not any later Ghostscript call.
                if (_activity != null)
                {
                    _activity.End();
                }
            }
        }

        private void ReleaseLocalClientHandle()
        {
            if (Interlocked.Exchange(ref _clientHandleReleased, 1) == 1)
            {
                return;
            }

            try
            {
                _pipe.DisposeLocalCopyOfClientHandle();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
