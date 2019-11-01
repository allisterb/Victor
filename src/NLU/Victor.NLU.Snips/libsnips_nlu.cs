using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: InternalsVisibleTo("SnipsNLU.Tests.Interop")]
namespace Victor.SnipsNLU
{
    /// <summary>
    /// Enum representing the grain of a resolved date related value
    /// </summary>
    public enum SNIPS_GRAIN
    {
        /*
         * The resolved value has a granularity of a year
         */
        SNIPS_GRAIN_YEAR = 0,
        /*
         * The resolved value has a granularity of a quarter
         */
        SNIPS_GRAIN_QUARTER = 1,
        /*
         * The resolved value has a granularity of a mount
         */
        SNIPS_GRAIN_MONTH = 2,
        /*
         * The resolved value has a granularity of a week
         */
        SNIPS_GRAIN_WEEK = 3,
        /*
         * The resolved value has a granularity of a day
         */
        SNIPS_GRAIN_DAY = 4,
        /*
         * The resolved value has a granularity of an hour
         */
        SNIPS_GRAIN_HOUR = 5,
        /*
         * The resolved value has a granularity of a minute
         */
        SNIPS_GRAIN_MINUTE = 6,
        /*
         * The resolved value has a granularity of a second
         */
        SNIPS_GRAIN_SECOND = 7,
    }

    public enum SNIPS_PRECISION
    {
        /*
         * The resolved value is approximate
         */
        SNIPS_PRECISION_APPROXIMATE = 0,
        /*
         * The resolved value is exact
         */
        SNIPS_PRECISION_EXACT = 1,
    }

    /// <summary>
    /// Used as a return type of functions that can encounter errors
    /// </summary>
    public enum SNIPS_RESULT {
        /*
         * The function returned successfully
         */
        SNIPS_RESULT_OK = 0,
        /*
         * The function encountered an error, you can retrieve it using the dedicated function
         */
        SNIPS_RESULT_KO = 1,
    }

