using System;
using System.Collections.Generic;
using System.Text;

namespace Victor
{
    public class WinMMException : Exception
    {
        public WinMMException(string message) : base($"WinMM exception: {message}") { }
    }
}
