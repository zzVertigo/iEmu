using Apollo.ARM11;
using System;

namespace Apollo.iPhone
{
    public class Memory : IBus
    {
        /* iBoot SDRAM map
            0x08000000-0x0B000000 - Load area (48 MB)
            0x0B000000-0x0DF00000 - Kernel (47 MB)
            0x0DF00000-0x0E000000 - Device tree (1 MB)
            0x0E000000-0x11000000 - Ramdisk (48 MB)
            0x11000000-0x17F00000 - Heap (111 MB)
            0x17F00000-0x17FFC000 - iBoot (unused) (1008 kB)
            0x17FFC000-0x18000000 - Panic (16kB)
        */

        const uint SDRAM_BASE = 0x08000000;
        const uint SDRAM_BANK_LEN = 0x10000000;
        const uint SDRAM_BANK_COUNT = 1;
        const uint SDRAM_END = (SDRAM_BASE + (SDRAM_BANK_LEN * SDRAM_BANK_COUNT));

        const uint VROM_BASE = 0x20000000;
        const uint VROM_BANK_LEN = 0x00100000;
        const uint VROM_LEN = 0x00020000;

        const uint SRAM_BASE = 0x22000000;
        const uint SRAM_BANK_LEN = 0x00400000;
        const uint SRAM_LEN = 0x00020000;

        const uint PANIC_SIZE = 0x00004000;
        const uint PANIC_BASE = (SDRAM_END - PANIC_SIZE);

        const uint IBOOT_SIZE = (0x00100000 - PANIC_SIZE);
        const uint IBASE_BASE = (PANIC_BASE - IBOOT_SIZE);
        const uint PROTECTED_REGION_SIZE = IBOOT_SIZE;

        const uint MMU_TT_SIZE = 0x1000;
        const uint MMU_TT_BASE = (SRAM_BASE + SRAM_LEN - MMU_TT_SIZE);

        const uint KB = 1024;
        const uint MB = 1024 * KB;

        public byte[] BootROM;
        public byte[] SDRAM;
        public byte[] SRAM;

        public Clock1 Clock1;
        public Timer Timer;
        public Power Power;
        public USBPhy USBPhy;
        public GPIO GPIO;
        public ChipID ChipID;
        public SPI SPI;
        public VIC VIC0;
        public VIC VIC1;

        public Memory(Emulator emulator)
        {
            BootROM = new byte[0x10000 + 1];
            SDRAM = new byte[0x23FFF + 1];
            SRAM = new byte[0x2FFFF + 1];

            Clock1 = new Clock1();
            Timer = new Timer(emulator);
            Power = new Power();
            USBPhy = new USBPhy();
            GPIO = new GPIO();
            ChipID = new ChipID();
            SPI = new SPI();
            VIC0 = new VIC();
            VIC1 = new VIC();
        }

        public byte ReadUInt8(uint Address)
        {
            if (InRange(Address, 0x0, 0xFFFF))
            {
                return BootROM[Address];
            }
            else if (InRange(Address, 0x22000000, 0x22023FFF))
            {
                uint tempAddr = Address - 0x22000000;

                return SDRAM[tempAddr];
            }
            else if (InRange(Address, 0x22024000, 0x2202FFFF))
            {
                uint tempAddr = Address - 0x22024000;

                return SRAM[tempAddr];
            }
            else if (InRange(Address, 0x3C500000, 0x3C501000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C500000;

                return Clock1.Read(tempAddr);
            }
            else if (InRange(Address, 0x3C700000, 0x3C701000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C700000;

                return Timer.Read(tempAddr);
            }
            else if (InRange(Address, 0x3D100000, 0x3D101000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3D100000;

                return ChipID.Read(tempAddr);
            }
            else if (InRange(Address, 0x3C300000, 0x3C301000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C300000;

                return SPI.Read(tempAddr);
            }
            else if (InRange(Address, 0x3CF00000, 0x3CF01000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3CF00000;

                GPIO.Read(tempAddr);
            }
            else if (Address >= 0x39700000 && Address < 0x39701000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x39700000;

                Power.Read(tempAddr);
            }
            else if (Address >= 0x38E00000 && Address < 0x38E01000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x38E00000;

                VIC0.Read(tempAddr);
            }
            else if (Address >= 0x38E01000 && Address < 0x38E02000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x38E01000;

                VIC1.Read(tempAddr);
            }
            //else if (Address >= 0x3C400000 && Address < 0x3C401000)
            //{
            //    uint tempAddr = (Address & 0xfffffffc) - 0x3C400000;

            //    USBPhy.Read(tempAddr);
            //}
            //else if (Address >= 0x38400000 && Address < 0x38401000)
            //{
            //    // usb otg
            //}
            //else if (Address >= 0x3CC00000 && Address < 0x3C01000)
            //{
            //    Console.WriteLine("UART Read");
            //}
            else
            {
                Console.WriteLine("Debug: unknown read from address: " + (Address & 0xfffffffc).ToString("X8"));
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            if (InRange(Address, 0x0, 0xFFFF))
            {
                BootROM[Address] = Value;
            }
            else if (InRange(Address, 0x22000000, 0x22023FFF))
            {
                uint tempAddr = Address - 0x22000000;

                SDRAM[tempAddr] = Value;
            }
            else if (InRange(Address, 0x22024000, 0x2202FFFF))
            {
                uint tempAddr = (Address) - 0x22024000;

                SRAM[tempAddr] = Value;
            }
            else if (InRange(Address, 0x3C500000, 0x3C501000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C500000;

                Clock1.Write(tempAddr, Value);
            }
            else if (InRange(Address, 0x3C700000, 0x3C701000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C700000;

                Timer.Write(tempAddr, Value);
            }
            else if (InRange(Address, 0x3D100000, 0x3D101000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3D100000;

                ChipID.Write(tempAddr, Value);
            }
            else if (InRange(Address, 0x3C300000, 0x3C301000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C300000;

                SPI.Write(tempAddr, Value);
            }
            else if (InRange(Address, 0x3CF00000, 0x3CF01000))
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3CF00000;

                GPIO.Write(tempAddr, Value);
            }
            else if (Address >= 0x39700000 && Address < 0x39701000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x39700000;

                Power.Write(tempAddr, Value);
            }
            else if (Address >= 0x38E00000 && Address < 0x38E01000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x38E00000;

                VIC0.Write(tempAddr, Value);
            }
            else if (Address >= 0x38E01000 && Address < 0x38E02000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x38E01000;

                VIC1.Write(tempAddr, Value);
            }
            //else if (Address >= 0x3C400000 && Address < 0x3C401000)
            //{
            //    uint tempAddr = (Address & 0xfffffffc) - 0x3C400000;

            //    USBPhy.Write(tempAddr, Value);
            //}
            //else if (Address >= 0x38400000 && Address < 0x38401000)
            //{
            //    // usb otg
            //}
            //else if (InRange(Address, 0x3CC00000, 0x3C01000))
            //{
            //    Console.WriteLine("UART Write");
            //}
            else
            {
                Console.WriteLine("Debug: unknown write from address: " + (Address & 0xfffffffc).ToString("X8") + " with a value of " + Value.ToString("X8"));
            }
        }

        public bool InRange(uint x, uint y, uint z)
        {
            if (x >= y && x <= z)
                return true;
            else
                return false;
        }
    }
}
