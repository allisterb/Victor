# Victor - Voice Input Controller

Victor is an free cross-platform programmable voice control program for desktops that is currently in the early stages of development. Victor was started during my entry into the [Mozilla Voice Challenge](https://www.herox.com/voice) to test some ideas for an integrated open-source voice stack and currently uses the following open-source projects:

* Julius (ASR) Julius is a hi-speed accurate and flexible LVCSR library whicn can decode and recognize speech in real-time using a variety of models built for different languages like Japanese, English and Polish. Julius can be built as a statically-linked binary and run as a sub-process of Victor. Victor communicates with Julius by monitoring its stdout stream and detecting the different states the program is in.

[![Victor Debug Mode](https://tt4r9w.dm.files.1drv.com/y4m2-x2RJFkx1VaXFHLqEWIUwZhBfEbfKaTlKsCQxKlnboznLObf-BnmLosDEg5Gj7qWA8JIj9wA2wjxVwJsqD4H1agM-WaIF7AfcgLOrnL43DZJ5B9r_KRe-og-njzg2a6SeOdZYGdaKA8INf67y7suXXkeUwWyxkdsnp2eEMlt-Ve-6JJmiMvoG_l3JPe6paKY6U-eNK8rTcItqLKaZ_VoQ?width=475&height=315&cropmode=none)](https://www.youtube.com/watch?v=1PFBRR15F-A "Victor Debug Mode")

* SnipsNLU (NLU) Snips NLU 
* Mimic (TTS)

The current release is v0.1.1 and can be downloaded from the [release page](https://github.com/allisterb/Victor/releases/tag/v0.1.1).

The following videos are available demoing and documenting some aspects of Victor (click the screenshot to go to the YouTube video): 

[![Victor Test 1](https://oqlqyg.dm.files.1drv.com/y4myL6ntcHAuxBvE4mz9RcFPsgmFXgu2Fo_BAv6eETqRVt1n7VmqAKPAQIaykLZy6NzzlRx5hJUL8TbPm78Dyp-RBCXE6nJvk3Bv304hXfDX4RXpTPMLB4KpcNX-odIYWTbkCGwXmkuQMAGw8rWexWteVwAHI7RlpWL1AX2MCsxLwTUY_mVydRdhZXTHMSyefXRBcaXqlctZIbO8yQS5cCK-A?width=476&height=315&cropmode=none)](https://youtu.be/Lvw4WmbTTBk "Victor Test 1")

[![Victor Test 2](https://tt6saw.dm.files.1drv.com/y4mm7AvW6iDJzANRlfK5PTWeBc1HzsQlkgQdVNq9h47stgzeAhCO5rkISenEz1QxJpDjFwcfXMpQYoGEBJnr3qNwl9hw2S9w17XHva98P4LwTxuni0eHcgyAVTGOe28OO-FtVQK3u2WsSYxDlqpy0e2M1EXRBPtGhiAHSpl18sk-EgjDKDbb8FVV1lwK5udB4C_CJmzCspiDdrnBtpszV6cvg?width=454&height=272&cropmode=none)](https://youtu.be/LQLpoyohYtE "Victor Test 2")
