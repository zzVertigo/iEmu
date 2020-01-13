# iEmu
Simple and no where near ready for use iPod 2G emulator

Current State: (Hypothetical and not confirmed) Loads LLB after setting up Clock0/1, GPIO, GPIOIC, Power, VIC, WDT, Timer, SPI and USBPhy. Proceeds to setup(?) SHA1 (which will end up being pretty useless considering that in v1.0 of iPhoneOS the signature checks were skipped lmao)

A picture of what seems to be data written by the iPhone into SRAM
![SRAM](https://i.imgur.com/rj5ZFpx.png)
