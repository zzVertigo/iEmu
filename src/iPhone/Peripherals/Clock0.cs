using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class Clock0 : IO32, IDisposable
    {
        public struct clock0_t
        {
            public uint config;
            public uint adj1, adj2;
        }

        public enum Registers
        {
            CLOCK_CONFIG = 0x0,
            CLOCK_ADJ1 = 0x8,
            CLOCK_ADJ2 = 0x404
        }

        clock0_t clock0;

        public Clock0()
        {
            clock0 = new clock0_t();
        }

        public override uint ProcessRead(uint Address)
        {
            switch ((Registers)Address)
            {
                case Registers.CLOCK_CONFIG:
                    return clock0.config;

                case Registers.CLOCK_ADJ1:
                    return clock0.adj1;

                case Registers.CLOCK_ADJ2:
                    return clock0.adj2;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            switch ((Registers)Address)
            {
                case Registers.CLOCK_CONFIG:
                    {
                        clock0.config = Value;
                        break;
                    }

                case Registers.CLOCK_ADJ1:
                    {
                        clock0.adj1 = Value;
                        break;
                    }

                case Registers.CLOCK_ADJ2:
                    {
                        clock0.adj2 = Value;
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
