//
// ActivityMonitor.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Ghostscript.NET
{
    /// <summary>
    /// Volume-license reporter for one Ghostscript call.
    /// </summary>
    /// <remarks>
    /// One instance per <c>gsapi</c> call that processed a qualifying Office file.
    /// The first document is POSTed as soon as its 0 arrives. Further documents from
    /// the same call are batched about every 10 seconds so a 1000-file job is not
    /// 1000 HTTP requests. When that call's pipe reaches EOF, the remainder is POSTed
    /// and this worker exits. The next <c>GhostscriptProcessor</c> call starts another
    /// reporter. Mid-run HTTP failures are left for the next batch; EOF retries for
    /// up to 10 seconds. <see cref="FlushForExit"/> waits for live workers on process
    /// exit; <c>StartProcessing</c> does not wait for HTTP.
    /// </remarks>
    internal sealed class ActivityMonitor
    {
        internal const string DefaultVolumeMonitorUrl = "https://lp2piszqpd.execute-api.us-east-1.amazonaws.com/usage";

        internal const string VolumeMonitorUrlEnvironmentVariable = "GHOSTSCRIPT_NET_OFFICE_VOLUME_MONITOR_URL";

        static string _unlockVolumeMonitorUrl;

        internal const int ShutdownFlushSeconds = 10;

        internal const string ShutdownFlushFailedMessage =
            "Ghostscript.NET Office: Activity flush to server failed after 10 seconds.";

        const int ActivityNewDoc = 0;
        const int BatchSeconds = 10;
        const int MidRunPostSeconds = 30;

        static readonly object Gate = new object();
        static readonly List<ActivityMonitor> Live = new List<ActivityMonitor>();
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly HttpClient Http = CreateHttpClient();

        static bool _exitHooked;
        static int _shutdownFailedPrinted;

        readonly BlockingCollection<int?> _queue = new BlockingCollection<int?>();
        readonly Thread _worker;
        readonly string _key;

        /// <summary>
        /// POST target: <see cref="SetUnlockVolumeMonitorUrl"/>, else
        /// <c>GHOSTSCRIPT_NET_OFFICE_VOLUME_MONITOR_URL</c>, else <see cref="DefaultVolumeMonitorUrl"/>.
        /// </summary>
        internal static string VolumeMonitorUrl
        {
            get
            {
                lock (Gate)
                {
                    if (!string.IsNullOrEmpty(_unlockVolumeMonitorUrl))
                    {
                        return _unlockVolumeMonitorUrl;
                    }
                }

                string fromEnv = Environment.GetEnvironmentVariable(VolumeMonitorUrlEnvironmentVariable);
                if (!string.IsNullOrWhiteSpace(fromEnv))
                {
                    return fromEnv.Trim();
                }

                return DefaultVolumeMonitorUrl;
            }
        }

        internal static void SetUnlockVolumeMonitorUrl(string volumeMonitorUrl)
        {
            lock (Gate)
            {
                _unlockVolumeMonitorUrl = string.IsNullOrWhiteSpace(volumeMonitorUrl) ? null : volumeMonitorUrl.Trim();
            }
        }

        internal static bool ActivityVerbose
        {
            get
            {
                return true;// Environment.GetEnvironmentVariable("GHOSTSCRIPT_NET_OFFICE_ACTIVITY_VERBOSE") == "1";
            }
        }

        internal static bool IsRunning
        {
            get
            {
                lock (Gate)
                {
                    for (int i = 0; i < Live.Count; i++)
                    {
                        if (Live[i]._worker.IsAlive)
                            return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Start a sender for this Ghostscript call. Always creates a new worker.
        /// A later call must not reuse a worker that is still flushing EOF.
        /// </summary>
        internal static ActivityMonitor Start(string keyString)
        {
            if (string.IsNullOrEmpty(keyString))
                throw new ArgumentException("Volume-license activity requires a key.", "keyString");

            ActivityMonitor monitor = new ActivityMonitor(keyString);
            lock (Gate)
            {
                EnsureTls();
                HookProcessExit();
                Live.Add(monitor);
            }

            if (ActivityVerbose)
            {
                Console.WriteLine("Ghostscript.NET Office: Starting activity monitoring.");
                Console.Out.Flush();
            }

            monitor._worker.Start();
            return monitor;
        }

        ActivityMonitor(string keyString)
        {
            _key = keyString;
            _worker = new Thread(QueueLoop);
            // Background so a hung POST cannot keep the process alive after Main returns.
            // ProcessExit still calls FlushForExit and waits up to ~12 seconds.
            _worker.IsBackground = true;
            _worker.Name = "GhostscriptOfficeActivity";
        }

        internal void RecordDocument()
        {
            try
            {
                _queue.Add(ActivityNewDoc);
            }
            catch (InvalidOperationException)
            {
                /* worker is shutting down */
            }
        }

        /// <summary>
        /// This call's pipe reached EOF. Send whatever is still queued and stop this worker.
        /// Does not wait for the HTTP call.
        /// </summary>
        internal void End()
        {
            try
            {
                _queue.Add(null);
            }
            catch (InvalidOperationException)
            {
                /* already completed */
            }
        }

        /// <summary>
        /// Send any queued document counts and stop every live worker.
        /// </summary>
        internal static void FlushForExit()
        {
            ActivityMonitor[] snapshot;
            lock (Gate)
                snapshot = Live.ToArray();

            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i].End();

            int joinMs = ShutdownFlushSeconds * 1000 + 2000;
            int deadline = Environment.TickCount + joinMs;
            for (int i = 0; i < snapshot.Length; i++)
            {
                int left = deadline - Environment.TickCount;
                if (left < 1)
                    left = 1;
                if (!snapshot[i]._worker.Join(left))
                    WriteShutdownFailed();
            }

            lock (Gate)
            {
                for (int i = 0; i < snapshot.Length; i++)
                    Live.Remove(snapshot[i]);
            }
        }

        void QueueLoop()
        {
            try
            {
                BlockingCollection<int?> queue = _queue;
                string key = _key;
                List<int> items = new List<int>();
                bool first = true;
                while (true)
                {
                    object end = 0;
                    if (first)
                    {
                        // Block until the first document (or EOF with nothing to send).
                        int? item;
                        try
                        {
                            item = queue.Take();
                        }
                        catch (InvalidOperationException)
                        {
                            return;
                        }

                        if (item == null)
                            return;

                        items.Add(item.Value);
                        // If GhostPDL already closed the pipe, send once as EOF instead
                        // of an immediate POST plus a second empty flush.
                        if (DrainReady(queue, items))
                            end = "EOF";
                        first = false;
                    }
                    else
                    {
                        double endTime = Seconds() + BatchSeconds;
                        while (true)
                        {
                            int timeoutMs = (int)Math.Max(0, (endTime - Seconds()) * 1000);
                            int? item;
                            if (!queue.TryTake(out item, timeoutMs))
                                break;
                            if (item == null)
                            {
                                end = "EOF";
                                break;
                            }
                            items.Add(item.Value);
                        }
                    }

                    if (items.Count > 0)
                    {
                        if (ActivityVerbose)
                        {
                            Console.WriteLine("Activity flush to server: " + FormatEnd(end) + ".");
                            for (int i = 0; i < items.Count; i++)
                                Console.WriteLine("Activity to server: item=" + items[i] + ".");
                            Console.Out.Flush();
                        }
                        Send(key, items, end);
                    }

                    if (!IsFalsey(end))
                        break;
                }
            }
            finally
            {
                lock (Gate)
                    Live.Remove(this);
            }
        }

        static bool DrainReady(BlockingCollection<int?> queue, List<int> items)
        {
            int? item;
            while (queue.TryTake(out item, 0))
            {
                if (item == null)
                    return true;
                items.Add(item.Value);
            }

            return false;
        }

        static void Send(string keyString, List<int> items, object end)
        {
            string dataStr =
                "{\"key_code\": \"" + keyString + "\", \"document_count\": " + items.Count + "}";
            const string headersRepr = "{'Content-Type': 'application/json'}";

            if (ActivityVerbose)
            {
                Console.WriteLine(
                    "Sending: headers=" + headersRepr + " data_str='" + dataStr + "'.");
                Console.Out.Flush();
            }

            bool retryUntilOk = !IsFalsey(end);
            double deadline = retryUntilOk
                ? Seconds() + ShutdownFlushSeconds
                : double.PositiveInfinity;

            while (true)
            {
                double remaining = deadline - Seconds();
                if (retryUntilOk && remaining <= 0)
                {
                    WriteShutdownFailed();
                    break;
                }

                try
                {
                    TimeSpan attempt = retryUntilOk
                        ? TimeSpan.FromSeconds(Math.Max(0.01, remaining))
                        : TimeSpan.FromSeconds(MidRunPostSeconds);
                    PostUsage(dataStr, attempt);
                    if (ActivityVerbose)
                    {
                        Console.WriteLine(
                            "Sent to url='" + VolumeMonitorUrl + "': data_str='" + dataStr + "'");
                        Console.Out.Flush();
                    }
                    items.Clear();
                }
                catch (Exception e)
                {
                    if (ActivityVerbose)
                    {
                        Console.WriteLine(
                            "Failed to send to url='" + VolumeMonitorUrl + "': " + OneLine(e));
                        Console.Out.Flush();
                    }
                    if (retryUntilOk)
                    {
                        remaining = deadline - Seconds();
                        if (remaining <= 0)
                        {
                            WriteShutdownFailed();
                            break;
                        }
                        if (ActivityVerbose)
                        {
                            Console.WriteLine(FormatEnd(end) + ": retrying...");
                            Console.Out.Flush();
                        }
                        int sleepMs = (int)Math.Min(1000, remaining * 1000);
                        if (sleepMs > 0)
                            Thread.Sleep(sleepMs);
                        continue;
                    }
                }
                break;
            }
        }

        static void WriteShutdownFailed()
        {
            if (Interlocked.Exchange(ref _shutdownFailedPrinted, 1) != 0)
                return;
            Console.WriteLine(ShutdownFlushFailedMessage);
            Console.Out.Flush();
            Console.Error.WriteLine(ShutdownFlushFailedMessage);
            Console.Error.Flush();
        }

        static HttpClient CreateHttpClient()
        {
            EnsureTls();
            return new HttpClient { Timeout = TimeSpan.FromSeconds(MidRunPostSeconds) };
        }

        static void PostUsage(string dataStr, TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
                throw new TimeoutException("Activity flush timed out.");
            using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
            using (StringContent content = new StringContent(dataStr, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = Http.PostAsync(VolumeMonitorUrl, content, cts.Token)
                    .GetAwaiter()
                    .GetResult();
                using (response)
                    response.EnsureSuccessStatusCode();
            }
        }

        static void HookProcessExit()
        {
            if (_exitHooked)
                return;
            _exitHooked = true;
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                try { FlushForExit(); }
                catch { /* process is exiting */ }
            };
        }

        static void EnsureTls()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            catch
            {
                /* older runtimes may reject unknown protocol flags */
            }
        }

        static double Seconds()
        {
            return (DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        static bool IsFalsey(object end)
        {
            return end is int && (int)end == 0;
        }

        static string FormatEnd(object end)
        {
            string s = end as string;
            if (s != null)
                return "end='" + s + "'";
            return "end=" + end;
        }

        static string OneLine(Exception e)
        {
            string text = e.Message ?? e.GetType().Name;
            return text.Replace("\r", " ").Replace("\n", " ");
        }
    }
}