    /// <summary>
    /// Enum type describing how to cast the value of a CSlotValue
    /// </summary>
    public enum SNIPS_SLOT_VALUE_TYPE
    {
        /*
         * Custom type represented by a char *
         */
        SNIPS_SLOT_VALUE_TYPE_CUSTOM = 1,
        /*
         * Number type represented by a CNumberValue
         */
        SNIPS_SLOT_VALUE_TYPE_NUMBER = 2,
        /*
         * Ordinal type represented by a COrdinalValue
         */
        SNIPS_SLOT_VALUE_TYPE_ORDINAL = 3,
        /*
         * Instant type represented by a CInstantTimeValue
         */
        SNIPS_SLOT_VALUE_TYPE_INSTANTTIME = 4,
        /*
         * Interval type represented by a CTimeIntervalValue
         */
        SNIPS_SLOT_VALUE_TYPE_TIMEINTERVAL = 5,
        /*
         * Amount of money type represented by a CAmountOfMoneyValue
         */
        SNIPS_SLOT_VALUE_TYPE_AMOUNTOFMONEY = 6,
        /*
         * Temperature type represented by a CTemperatureValue
         */
        SNIPS_SLOT_VALUE_TYPE_TEMPERATURE = 7,
        /*
         * Duration type represented by a CDurationValue
         */
        SNIPS_SLOT_VALUE_TYPE_DURATION = 8,
        /*
         * Percentage type represented by a CPercentageValue
         */
        SNIPS_SLOT_VALUE_TYPE_PERCENTAGE = 9,
        /*
         * Music Album type represented by a char *
         */
        SNIPS_SLOT_VALUE_TYPE_MUSICALBUM = 10,
        /*
         * Music Artist type represented by a char *
         */
        SNIPS_SLOT_VALUE_TYPE_MUSICARTIST = 11,
        /*
         * Music Track type represented by a char *
         */
        SNIPS_SLOT_VALUE_TYPE_MUSICTRACK = 12,
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class libsnips
    {
        /// <summary>
        /// API version we are targeting
        /// </summary>
        internal static readonly string SNIPS_NLU_VERSION = "0.65.3";

        /// <summary>
        /// Results of the intent classifier
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CIntentClassifierResult
        {
            /*
             * Name of the intent detected
             */
            [MarshalAs(UnmanagedType.LPStr)]
            internal readonly string intent_name;
        
            /*
             * Between 0 and 1
             */
            internal readonly float confidence_score;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CIntentlassifierResultArray
        {
            internal readonly IntPtr intent_classifer_result_array_ptr;

            internal readonly int size;
        }

        /// <summary>
        /// Results of intent parsing
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CIntentParserResult
        {
            /*
             * The text that was parsed
             */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string input;

            /*
             * The result of intent classification, may be null if no intent was detected
             */
            [MarshalAs(UnmanagedType.LPStruct)]
            readonly CIntentClassifierResult intent;

            /*
             * Name of the intent detected
             */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string intent_intent_name;

            /*
             * Between 0 and 1
             */
            readonly float intent_probability;
            /*
             * The slots extracted if an intent was detected
             */

            //[MarshalAs(UnmanagedType.LPArray)]
            // readonly CSlotList[] slots;
            readonly IntPtr slots;
        }

        /// <summary>
        /// Struct describing a Slot
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CSlotValue
        {
            /*
             * Points to either a *const char, a CNumberValue, a COrdinalValue,
             * a CInstantTimeValue, a CTimeIntervalValue, a CAmountOfMoneyValue,
             * a CTemperatureValue or a CDurationValue depending on value_type
             */
            readonly IntPtr value;

           /*
            * The type of the value
            */
            readonly SNIPS_SLOT_VALUE_TYPE value_type;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CSlotValueArray
        {
            internal readonly IntPtr slot_values_array_ptr;

            internal readonly int size;
        }

        /// <summary>
        /// Struct describing a Slot
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CSlot
        {
            /*
             * The resolved value of the slot
             */
            
            internal readonly IntPtr value;

            /**
            * The alternative slot values
            */
            
            internal readonly IntPtr alternatives;

            /*    
             * The raw value as it appears in the input text
             */
            [MarshalAs(UnmanagedType.LPStr)]
            internal readonly string raw_value;

            /*
            * Name of the entity type of the slot
            */
            [MarshalAs(UnmanagedType.LPStr)]
            internal readonly string entity;
            
            /*
            * Name of the slot
            */
            [MarshalAs(UnmanagedType.LPStr)]
            internal readonly string slot_name;
            
            /*
            * Start index of raw value in input text
            */
            internal readonly int range_start;

           /*
            * End index of raw value in input text
            */
            internal readonly int range_end;

            
            internal readonly float confidence_score;
        }

        /*
        * Wrapper around a slot list
        */
        [StructLayout(LayoutKind.Sequential)]
        internal struct CSlotArray
        {
           /*
            * Pointer to the first slot of the list
            */
            internal readonly IntPtr slots_array_ptr;
           
           /*
            * Number of slots in the list
            */
            internal readonly int size;
        }

        /// <summary>
        /// Representation of an instant value
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CInstantTimeValue
        {
            /*
            * String representation of the instant
            */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string value;
            
            /*
             * The grain of the resolved instant
             */
            readonly SNIPS_GRAIN grain;

            /*
             * The precision of the resolved instant
             */
            readonly SNIPS_PRECISION precision;
        }


        /// <summary>
        /// Representation of an interval value
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CTimeIntervalValue
        {
            /*
             * String representation of the beginning of the interval
             */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string from;

            /*
             * String representation of the end of the interval
             */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string to;
        }


        /// <summary>
        /// Representation of an amount of money value
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CAmountOfMoneyValue
        {
            /*
            * The currency
            */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string unit;
        
            /*
             * The amount of money
             */
            readonly float value;
        
            /*
             * The precision of the resolved value
             */
            readonly SNIPS_PRECISION precision;
        }


        /// <summary>
        /// Representation of a temperature value
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CTemperatureValue
        {
            /*
             * The unit used
             */
            [MarshalAs(UnmanagedType.LPStr)]
            readonly string unit;

        
            /*
             * The temperature resolved
             */
            readonly float value;
        }

        /// <summary>
        /// Representation of a duration value
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CDurationValue
        {
            /*
             * Number of years in the duration
             */
            readonly long years;
            /*
             * Number of quarters in the duration
             */
                readonly long quarters;
            /*
             * Number of months in the duration
             */
            readonly long months;
            /*
             * Number of weeks in the duration
             */
            readonly long weeks;
            /*
             * Number of days in the duration          
             */
            readonly long days;
            /*
             * Number of hours in the duration
             */
            readonly long hours;
            /*
             * Number of minutes in the duration
             */
            readonly long minutes;
            /*  
             * Number of seconds in the duration
             */
            readonly long seconds;
            /*
             * Precision of the resolved value
             */
            readonly SNIPS_PRECISION precision;
        }

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_create_from_dir
            ([In, MarshalAs(UnmanagedType.LPStr)] string root_dir, [In, Out] ref IntPtr client);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_get_last_error
            ([In, Out, MarshalAs(UnmanagedType.LPStr)] ref string error);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_create_from_zip
            ([In, MarshalAs(UnmanagedType.LPStr)] string zip_file, [In] int zip_file_size, [In, Out] ref IntPtr client);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_get_model_version([In, Out] ref IntPtr version);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_run_get_intents([In] IntPtr engine, 
            [In, MarshalAs(UnmanagedType.LPStr)] string input, [In, Out] ref IntPtr result);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_run_get_slots([In] IntPtr engine,
          [In, MarshalAs(UnmanagedType.LPStr)] string input, [In, MarshalAs(UnmanagedType.LPStr)] string intent, [In, Out] ref IntPtr result);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_run_get_slots_with_alternatives([In] IntPtr engine,
            [In, MarshalAs(UnmanagedType.LPStr)] string input, [In, MarshalAs(UnmanagedType.LPStr)] string intent, [Out] uint slots_alternatives, [In, Out] ref IntPtr result);

        [DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_run_get_slots_into_json([In] IntPtr engine,
  [In, MarshalAs(UnmanagedType.LPStr)] string input, [In, MarshalAs(UnmanagedType.LPStr)] string intent, [In, Out] ref string result);

    }
}
