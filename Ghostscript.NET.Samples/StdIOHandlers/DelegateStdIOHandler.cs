using System;

namespace Ghostscript.NET.Samples.StdIOHandlers
{
    public class DelegateStdIOHandler : GhostscriptStdIO
    {
        readonly Func<int, string> _stdIn;
        readonly Action<string> _stdOut;
        readonly Action<string> _stdErr;

        public DelegateStdIOHandler(
            Func<int, string> stdIn = null,
            Action<string> stdOut = null,
            Action<string> stdErr = null) :
                base(stdIn != null, stdOut != null, stdErr != null)
        {
            _stdIn = stdIn;
            _stdOut = stdOut;
            _stdErr = stdErr;
        }

        public override void StdIn(out string input, int count)
        {
            input = _stdIn == null
                ? null
                : _stdIn(count);
        }

        public override void StdOut(string output)
        {
            if (_stdOut != null)
            {
                _stdOut(output);
            }
        }

        public override void StdError(string error)
        {
            if (_stdErr != null)
            {
                _stdErr(error);
            }
        }
    }
}