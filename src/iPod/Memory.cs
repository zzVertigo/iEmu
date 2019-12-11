using Apollo.ARM11;
using System;

namespace Apollo.iPod
{
    public class Memory : IBus
    {
        const uint KB = 1024;
        const uint MB = 1024 * KB;

        public byte[] BootROM;
        public byte[] SRAM;

        public Memory()
        {
            BootROM = new byte[10000];
            //SRAM = new byte[24000];
        }

        public byte ReadUInt8(uint Address)
        {
            Address &= 0x3fffffff;

            if (Address < BootROM.Length)
            {
                return BootROM[Address];
            }
            //else if (Address < SRAM.Length)
            //{
            //    return SRAM[Address];
            //}
            else
            {
                //Console.WriteLine("CPU write out of range at " + Address.ToString("X8"));
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            Address &= 0x3fffffff;

            if (Address < BootROM.Length)
            {
                BootROM[Address] = Value;
            }
            //else if (Address >= 0x22000000 && Address < 0x22024000)
            //{
            //    SRAM[Address] = Value;
            //}
            else
            {
                //Console.WriteLine("CPU write out of range at " + Address.ToString("X8"));
            }
        }
    }
}
