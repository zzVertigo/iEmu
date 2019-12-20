using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class ChipID : IO32, IDisposable
    {
        public uint chipid { get; set; }

        public enum Registers
        {
            CHIPID_UNUSED = 0x0,
            CHIPID_UNK0 = 0x4,
            CHIPID_INFO = 0x8
        }

        public ChipID()
        {
            for (byte i = 0; i < 14; i++)
            {
                this.chipid |= (uint)(0x01 << i);
            }

            for (byte i = 15; i < 30; i++)
            {
                this.chipid |= (uint)(0x8720 << i);
            }
        }

        public override uint ProcessRead(uint Address)
        {
            //Console.WriteLine("ChipID Read: " + Enum.GetName(typeof(Registers), Address));

            switch ((Registers)Address)
            {
                case Registers.CHIPID_INFO:
                    return this.chipid;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            //Console.WriteLine("ChipID Write: " + Enum.GetName(typeof(Registers), Address));

            switch ((Registers)Address)
            {
                case Registers.CHIPID_INFO: {
                        this.chipid = Value;
                        break;
                    }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                // dispose components
            }
        }
    }
}
