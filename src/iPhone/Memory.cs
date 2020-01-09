using Apollo.ARM11;
using System;
using System.IO;

namespace Apollo.iPhone
{
    public class Memory : IBus
    {
        const uint KB = 1024;
        const uint MB = 1024 * KB;

        public byte[] BootROM;
        public byte[] LLB;
        public byte[] RAM;
        public byte[] LowRam;
        public byte[] NOR;
        public byte[] SRAM;

        public Clock0 Clock0;
        public Clock1 Clock1;
        public Timer Timer;
        public Power Power;
        public USBPhy USBPhy;
        public GPIO GPIO;
        public ChipID ChipID;
        public SPI[] SPI;
        public VIC[] VIC; // Vectored Interrupt Controller
        public WDT WDT; // Watch Dog Timer
        public SHA1 SHA1;

        public StreamWriter logFile;

        public Memory(Emulator emulator)
        {
            BootROM = new byte[0x10000];
            LLB = new byte[0x10000];
            NOR = new byte[0x00100000];
            RAM = new byte[0x08000000];
            SRAM = new byte[0x00500000];
            LowRam = new byte[0x08000000];

            Clock0 = new Clock0();
            Clock1 = new Clock1(emulator);
            Timer = new Timer(emulator);
            Power = new Power();
            USBPhy = new USBPhy();
            GPIO = new GPIO();
            ChipID = new ChipID();

            SPI = new SPI[3];
            VIC = new VIC[2];

            SPI[0] = new SPI();
            SPI[1] = new SPI();
            SPI[2] = new SPI();

            VIC[0] = new VIC();
            VIC[1] = new VIC();

            VIC[0].vic.daisy = VIC[1];

            WDT = new WDT();

            SHA1 = new SHA1();

            logFile = File.CreateText("io.txt");
        }

