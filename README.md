# Victor - Voice Input Controller

Victor is an free cross-platform programmable voice control program for desktops that is currently in the early stages of development. Victor was started during my entry into the [Mozilla Voice Challenge](https://www.herox.com/voice) to test some ideas for an integrated open-source voice stack. The current release is v0.1.1 and can be downloaded from the [release page](https://github.com/allisterb/Victor/releases/tag/v0.1.1).

The following videos are available demoing and documenting some aspects of Victor (click the screenshot to go to the YouTube video): 

[![Victor Test 1](https://oqlqyg.dm.files.1drv.com/y4myL6ntcHAuxBvE4mz9RcFPsgmFXgu2Fo_BAv6eETqRVt1n7VmqAKPAQIaykLZy6NzzlRx5hJUL8TbPm78Dyp-RBCXE6nJvk3Bv304hXfDX4RXpTPMLB4KpcNX-odIYWTbkCGwXmkuQMAGw8rWexWteVwAHI7RlpWL1AX2MCsxLwTUY_mVydRdhZXTHMSyefXRBcaXqlctZIbO8yQS5cCK-A?width=476&height=315&cropmode=none)](https://youtu.be/Lvw4WmbTTBk "Victor Test 1")

[![Victor Test 2](https://tt6saw.dm.files.1drv.com/y4mm7AvW6iDJzANRlfK5PTWeBc1HzsQlkgQdVNq9h47stgzeAhCO5rkISenEz1QxJpDjFwcfXMpQYoGEBJnr3qNwl9hw2S9w17XHva98P4LwTxuni0eHcgyAVTGOe28OO-FtVQK3u2WsSYxDlqpy0e2M1EXRBPtGhiAHSpl18sk-EgjDKDbb8FVV1lwK5udB4C_CJmzCspiDdrnBtpszV6cvg?width=454&height=272&cropmode=none)](https://youtu.be/LQLpoyohYtE "Victor Test 2")

## Architecture
Victor currently uses the following open-source projects:

### Julius (ASR)
Julius is a hi-speed accurate and flexible LVCSR library whicn can decode and recognize speech in real-time using a variety of models built for different languages like Japanese, English and Polish. Unlike other ASR libraries including Facebook's wav2letter++ Julius is fully supported on Windows and unlike Mozilla's own DeepSpeech, Julis can decode speech waveform input via a system mic device in real-time with appropriate handling of silences and pauses. Julius is used in the [Simon](https://simon.kde.org/) voice control program for KDE. Leslaw Pawlaczyk has [created](https://discourse.mozilla.org/t/julius-speech-models-based-on-mozilla-corpus/27651) Julius models based on the Mozilla corpus and has modified Julius to support DNN-HMM models as well as GMM-HMM. 

Julius can be built as a statically-linked binary and run as a sub-process of Victor. Victor communicates with Julius by monitoring its stdout stream and detecting the different states the program is in:

[![Victor Debug Mode](https://tt4r9w.dm.files.1drv.com/y4m2-x2RJFkx1VaXFHLqEWIUwZhBfEbfKaTlKsCQxKlnboznLObf-BnmLosDEg5Gj7qWA8JIj9wA2wjxVwJsqD4H1agM-WaIF7AfcgLOrnL43DZJ5B9r_KRe-og-njzg2a6SeOdZYGdaKA8INf67y7suXXkeUwWyxkdsnp2eEMlt-Ve-6JJmiMvoG_l3JPe6paKY6U-eNK8rTcItqLKaZ_VoQ?width=475&height=315&cropmode=none)](https://www.youtube.com/watch?v=1PFBRR15F-A "Victor Debug Mode")

The desired Julius configuration is specified in a plain text file and passed to the Julius executable as a startup argument. In this way Julius can be used by any program on any hardware or operation system platform supported by Julius. Julius's portability and real-time input recognition capabilities make it a good choice for the ASR component of an integrated voice stack.

### SnipsNLU (NLU)
[Snips NLU](https://github.com/snipsco/snips-nlu-rs) is a hi-speed accurate open-source NLU inference engine which can recognize intents and entities in utterances for a particular domain in real-time. It is written in Rust and has an FFI allowing it to be used by any language that call C libraries. 
Victor [interfaces](https://github.com/allisterb/Victor/blob/master/src/NLU/Victor.NLU.Snips/SnipsApi.cs) with the Snips NLU engine using its C FFI e.g in C# calling a SnipsNLU function in a native DLL looks like:
```
[DllImport("snips_nlu_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SNIPS_RESULT snips_nlu_engine_create_from_dir
            ([In, MarshalAs(UnmanagedType.LPStr)] string root_dir, [In, Out] ref IntPtr client);
```
Abstractions over the lower-level Snips functions are built-up to avoid other code having to manage the details of calling the library code. This is the standard procedure used for Snips bindings to other languages like Python. This ability to interface with the Snips library directly removes the need for an intermediate Python interpreter or REST API makes SnipsNLU a good choice for the NLU component of an integrated voice stack.

### Mimic (TTS)
