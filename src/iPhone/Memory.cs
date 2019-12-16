using Apollo.ARM11;
using System;

namespace Apollo.iPhone
{
    public class Memory : IBus
    {
        const uint KB = 1024;
        const uint MB = 1024 * KB;

        public byte[] BootROM;
        public byte[] SDRAM;

        public Clock1 Clock1;
        public Timer Timer;
        public Power Power;
        public USBPhy USBPhy;
        public GPIO GPIO;

        public Memory()
        {
            BootROM = new byte[0x100000];
            SDRAM = new byte[0x100000];

            Clock1 = new Clock1();
            Timer = new Timer();
            Power = new Power();
            USBPhy = new USBPhy();
            GPIO = new GPIO();
        }

        public byte ReadUInt8(uint Address)
        {
            //Console.WriteLine("Read: " + (Address & 0xfffffffc).ToString("X8"));

            if (Address <= 0xFFFF)
            {
                return BootROM[Address];
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                return SDRAM[Address - 0x22000000];
            }
            else if (Address >= 0x3C500000 && Address < 0x3C501000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C500000;

                return Clock1.Read(tempAddr);
            }
            //else if (Address >= 0x3C700000 && Address < 0x3C701000)
            //{
            //    uint tempAddr = (Address & 0xfffffffc) - 0x3C700000;

            //    return Timer.Read(tempAddr);
            //}
            //else if (Address >= 0x3D100000 && Address < 0x3D101000)
            //{
            //    // chipid
            //}
            else if (Address >= 0x3CF00000 && Address < 0x3CF01000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3CF00000;

                GPIO.Read(tempAddr);
            }
            else if (Address >= 0x39700000 && Address < 0x39701000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x39700000;

                Power.Read(tempAddr);
            }
            else if (Address >= 0x3C400000 && Address < 0x3C401000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C400000;

                USBPhy.Read(tempAddr);
            }
            //else if (Address >= 0x38400000 && Address < 0x38401000)
            //{
            //    // usb otg
            //}
            //else if (Address >= 0x38E00000 && Address < 0x38E01000)
            //{
            //    // vic0
            //}
            //else if (Address >= 0x38E01000 && Address < 0x38E02000)
            //{
            //    // vic1
            //}
            else
            {
                //Console.WriteLine("Debug: unknown read from address: " + Address.ToString("X8"));
            }

            return 0;
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            //Console.WriteLine("Write: " + (Address & 0xfffffffc).ToString("X8"));

            if (Address <= 0xFFFF)
            {
                BootROM[Address] = Value;
            }
            else if (Address >= 0x22000000 && Address < 0x22100000)
            {
                SDRAM[Address - 0x22000000] = Value;
            }
            else if (Address >= 0x3C500000 && Address < 0x3C501000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C500000;

                Clock1.Write(tempAddr, Value);
            }
            //else if (Address >= 0x3C700000 && Address < 0x3C701000)
            //{
            //    uint tempAddr = (Address & 0xfffffffc) - 0x3C700000;

            //    Timer.Write(tempAddr, Value);
            //}
            //else if (Address >= 0x3D100000 && Address < 0x3D101000)
            //{
            //    // chipid
            //}
            else if (Address >= 0x3CF00000 && Address < 0x3CF01000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3CF00000;

                GPIO.Write(tempAddr, Value);
            }
            else if (Address >= 0x39700000 && Address < 0x39701000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x39700000;

                Power.Write(tempAddr, Value);
            }
            else if (Address >= 0x3C400000 && Address < 0x3C401000)
            {
                uint tempAddr = (Address & 0xfffffffc) - 0x3C400000;

                USBPhy.Write(tempAddr, Value);
            }
            //else if (Address >= 0x38400000 && Address < 0x38401000)
            //{
            //    // usb otg
            //}
            //else if (Address >= 0x38E00000 && Address < 0x38E01000)
            //{
            //    // vic0
            //}
            //else if (Address >= 0x38E01000 && Address < 0x38E02000)
            //{
            //    // vic1
            //}
            else
            {
                //Console.WriteLine("Debug: unknown write from address: " + Address.ToString("X8") + " with a value of " + Value.ToString("X8"));
            }
        }
    }
}
