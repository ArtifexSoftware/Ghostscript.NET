//
// GhostscriptPipedOutput.cs
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
using System.Threading.Tasks;

namespace Ghostscript.NET
{
    /// <summary>
    /// Represents a Ghostscript piped output.
    /// </summary>
    public class GhostscriptPipedOutput : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private int _localClientHandleReleased = 0;
        private AnonymousPipeServerStream _pipe;
        private Task _readTask;
        private readonly MemoryStream _data = new MemoryStream();
        private readonly object _dataSync = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptPipedOutput class.
        /// </summary>
        public GhostscriptPipedOutput()
        {
            _pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            _readTask = Task.Factory.StartNew(() => ReadGhostscriptPipeOutput(null), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        #region Destructor

        ~GhostscriptPipedOutput()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        #region Dispose

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptPipedOutput instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptPipedOutput instance.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ReleaseLocalClientHandle();

                    if (_pipe != null)
                    {
                        try
                        {
                            _pipe.Dispose();
                        }
                        catch
                        {
                        }

                        _pipe = null;
                    }

                    if (_readTask != null)
                    {
                        try
                        {
                            _readTask.Wait(TimeSpan.FromSeconds(2));
                        }
                        catch (AggregateException)
                        {
                        }

                        _readTask = null;
                    }

                    lock (_dataSync)
                    {
                        _data.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region ClientHandle

        /// <summary>
        /// Gets pipes client handle as string. 
        /// </summary>
        public string ClientHandle
        {
            get 
            {
                if (_disposed || _pipe == null)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                return _pipe.GetClientHandleAsString();
            }
        }

        #endregion

        #region ReleaseLocalClientHandle

        /// <summary>
        /// Closes the server's extra copy of the write end. Ghostscript keeps its own handle
        /// after <c>%handle%</c> is opened; until this copy is released, Read never sees EOF.
        /// Call after Ghostscript has received the handle (typically from <see cref="Data"/>).
        /// </summary>
        private void ReleaseLocalClientHandle()
        {
            if (Interlocked.Exchange(ref _localClientHandleReleased, 1) == 1)
            {
                return;
            }

            AnonymousPipeServerStream pipe = _pipe;
            if (pipe == null)
            {
                return;
            }

            try
            {
                pipe.DisposeLocalCopyOfClientHandle();
            }
            catch
            {
            }
        }

        #endregion

        #region ReadGhostscriptPipeOutput

        /// <summary>
        /// Reads Ghostscript output.
        /// </summary>
        public void ReadGhostscriptPipeOutput(object state)
        {
            AnonymousPipeServerStream pipe = _pipe;
            if (pipe == null)
            {
                return;
            }

            byte[] buffer = new byte[8192];

            try
            {
                int readCount;

                while ((readCount = pipe.Read(buffer, 0, buffer.Length)) > 0)
                {
                    lock (_dataSync)
                    {
                        try
                        {
                            _data.Write(buffer, 0, readCount);
                        }
                        catch (ObjectDisposedException)
                        {
                            return;
                        }
                    }
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
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the Ghostscript output.
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                ReleaseLocalClientHandle();

                if (!_readTask.Wait(TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException("Timed out reading Ghostscript piped output.");
                }

                lock (_dataSync)
                {
                    return _data.ToArray();
                }
            }
        }

        #endregion

    }

}
