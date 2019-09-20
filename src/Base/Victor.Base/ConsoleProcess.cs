using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
namespace Victor
{
    public delegate void OnExit(Process P);

    public delegate void OnOutput(string line);

    public delegate void OnError(string line);

    public class ConsoleProcess : Api
    {
        #region Constructors
        public ConsoleProcess(string cmd, string[] args, bool relativeToAssemblyDir = true, OnExit onExit = null, OnOutput onOutput = null,  OnError onError = null)
        {
            if (!File.Exists(cmd))
            {
                return;
            }
            Process = new Process();
            Cmd = relativeToAssemblyDir ? Path.Combine(AssemblyDirectory.FullName, cmd) : cmd;
            Args = args;
            OnExit = onExit;
            OnOutput = onOutput;
            OnError = onError;
            Process.StartInfo = new ProcessStartInfo(cmd, string.Join(" ", args))
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true
            };
            Process.EnableRaisingEvents = true;
            Process.OutputDataReceived += Process_OutputDataReceived;
            Process.ErrorDataReceived += Process_ErrorDataReceived; 
            Process.Exited += Process_Exited;

            Initialized = true;
        }
        #endregion

        #region Event Handlers
        private void Process_Exited(object sender, EventArgs e)
        {
            OnExit?.Invoke(this.Process);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug(e.Data);
                ErrorBuilder.AppendLine(e.Data);
                OnError?.Invoke(e.Data);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        { 
            if (CancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug(e.Data);
                OutputBuilder.AppendLine(e.Data);
                OnOutput?.Invoke(e.Data);
            }
        }
        #endregion

        #region Properties
        public Process Process { get; protected set; }

        public string Cmd { get; }

        public string[] Args { get; }

        public bool IsStarted { get; protected set; }

        public bool? HasExited => Process?.HasExited;

        public string Output => OutputBuilder.ToString();

        StringBuilder OutputBuilder { get; } = new StringBuilder();

        StringBuilder ErrorBuilder { get; } = new StringBuilder();

        public OnExit OnExit { get; protected set; }

        public OnError OnError { get; protected set; }

        public OnOutput OnOutput { get; protected set; }
        #endregion

        #region Methods
        public void Start()
        {
            ThrowIfNotInitialized();
            Process.Start();
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
            IsStarted = true;
            Info("Process {0} started.", Cmd);
        }

        public void Stop()
        {
            ThrowIfNotInitialized();
            ThrowIfNotStarted();
            Process.Kill();
            Process.Dispose();
            Info("Process {0} stopped.", Cmd);
        }

        public void WaitForExit()
        {
            ThrowIfNotInitialized();
            ThrowIfNotStarted();
            Process.WaitForExit();
        }

        public void WaitForInputIdle()
        {
            ThrowIfNotInitialized();
            ThrowIfNotStarted();
            Process.WaitForInputIdle();
        }

        protected void ThrowIfNotStarted()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException($"Process {Process.StartInfo.FileName} not started.");
            }
        }
        #endregion

        #region Fields
        private static Mutex mutex = new Mutex();

        private static Mutex mutex2 = new Mutex();
        #endregion
    }
}