        public byte ReadUInt8(uint Address)
        {
            if (Address < 0x08000000)
            {
                byte ret = (byte)(this.LowRam[(Address + 0) & 0x7ffffff] | (this.LowRam[(Address + 1) & 0x7ffffff] << 8) | (this.LowRam[(Address + 2) & 0x7ffffff] << 16) | (this.LowRam[(Address + 3) & 0x7ffffff] << 24));

                logFile.WriteLine("LowRam Read at " + Address.ToString("X8") + " -> " + ret.ToString("X8"));

                return ret;
            }
            else if (InRange(Address, 0x08000000, 0x10000000))
            {
                byte ret = (byte)(this.RAM[(Address + 0) - 0x08000000] | (this.RAM[(Address + 1) - 0x08000000] << 8) | (this.RAM[(Address + 2) - 0x08000000] << 16) | (this.RAM[(Address + 3) - 0x08000000] << 24));

                logFile.WriteLine("RAM Read at " + Address.ToString("X8") + " -> " + ret.ToString("X8"));

                if (Address == 0x0fffbf50)
                {
                    logFile.WriteLine("WTF READDDDDDDDDDDD");
                }

                return ret;
            }
            else if (InRange(Address, 0x20000000, 0x20100000))
            {
                return (byte)(this.BootROM[(Address + 0) & 0xffff] | (this.BootROM[(Address + 1) & 0xffff] << 8) | (this.BootROM[(Address + 2) & 0xffff] << 16) | (this.BootROM[(Address + 3) & 0xffff] << 24));
            }
            //else if (InRange(Address, 0x22000000, 0x22010000))
            //{
            //    return (byte)(this.LLB[(Address + 0) - 0x22000000] | (this.LLB[(Address + 1) - 0x22000000] << 8) | (this.LLB[(Address + 2) - 0x22000000] << 16) | (this.LLB[(Address + 3) - 0x22000000] << 24));
            //}
            else if (InRange(Address, 0x22000000, 0x22500000))
            {
                byte ret = (byte)(this.SRAM[(Address + 0) - 0x22000000] | (this.SRAM[(Address + 1) - 0x22000000] << 8) | (this.SRAM[(Address + 2) - 0x22000000] << 16) | (this.SRAM[(Address + 3) - 0x22000000] << 24));

                logFile.WriteLine("SRAM Read at " + Address.ToString("X8") + " -> " + ret.ToString("X8"));

                return ret;
            }
            else if (InRange(Address, 0x24000000, 0x24100000))
            {
                logFile.WriteLine("NOR Read at " + Address.ToString("X8") + " -> 0x0");

                return 0;
            }
            else if (InRange(Address, 0x38000000, 0x40000000))
            {
                uint peripheralsAddr = Address - 0x38000000;

                if (peripheralsAddr < 0x1000)
                {
                    logFile.WriteLine("iPhone: SHA1 Read");

                    // sha1

                    return SHA1.Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x00100000, 0x00101000))
                {
                    logFile.WriteLine("iPhone: Clock0 Read");

                    // clock 0

                    return Clock0.Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x00200000, 0x00201000))
                {
                    logFile.WriteLine("iPhone: DMAC[0] Read");

                    // dmac 0
                }
                else if (InRange(peripheralsAddr, 0x00400000, 0x00403000))
                {
                    logFile.WriteLine("iPhone: USB OTG Write");

                    // usb otg
                }
                else if (InRange(peripheralsAddr, 0x00c00000, 0x00c01000))
                {
                    logFile.WriteLine("iPhone: AES Read");

                    // aes
                }
                else if (InRange(peripheralsAddr, 0x00e00000, 0x00e01000))
                {
                    logFile.WriteLine("iPhone: VIC[0] Read");

                    // vic 0

                    return VIC[0].Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x00e01000, 0x00e02000))
                {
                    logFile.WriteLine("iPhone: VIC[1] Read");

                    // vic 1

                    return VIC[1].Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x00e02000, 0x00e03000))
                {
                    logFile.WriteLine("iPhone: EdgeIC Read");

                    // edgeic
                }
                else if (InRange(peripheralsAddr, 0x01900000, 0x01901000))
                {
                    logFile.WriteLine("iPhone: DMAC[1] Read");

                    // dmac 1
                }
                else if (InRange(peripheralsAddr, 0x01a00000, 0x01a00080))
                {
                    logFile.WriteLine("iPhone: Power Read");

                    // power

                    return Power.Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x01a00080, 0x01a00100))
                {
                    logFile.WriteLine("iPhone: GPIOIC Read");

                    // gpioic
                }
                else if (InRange(peripheralsAddr, 0x04300000, 0x04300100))
                {
                    logFile.WriteLine("iPhone: SPI[0] Read");

                    // spi 0

                    return SPI[0].Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x04400000, 0x04401000))
                {
                    logFile.WriteLine("iPhone: USB Phy Read");

                    // usb phy

                    return USBPhy.Read(Address & 0xfffffffc);
                }
                else if (InRange(peripheralsAddr, 0x04500000, 0x04501000))
                {
                    logFile.WriteLine("iPhone: Clock1 Read");

                    // clock 1

                    return Clock1.Read(Address);
                }
                else if (InRange(peripheralsAddr, 0x04e00000, 0x04e00100))
                {
                    logFile.WriteLine("iPhone: SPI[1] Read");

                    // spi 1

                    return SPI[1].Read(Address);
                }
                else if (InRange(peripheralsAddr, 0x05200000, 0x05200100))
                {
                    logFile.WriteLine("iPhone: SPI[2] Read");

                    // spi 2

                    return SPI[2].Read(Address);
                }
                else if (InRange(peripheralsAddr, 0x06200000, 0x06220000))
                {
                    logFile.WriteLine("iPhone: Timers Read");

                    // timers

                    return Timer.Read(Address);
                }
                else if (InRange(peripheralsAddr, 0x06300000, 0x06301000))
                {
                    logFile.WriteLine("iPhone: WDT Read");

                    // wdt

                    return WDT.Read(Address);
                }
                else if (InRange(peripheralsAddr, 0x06400000, 0x06401000))
                {
                    logFile.WriteLine("iPhone: GPIO Read");

                    // gpio

                    return GPIO.Read(Address);
                }
                else
                {
                    logFile.WriteLine("Unknown peripheral port address 0x" + peripheralsAddr.ToString("X"));
                }
            }
            else if (InRange(Address, 0x80000000, 0x88000000))
            {
                byte ret = (byte)(this.RAM[(Address + 0) - 0x88000000] | (this.RAM[(Address + 1) - 0x88000000] << 8) | (this.RAM[(Address + 2) - 0x88000000] << 16) | (this.RAM[(Address + 3) - 0x88000000] << 24));

                logFile.WriteLine("RAM Read at " + Address.ToString("X8") + " -> " + ret.ToString("X8"));

                return ret;
            }
            else
            {
                Console.WriteLine("Debug: unknown read from address: " + (Address & 0xfffffffc).ToString("X8"));
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            if (Address < 0x08000000)
            {
                logFile.WriteLine("LowRam Write at " + Address.ToString("X8") + " -> " + Value.ToString("X8"));

                this.LowRam[(Address + 0) & 0x7ffffff] = (byte)((Value >> 0) & 0xFF);
                this.LowRam[(Address + 1) & 0x7ffffff] = (byte)((Value >> 8) & 0xFF);
                this.LowRam[(Address + 2) & 0x7ffffff] = (byte)((Value >> 16) & 0xFF);
                this.LowRam[(Address + 3) & 0x7ffffff] = (byte)((Value >> 24) & 0xFF);
            }
            else if (InRange(Address, 0x08000000, 0x10000000))
            {
                logFile.WriteLine("RAM Write at " + Address.ToString("X8") + " -> " + Value.ToString("X8"));

                this.RAM[(Address + 0) & 0x7ffffff] = (byte)((Value >> 0) & 0xFF);
                this.RAM[(Address + 1) & 0x7ffffff] = (byte)((Value >> 8) & 0xFF);
                this.RAM[(Address + 2) & 0x7ffffff] = (byte)((Value >> 16) & 0xFF);
                this.RAM[(Address + 3) & 0x7ffffff] = (byte)((Value >> 24) & 0xFF);
            }
            else if (InRange(Address, 0x22000000, 0x22500000))
            {
                logFile.WriteLine("SRAM Write at " + Address.ToString("X8") + " -> " + Value.ToString("X8"));

                this.SRAM[(Address + 0) - 0x22000000] = (byte)((Value >> 0) & 0xFF);
                this.SRAM[(Address + 1) - 0x22000000] = (byte)((Value >> 8) & 0xFF);
                this.SRAM[(Address + 2) - 0x22000000] = (byte)((Value >> 16) & 0xFF);
                this.SRAM[(Address + 3) - 0x22000000] = (byte)((Value >> 24) & 0xFF);
            }
            else if (InRange(Address, 0x24000000, 0x24100000))
            {
                logFile.WriteLine("NOR Write at " + Address.ToString("X8") + " -> " + Value.ToString("X8"));
            }
            else if (InRange(Address, 0x38000000, 0x40000000))
            {
                uint peripheralsAddr = Address - 0x38000000;

                if (peripheralsAddr < 0x1000)
                {
                    logFile.WriteLine("iPhone: SHA1 Write -> 0x" + Value.ToString("X"));

                    // sha1

                    SHA1.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x00100000, 0x00101000))
                {
                    logFile.WriteLine("iPhone: Clock0 Write -> 0x" + Value.ToString("X"));

                    // clock 0

                    Clock0.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x00200000, 0x00201000))
                {
                    logFile.WriteLine("iPhone: DMAC[0] Write -> 0x" + Value.ToString("X"));

                    // dmac 0
                }
                else if (InRange(peripheralsAddr, 0x00400000, 0x00403000))
                {
                    logFile.WriteLine("iPhone: USB OTG Write -> 0x" + Value.ToString("X"));

                    // usb otg
                }
                else if (InRange(peripheralsAddr, 0x00c00000, 0x00c01000))
                {
                    logFile.WriteLine("iPhone: AES Write -> 0x" + Value.ToString("X"));

                    // aes
                }
                else if (InRange(peripheralsAddr, 0x00e00000, 0x00e01000))
                {
                    logFile.WriteLine("iPhone: VIC[0] Write -> 0x" + Value.ToString("X"));

                    // vic 0

                    VIC[0].Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x00e01000, 0x00e02000))
                {
                    logFile.WriteLine("iPhone: VIC[1] Write -> 0x" + Value.ToString("X"));

                    // vic 1

                    VIC[1].Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x00e02000, 0x00e03000))
                {
                    logFile.WriteLine("iPhone: EdgeIC Write -> 0x" + Value.ToString("X"));

                    // edgeic
                }
                else if (InRange(peripheralsAddr, 0x01900000, 0x01901000)) 
                {
                    logFile.WriteLine("iPhone: DMAC[1] Write -> 0x" + Value.ToString("X"));

                    // dmac 1
                }
                else if (InRange(peripheralsAddr, 0x01a00000, 0x01a00080))
                {
                    logFile.WriteLine("iPhone: Power Write -> 0x" + Value.ToString("X"));

                    // power

                    Power.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x01a00080, 0x01a00100))
                {
                    logFile.WriteLine("iPhone: GPIOIC Write -> 0x" + Value.ToString("X"));

                    // gpioic
                }
                else if (InRange(peripheralsAddr, 0x04300000, 0x04300100))
                {
                    logFile.WriteLine("iPhone: SPI[0] Write -> 0x" + Value.ToString("X"));

                    // spi 0

                    SPI[0].Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x04400000, 0x04401000))
                {
                    logFile.WriteLine("iPhone: USB Phy Write -> 0x" + Value.ToString("X"));

                    // usb phy

                    USBPhy.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x04500000, 0x04501000))
                {
                    logFile.WriteLine("iPhone: Clock1 Write -> 0x" + Value.ToString("X"));

                    // clock 1

                    Clock1.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x04e00000, 0x04e00100))
                {
                    logFile.WriteLine("iPhone: SPI[1] Write -> 0x" + Value.ToString("X"));

                    // spi 1

                    SPI[1].Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x05200000, 0x05200100))
                {
                    logFile.WriteLine("iPhone: SPI[2] Write -> 0x" + Value.ToString("X"));

                    // spi 2

                    SPI[2].Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x06200000, 0x06220000))
                {
                    logFile.WriteLine("iPhone: Timers Write -> 0x" + Value.ToString("X"));

                    // timers

                    Timer.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x06300000, 0x06301000))
                {
                    logFile.WriteLine("iPhone: WDT Write -> 0x" + Value.ToString("X"));

                    // wdt

                    WDT.Write(Address, Value);
                }
                else if (InRange(peripheralsAddr, 0x06400000, 0x06401000))
                {
                    logFile.WriteLine("iPhone: GPIO Write -> 0x" + Value.ToString("X"));

                    // gpio

                    GPIO.Write(Address, Value);
                }
                else
                {
                    logFile.WriteLine("Unknown peripheral port address 0x" + peripheralsAddr.ToString("X"));
                }
            }
            else if (InRange(Address, 0x80000000, 0x88000000))
            {
                logFile.WriteLine("RAM Write at " + Address.ToString("X8") + " -> " + Value.ToString("X8"));

                this.RAM[(Address + 0) & 0x7ffffff] = (byte)((Value >> 0) & 0xFF);
                this.RAM[(Address + 1) & 0x7ffffff] = (byte)((Value >> 8) & 0xFF);
                this.RAM[(Address + 2) & 0x7ffffff] = (byte)((Value >> 16) & 0xFF);
                this.RAM[(Address + 3) & 0x7ffffff] = (byte)((Value >> 24) & 0xFF);
            }
            else
            {
                Console.WriteLine("Debug: unknown write from address: " + (Address & 0xfffffffc).ToString("X8") + " with a value of " + Value.ToString("X8"));
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
            WDT.wdtTick();
            Timer.timersTick();
            SPI[0].spiTick();
        }
    }
}
