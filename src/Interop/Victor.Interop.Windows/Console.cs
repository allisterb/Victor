using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Victor
{
    public partial class Windows
    {
        public delegate bool SetConsoleFont(
        IntPtr hWnd,
        uint DWORD
    );

        public delegate uint GetNumberOfConsoleFonts();

        public delegate bool GetConsoleFontInfo(
            IntPtr hWnd,
            bool BOOL,
            uint DWORD,
            [Out] CONSOLE_FONT_INFO[] ConsoleFontInfo
        );


        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_FONT_INFO
        {
            public uint nFont;
            public COORD dwFontSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandleA(
            string module
        );

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(
            int nStdHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            out CONSOLE_FONT_INFO lpConsoleCurrentFont
            );

        [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
        ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);
        }
}
