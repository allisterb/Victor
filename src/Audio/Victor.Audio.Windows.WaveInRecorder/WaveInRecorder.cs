using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Victor
{
    public delegate void BufferDoneEventHandler(IntPtr data, int size);

    public class WindowsWaveInRecorder : IDisposable
    {
        #region Constructors
        public WindowsWaveInRecorder(int device, Windows.WaveFormat format, int bufferSize, int bufferCount, BufferDoneEventHandler doneProc)
        {
            this.doneProc = doneProc;
            Windows.FailOnMMError(Windows.waveInOpen(out waveInPtr, device, format, bufferProc, 0, Windows.CALLBACK_FUNCTION));
            AllocateBuffers(bufferSize, bufferCount);
            for (int i = 0; i < bufferCount; i++)
            {
                SelectNextBuffer();
                currentBuffer.Record();
            }
            Windows.FailOnMMError(Windows.waveInStart(waveInPtr));
            thread = new Thread(new ThreadStart(ThreadProc));
            thread.Start();
        }
        #endregion

        #region Properties
        public static int DeviceCount => Windows.waveInGetNumDevs();
        #endregion

        #region Disposer and Finalizer
        public void Dispose()
        {
            if (thread != null)
                try
                {
                    finished = true;
                    if (waveInPtr != IntPtr.Zero)
                        Windows.waveInReset(waveInPtr);
                    WaitForAllBuffers();
                    thread.Join();
                    doneProc = null;
                    FreeBuffers();
                    if (waveInPtr != IntPtr.Zero)
                        Windows.waveInClose(waveInPtr);
                }
                finally
                {
                    thread = null;
                    waveInPtr = IntPtr.Zero;
                }
            GC.SuppressFinalize(this);
        }

        ~WindowsWaveInRecorder()
        {
            Dispose();
        }

        #endregion

        #region Methods
        private void ThreadProc()
        {
            while (!finished)
            {
                Advance();
                if (doneProc != null && !finished)
                    doneProc(currentBuffer.Data, currentBuffer.Size);
                currentBuffer.Record();
            }
        }
        private void AllocateBuffers(int bufferSize, int bufferCount)
        {
            FreeBuffers();
            if (bufferCount > 0)
            {
                buffers = new WaveInBuffer(waveInPtr, bufferSize);
                WaveInBuffer Prev = buffers;
                try
                {
                    for (int i = 1; i < bufferCount; i++)
                    {
                        WaveInBuffer Buf = new WaveInBuffer(waveInPtr, bufferSize);
                        Prev.NextBuffer = Buf;
                        Prev = Buf;
                    }
                }
                finally
                {
                    Prev.NextBuffer = buffers;
                }
            }
        }
        private void FreeBuffers()
        {
            currentBuffer = null;
            if (buffers != null)
            {
                WaveInBuffer First = buffers;
                buffers = null;

                WaveInBuffer Current = First;
                do
                {
                    WaveInBuffer Next = Current.NextBuffer;
                    Current.Dispose();
                    Current = Next;
                } while (Current != First);
            }
        }
        private void Advance()
        {
            SelectNextBuffer();
            currentBuffer.WaitFor();
        }
        private void SelectNextBuffer()
        {
            currentBuffer = currentBuffer == null ? buffers : currentBuffer.NextBuffer;
        }
        private void WaitForAllBuffers()
        {
            WaveInBuffer Buf = buffers;
            while (Buf.NextBuffer != buffers)
            {
                Buf.WaitFor();
                Buf = Buf.NextBuffer;
            }
        }
        #endregion

        #region Fields
        private IntPtr waveInPtr;
        private WaveInBuffer buffers;
        private WaveInBuffer currentBuffer;
        private Thread thread;
        private BufferDoneEventHandler doneProc;
        private bool finished;
        private Windows.WaveDelegate bufferProc = new Windows.WaveDelegate(WaveInBuffer.WaveInProc);
        #endregion
    }

    internal class WaveInBuffer : IDisposable
    {
        #region Constructors
        public WaveInBuffer(IntPtr waveInHandle, int size)
        {
            waveInPtr = waveInHandle;

            headerHandle = GCHandle.Alloc(header, GCHandleType.Pinned);
            header.dwUser = (IntPtr)GCHandle.Alloc(this);
            headerData = new byte[size];
            headerDataHandle = GCHandle.Alloc(headerData, GCHandleType.Pinned);
            header.lpData = headerDataHandle.AddrOfPinnedObject();
            header.dwBufferLength = size;
            Windows.FailOnMMError(Windows.waveInPrepareHeader(waveInPtr, ref header, Marshal.SizeOf(header)));
        }
        #endregion

        #region Disposer and Finalizer
        ~WaveInBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (header.lpData != IntPtr.Zero)
            {
                Windows.waveInUnprepareHeader(waveInPtr, ref header, Marshal.SizeOf(header));
                headerHandle.Free();
                header.lpData = IntPtr.Zero;
            }
            recordEvent.Close();
            if (headerDataHandle.IsAllocated)
                headerDataHandle.Free();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Properties
        public int Size => header.dwBufferLength;
        #endregion

        #region Methods
        internal static void WaveInProc(IntPtr hdrvr, int uMsg, int dwUser, ref Windows.WaveHdr wavhdr, int dwParam2)
        {
            if (uMsg == Windows.MM_WIM_DATA)
            {
                try
                {
                    GCHandle h = (GCHandle)wavhdr.dwUser;
                    WaveInBuffer buf = (WaveInBuffer)h.Target;
                    buf.OnCompleted();
                }
                catch
                {
                }
            }
        }

        public IntPtr Data
        {
            get { return header.lpData; }
        }

        public bool Record()
        {
            lock (this)
            {
                recordEvent.Reset();
                isRecording = Windows.waveInAddBuffer(waveInPtr, ref header, Marshal.SizeOf(header)) == Windows.MMSYSERR_NOERROR;
                return isRecording;
            }
        }

        public void WaitFor()
        {
            if (isRecording)
                isRecording = recordEvent.WaitOne();
            else
                Thread.Sleep(0);
        }

        private void OnCompleted()
        {
            recordEvent.Set();
            isRecording = false;
        }
        #endregion

        #region Fields
        public WaveInBuffer NextBuffer;
        private AutoResetEvent recordEvent = new AutoResetEvent(false);
        private IntPtr waveInPtr;
        private Windows.WaveHdr header;
        private byte[] headerData;
        private GCHandle headerHandle;
        private GCHandle headerDataHandle;
        private bool isRecording;
        #endregion
    }

}
