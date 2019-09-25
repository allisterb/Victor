using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Victor.SnipsNLU
{
    public static class IntPtrExtensions
    {
        public static bool IsZero(this IntPtr ptr) => ptr == IntPtr.Zero;

        public static bool IsNotZero(this IntPtr ptr) => !ptr.IsZero();

        public static void Zero(this IntPtr ptr)
        {
            ptr = IntPtr.Zero;
        }

        public static void ThrowIfZero(this IntPtr ptr)
        {

            if (ptr.IsZero()) throw new InvalidOperationException("The pointer is invalid.");
        }

        public static void FreeHGlobalIfNotZero(this IntPtr ptr)
        {
            if (ptr.IsNotZero())
            {
                Marshal.FreeHGlobal(ptr);
                ptr = IntPtr.Zero;
            }
        }
    }
}
