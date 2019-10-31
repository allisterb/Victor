using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Victor.SnipsNLU;

namespace Victor
{
    public class SnipsNLUEngine: Api
    {
        #region Constructors
        public SnipsNLUEngine(string engineDir, CancellationToken ct) : base(ct)
        {
            EngineDir = Path.Combine("Engines", engineDir);
            if (!Directory.Exists(EngineDir))
            {
                Error("The directory {0} does not exist.", EngineDir);
            }
            if (SnipsApi.CreateEngineFromDir(EngineDir, out IntPtr enginePtr, out string error))
            {
                EnginePtr = enginePtr;
                Initialized = true;
                Info("Created SnipsNLU engine from directory {0}", EngineDir);
            }
            else
            {
                Error("An error occurred creating the Snips NLU engine from directory {0}: {1}.", engineDir, error);
            }
        }

        public SnipsNLUEngine(string engineDir) : this(engineDir, Cts.Token) { }
        #endregion

        #region Properties
        public IntPtr EnginePtr { get; } 

        public string EngineDir { get; }
        #endregion

        #region Methods
        public void GetIntents(string input, out string[] intents, out string json, out string error)
        {
            ThrowIfNotInitialized();
            error = "";
            intents = Array.Empty<string>();
            json = "";
            if (SnipsApi.GetIntents(EnginePtr, input, out libsnips.CIntentClassifierResult[] results) && results.Count() > 0)
            {
                intents = results.Select(r => (string.IsNullOrEmpty(r.intent_name) ? "None" : r.intent_name) + ":" + r.confidence_score.ToString("N2")).ToArray();
                var topIntent = results.OrderByDescending(r => r.confidence_score).First();
                if (!string.IsNullOrEmpty(topIntent.intent_name))
                {
                    SnipsApi.GetSlotsIntoJson(EnginePtr, input, topIntent.intent_name, out json, out error);
                }
            }
        }

        public static void DownloadSnipsNativeLibIfMissing(string assemblyDirectory)
        {
            string libLinuxUrl = "https://allisterb-victor.s3.us-east-2.amazonaws.com/snips_nlu_lib.so";
            string libWindowsUrl = "https://allisterb-victor.s3.us-east-2.amazonaws.com/snips_nlu_ffi.dll";
            if (Environment.OSVersion.Platform == PlatformID.Unix && !File.Exists(Path.Combine(assemblyDirectory, "snips_nlu_ffi.so")))
            {
                using (var op = Begin("Downloading SnipsNLU native lib for RHEL 7"))
                using (HttpResponseMessage response = HttpClient.GetAsync(libLinuxUrl).Result)
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream remoteStream = response.Content.ReadAsStreamAsync().Result)
                    using (Stream fileStream = new FileStream(Path.Combine(assemblyDirectory, "snips_nlu_ffi.so"), FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024 * 100, true))
                    {
                        remoteStream.CopyTo(fileStream);
                        fileStream.Flush();
                        op.Complete();
                    }
                }
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT && !File.Exists(Path.Combine(assemblyDirectory, "snips_nlu_ffi.dll")))
            {
                using (var op = Begin("Downloading SnipsNLU native lib for Windows"))
                using (HttpResponseMessage response = HttpClient.GetAsync(libWindowsUrl).Result)
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream remoteStream = response.Content.ReadAsStreamAsync().Result)
                    using (Stream fileStream = new FileStream(Path.Combine(assemblyDirectory, "snips_nlu_ffi.dll"), FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024 * 100, true))
                    {
                        remoteStream.CopyTo(fileStream);
                        fileStream.Flush();
                        op.Complete();
                    }
                }
            }
        }
        #endregion
    }
}
