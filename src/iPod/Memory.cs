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

        public Memory()
        {
            BootROM = new byte[0x100000];
            SDRAM = new byte[0x100000];
        }

        public byte ReadUInt8(uint Address)
        {
            Console.WriteLine("Read from offset: " + (Address).ToString("X8") + "\n");

            if (Address < 0xFFFF)
            {
                return BootROM[Address];
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                return SDRAM[Address - 0x22000000];
            }
            else
            {
                //Console.WriteLine("CPU write out of range at " + Address.ToString("X8"));
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            Console.WriteLine("Read from offset: " + (Address).ToString("X8") + "\n");

            if (Address < 0xFFFF)
            {
                BootROM[Address] = Value;
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                SDRAM[Address - 0x22000000] = Value;
            }
            else
            {
                //Console.WriteLine("CPU write out of range at " + Address.ToString("X8"));
            }
        }
    }
}
