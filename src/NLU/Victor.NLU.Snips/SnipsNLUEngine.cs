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
    public class SnipsNLUEngine : NLUEngine
    {
        #region Constructors
        public SnipsNLUEngine(string engineDir, CancellationToken ct) : base(ct)
        {
            EngineDir = engineDir;
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

        public SnipsNLUEngine(string engineDir) : this(engineDir, Ct) { }
        #endregion

        #region Properties
        public IntPtr EnginePtr { get; }

        public string EngineDir { get; }
        #endregion

        #region Methods
        public void GetSnipsIntents(string input, out string[] intents, out string slots_json, out string error)
        {
            ThrowIfNotInitialized();
            error = "";
            intents = Array.Empty<string>();
            slots_json = "";
            if (SnipsApi.GetIntents(EnginePtr, input, out libsnips.CIntentClassifierResult[] results) && results.Count() > 0)
            {
                intents = results.Select(r => (string.IsNullOrEmpty(r.intent_name) ? "None" : r.intent_name) + ":" + r.confidence_score.ToString("N2")).ToArray();
                var topIntent = results.OrderByDescending(r => r.confidence_score).First();
                if (!string.IsNullOrEmpty(topIntent.intent_name))
                {
                    SnipsApi.GetSlotsIntoJson(EnginePtr, input, topIntent.intent_name, out slots_json, out error);
                }
            }
        }

        public SnipsIntents GetSnipsIntents(string input)
        {
            ThrowIfNotInitialized();
            GetSnipsIntents(input, out string[] scores, out string json, out string error);
            var entities = !string.IsNullOrEmpty(json) ? SnipsIntentEntity.FromJson(json) : Array.Empty<SnipsIntentEntity>();
            return new SnipsIntents(scores, entities, input);
        }

        public override Intent GetIntent(string input)
        {
            var snipsIntents = GetSnipsIntents(input);
            return new Intent(input, snipsIntents.Scores.Select(s => new IntentScore(s.Item1, s.Item2)),
                snipsIntents.Entities.Select(e => new IntentEntity(e.RawValue, e.Value.ValueValue, e.Alternatives, e.Entity, e.Value.Kind, e.SlotName)));
        }

        public static void DownloadSnipsNativeLibIfMissing(string assemblyDirectory)
        {
            string libLinuxUrl = "https://drive.google.com/uc?id=1adxadKT2inu6I4pI5iahUr86sUY4cBVA&export=download";
            string libWindowsUrl = "https://allisterb-victor.s3.us-east-2.amazonaws.com/snips_nlu_ffi.dll";
            if (Environment.OSVersion.Platform == PlatformID.Unix && !File.Exists(Path.Combine(assemblyDirectory, "snips_nlu_ffi.so")))
            {
                using (var op = Begin("Downloading SnipsNLU native lib for RHEL 7 at {0} to {1}", libLinuxUrl, assemblyDirectory))
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
                using (var op = Begin("Downloading SnipsNLU native lib for Windows to {0}", assemblyDirectory))
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
