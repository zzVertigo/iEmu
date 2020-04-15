using Apollo.ARM11;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Apollo.iPhone
{
    public class Memory : IBus
    {
        const int KB = 1024;
        const int MB = 1024 * KB;

        public byte[] BootRom;
        public byte[] LowRam;
        public byte[] SRam;

        public StreamWriter logFile;

        private WDT _wdt;
        private Power _power;
        private Clock1 _clock1;
        private VIC[] _vic;

        public Memory(Emulator emulator)
        {
            LowRam = new byte[0x08000000];
            BootRom = new byte[0x10000];
            SRam = new byte[0x00500000];

            _wdt = new WDT();
            _power = new Power();
            _clock1 = new Clock1();

            _vic = new VIC[2];
            _vic[0] = new VIC();
            _vic[1] = new VIC();

            logFile = File.CreateText("io.txt");
        }

        public uint ReadUInt32(uint Address)
        {
            Address &= 0xfffffffc;

            if (Address < 0x08000000)
            {
                uint value = (uint)(LowRam[(Address + 0) & 0x7ffffff] | (LowRam[(Address + 1) & 0x7ffffff] << 8) | (LowRam[(Address + 2) & 0x7ffffff] << 16) | (LowRam[(Address + 3) & 0x7ffffff] << 24));

                return value;
            }
            else if (InRange(Address, 0x08000000, 0x10000000))
            {
                Console.WriteLine("Ram read");
            }
            else if (InRange(Address, 0x20000000, 0x20010000))
            {
                uint value = (uint)(BootRom[(Address + 0) - 0x20000000] | (BootRom[(Address + 1) - 0x20000000] << 8) | (BootRom[(Address + 2) - 0x20000000] << 16) | (BootRom[(Address + 3) - 0x20000000] << 24));

                return value;
            }
            else if (InRange(Address, 0x22000000, 0x22500000))
            {
                uint value = (uint)(SRam[(Address + 0) - 0x22000000] | (SRam[(Address + 1) - 0x22000000] << 8) | (SRam[(Address + 2) - 0x22000000] << 16) | (SRam[(Address + 3) - 0x22000000] << 24));

                return value;
            }
            else if (InRange(Address, 0x38000000, 0x40000000)) // extra check
            {
                uint peripheralsAddr = Address - 0x38000000;

                if (peripheralsAddr < 0x1000)
                {
                    Console.WriteLine("iPhone > SHA1 Read");
                }
                else if (InRange(peripheralsAddr, 0x00100000, 0x00101000))
                {
                    Console.WriteLine("iPhone > Clock 0 Read");
                }
                else if (InRange(peripheralsAddr, 0x00200000, 0x00201000))
                {
                    Console.WriteLine("iPhone > DMAC 0 Read");
                }
                else if (InRange(peripheralsAddr, 0x00400000, 0x00403000))
                {
                    Console.WriteLine("iPhone > USB Otg Read");
                }
                else if (InRange(peripheralsAddr, 0x00c00000, 0x00c01000))
                {
                    Console.WriteLine("iPhone > AES Read");
                }
                else if (InRange(peripheralsAddr, 0x00e00000, 0x00e01000))
                {
                    Console.WriteLine("iPhone > VIC[0] Read");
                }
                else if (InRange(peripheralsAddr, 0x00e01000, 0x00e02000))
                {
                    Console.WriteLine("iPhone > VIC[1] Read");
                }
                else if (InRange(peripheralsAddr, 0x00e02000, 0x00e03000))
                {
                    Console.WriteLine("iPhone > Edge IC Read");
                }
                else if (InRange(peripheralsAddr, 0x01900000, 0x01901000))
                {
                    Console.WriteLine("iPhone > DMAC 1 Read");
                }
                else if (InRange(peripheralsAddr, 0x01a00000, 0x01a00080))
                {
                    return _power.ProcessRead(peripheralsAddr - 0x01a00000);
                }
                else if (InRange(peripheralsAddr, 0x01a00080, 0x01a00100))
                {
                    Console.WriteLine("iPhone > GPIOIC Read");
                }
                else if (InRange(peripheralsAddr, 0x04300000, 0x04300100))
                {
                    Console.WriteLine("iPhone > SPI 0 Read");
                }
                else if (InRange(peripheralsAddr, 0x04400000, 0x04401000))
                {
                    Console.WriteLine("iPhone > USB Phy Read");
                }
                else if (InRange(peripheralsAddr, 0x04500000, 0x04501000))
                {
                    return _clock1.ProcessRead(peripheralsAddr - 0x04500000);
                }
                else if (InRange(peripheralsAddr, 0x04e00000, 0x04e00100))
                {
                    Console.WriteLine("iPhone > SPI[1] Read");
                }
                else if (InRange(peripheralsAddr, 0x05200000, 0x05200100))
                {
                    Console.WriteLine("iPhone > SPI[2] Read");
                }
                else if (InRange(peripheralsAddr, 0x06200000, 0x06220000))
                {
                    Console.WriteLine("iPhone > Timers Read");
                }
                else if (InRange(peripheralsAddr, 0x06300000, 0x06301000))
                {
                    return _wdt.ProcessRead(peripheralsAddr - 0x06300000);
                }
                else if (InRange(peripheralsAddr, 0x06400000, 0x06401000))
                {
                    Console.WriteLine("iPhone > GPIO Read");
                }

                return 0;
            }

            return 0;
        }

        public void WriteUInt32(uint Address, uint Value)
        {
            Address &= 0xfffffffc;

            if (Address < 0x08000000)
            {
                Console.WriteLine("LowRam write");
            }
            else if (InRange(Address, 0x08000000, 0x10000000))
            {
                Console.WriteLine("Ram write");
            }
            else if (InRange(Address, 0x22000000, 0x22500000))
            {
                Console.WriteLine("SRAM Write at " + (Address - 0x22000000).ToString("X8") + " : " + Value.ToString("X8"));

                SRam[(Address + 0) - 0x22000000] = (byte)((Value >> 0) & 0xFF);
                SRam[(Address + 1) - 0x22000000] = (byte)((Value >> 8) & 0xFF);
                SRam[(Address + 2) - 0x22000000] = (byte)((Value >> 16) & 0xFF);
                SRam[(Address + 3) - 0x22000000] = (byte)((Value >> 24) & 0xFF);
            }
            else if (InRange(Address, 0x38000000, 0x40000000)) // extra check
            {
                uint peripheralsAddr = Address - 0x38000000;

                if (peripheralsAddr < 0x1000)
                {
                    Console.WriteLine("iPhone > SHA1 Write");
                }
                else if (InRange(peripheralsAddr, 0x00100000, 0x00101000))
                {
                    Console.WriteLine("iPhone > Clock 0 Write");
                }
                else if (InRange(peripheralsAddr, 0x00200000, 0x00201000))
                {
                    Console.WriteLine("iPhone > DMAC 0 Write");
                }
                else if (InRange(peripheralsAddr, 0x00400000, 0x00403000))
                {
                    Console.WriteLine("iPhone > USB Otg Write");
                }
                else if (InRange(peripheralsAddr, 0x00c00000, 0x00c01000))
                {
                    Console.WriteLine("iPhone > AES Write");
                }
                else if (InRange(peripheralsAddr, 0x00e00000, 0x00e01000))
                {
                    Console.WriteLine("iPhone > VIC[0] Write");
                }
                else if (InRange(peripheralsAddr, 0x00e01000, 0x00e02000))
                {
                    Console.WriteLine("iPhone > VIC[1] Write");
                }
                else if (InRange(peripheralsAddr, 0x00e02000, 0x00e03000))
                {
                    Console.WriteLine("iPhone > Edge IC Write");
                }
                else if (InRange(peripheralsAddr, 0x01900000, 0x01901000))
                {
                    Console.WriteLine("iPhone > DMAC 1 Write");
                }
                else if (InRange(peripheralsAddr, 0x01a00000, 0x01a00080))
                {
                    _power.ProcessWrite(peripheralsAddr - 0x01a00000, Value);
                }
                else if (InRange(peripheralsAddr, 0x01a00080, 0x01a00100))
                {
                    Console.WriteLine("iPhone > GPIOIC Write");
                }
                else if (InRange(peripheralsAddr, 0x04300000, 0x04300100))
                {
                    Console.WriteLine("iPhone > SPI 0 Write");
                }
                else if (InRange(peripheralsAddr, 0x04400000, 0x04401000))
                {
                    Console.WriteLine("iPhone > USB Phy Write");
                }
                else if (InRange(peripheralsAddr, 0x04500000, 0x04501000))
                {
                    _clock1.ProcessWrite(peripheralsAddr - 0x04500000, Value);
                }
                else if (InRange(peripheralsAddr, 0x04e00000, 0x04e00100))
                {
                    Console.WriteLine("iPhone > SPI[1] Write");
                }
                else if (InRange(peripheralsAddr, 0x05200000, 0x05200100))
                {
                    Console.WriteLine("iPhone > SPI[2] Write");
                }
                else if (InRange(peripheralsAddr, 0x06200000, 0x06220000))
                {
                    Console.WriteLine("iPhone > Timers Write");
                }
                else if (InRange(peripheralsAddr, 0x06300000, 0x06301000))
                {
                    _wdt.ProcessWrite(peripheralsAddr - 0x06300000, Value);
                }
                else if (InRange(peripheralsAddr, 0x06400000, 0x06401000))
                {
                    Console.WriteLine("iPhone > GPIO Write");
                }
            }
        }

        public bool InRange(uint x, uint y, uint z)
        {
            if (x >= y && x < z)
                return true;
            else
                return false;
        }

        public void Tick()
        {
            _wdt.wdtTick();
        }
    }
}
