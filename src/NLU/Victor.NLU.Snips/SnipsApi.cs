using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using static Victor.SnipsNLU.libsnips;

namespace Victor.SnipsNLU
{
    public static class SnipsApi
    {
        public static bool CreateEngineFromDir(string rootDir, out IntPtr enginePtr, out string error)
        {
            enginePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr))); ;
            try
            { 
                SNIPS_RESULT r = snips_nlu_engine_create_from_dir(rootDir, ref enginePtr);
                error = "";
                return r == SNIPS_RESULT.SNIPS_RESULT_OK;
            }
            catch (Exception e)
            {
                enginePtr.FreeHGlobalIfNotZero();
                error = e.Message;
                return false;
            }
            finally
            {
             
            }
        }

        public static bool CreateEngineFromZipFile(string zipFilePath, out IntPtr enginePtr, out string error)
        {
            error = "";
            if (!File.Exists(zipFilePath))
            {
                error = "File not found.";
                enginePtr = IntPtr.Zero;
                return false;
            }

            enginePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr))); ;
            try
            {
                var f = new FileInfo(zipFilePath);
                SNIPS_RESULT r = snips_nlu_engine_create_from_zip(f.FullName, zipFilePath.Length, ref enginePtr);
                return r == SNIPS_RESULT.SNIPS_RESULT_OK;
            }
            catch (Exception)
            {
                enginePtr.FreeHGlobalIfNotZero();
                return false;
            }
            finally
            {
                snips_nlu_engine_get_last_error(ref error);
            }
        }

        public static bool GetModelVersion(out string version)
        {
            IntPtr versionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            try
            {
                SNIPS_RESULT r = snips_nlu_engine_get_model_version(ref versionPtr);
                version = r == SNIPS_RESULT.SNIPS_RESULT_OK ? Marshal.PtrToStringAnsi(versionPtr) : string.Empty;
                return r == SNIPS_RESULT.SNIPS_RESULT_OK;
            }
            catch  (Exception)
            {
                version = string.Empty;
                return false;
            }
            finally
            {
                versionPtr.FreeHGlobalIfNotZero();
            }
        }

        internal static bool GetIntents(IntPtr enginePtr, string input, out CIntentClassifierResult[] results)
        {
            IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            try
            {
                SNIPS_RESULT r = snips_nlu_engine_run_get_intents(enginePtr, input, ref resultPtr);
                if (r == SNIPS_RESULT.SNIPS_RESULT_OK)
                {
                    CIntentlassifierResultArray result = (CIntentlassifierResultArray) Marshal.PtrToStructure(resultPtr, typeof(CIntentlassifierResultArray));
                    results = MarshalPtrToArray<CIntentClassifierResult>(result.intent_classifer_result_array_ptr, result.size);
                    return true;
                }
                else
                {
                    results = null;
                    return false;
                }
            }
            catch (Exception)
            {
                results = null;
                return false;
            }
            finally
            {
                resultPtr.FreeHGlobalIfNotZero();
            }
        }

        internal static bool GetSlots(IntPtr enginePtr, string input, string intent, out CSlot[] results, out string error)
        {
            error = "";
            IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            try
            {
                SNIPS_RESULT r = snips_nlu_engine_run_get_slots(enginePtr, input, intent, ref resultPtr);
                if (r == SNIPS_RESULT.SNIPS_RESULT_OK)
                {
                    CSlotArray result = (CSlotArray)Marshal.PtrToStructure(resultPtr, typeof(CSlotArray));
                    results = MarshalPtrToArray<CSlot>(result.slots_array_ptr, result.size);
                    return true;
                }
                else
                {
                    results = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                results = null;
                error = e.Message;
                return false;
            }
            finally
            {
                snips_nlu_engine_get_last_error(ref error);
                resultPtr.FreeHGlobalIfNotZero();
            }
        }

        internal static bool GetSlotsIntoJson(IntPtr enginePtr, string input, string intent, out string json, out string error)
        {
            error = "";
            json = "";
            try
            {
                SNIPS_RESULT r = snips_nlu_engine_run_get_slots_into_json(enginePtr, input, intent, ref json);
                if (r == SNIPS_RESULT.SNIPS_RESULT_OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
            finally
            {
                snips_nlu_engine_get_last_error(ref error);
            }
        }

        internal static T[] MarshalPtrToArray<T>(IntPtr ptr, int size) where T : struct
        {
            T[] results = new T[size];
            for (int i = 0; i < size; i++)
            {
                results[i] = Marshal.PtrToStructure<T>(ptr + (i * Marshal.SizeOf<T>()));
            }
            return results;
        }
        
    }
}
