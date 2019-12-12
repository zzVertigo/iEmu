using Apollo.ARM11;
using System;
using System.IO;

namespace Apollo.iPod
{
    public class Memory : IBus
    {
        const uint KB = 1024;
        const uint MB = 1024 * KB;

        public byte[] BootROM;
        public byte[] SDRAM;

        public Clock1 Clock1;

        public Memory()
        {
            BootROM = new byte[0x100000];
            SDRAM = new byte[0x100000];

            Clock1 = new Clock1();
        }

        public byte ReadUInt8(uint Address)
        {
            //Console.WriteLine("Read from offset: " + (Address).ToString("X8") + "\n");

            if (Address < 0xFFFF)
            {
                return BootROM[Address];
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                return SDRAM[Address - 0x22000000];
            }
            else if (Address >= 0x38000000 && Address < 0x40000000)
            {
                uint pAddress = Address - 0x38000000;

                if (pAddress >= 0x04500000 && pAddress < 0x04501000)
                {
                    Clock1.Read(Address);
                }
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            //Console.WriteLine("Read from offset: " + (Address).ToString("X8") + "\n");

            if (Address < 0xFFFF)
            {
                BootROM[Address] = Value;
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                SDRAM[Address - 0x22000000] = Value;
            }
            else if (Address >= 0x38000000 && Address < 0x40000000)
            {
                uint pAddress = Address - 0x38000000;

                if (pAddress >= 0x04500000 && pAddress < 0x04501000)
                {
                    Clock1.Write(Address, Value);
                }
            }
        }
    }
}
