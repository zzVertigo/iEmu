using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPod
{
    public class Clock1 : IO32, IDisposable
    {
        public struct clock1_t
        {
            public uint config0, config1, config2;

            public uint pll_lock;

            public uint cl0_gates, cl1_gates;
        }

        public enum Registers
        {
            CLOCK_CONFIG0 = 0x0,
            CLOCK_CONFIG1 = 0x4,
            CLOCK_CONFIG2 = 0x8,
            CLOCK_PLL0CON = 0x20,
            CLOCK_PLL1CON = 0x24,
            CLOCK_PLL2CON = 0x28,
            CLOCK_PLL0LCNT = 0x30,
            CLOCK_PLL1LCNT = 0x34,
            CLOCK_PLL2LCNT = 0x38,
            CLOCK_PLLLOCK = 0x40,
            CLOCK_PLLMODE = 0x44,
            CLOCK_GATES_0 = 0x48,
            CLOCK_GATES_1 = 0x4C,
            CLOCK_GATES_2 = 0x58,
            CLOCK_GATES_3 = 0x68,
            CLOCK_GATES_4 = 0x6C,
        }

        clock1_t clock1;

        public Clock1()
        {
            clock1 = new clock1_t();

            clock1.config0 = 0;
            clock1.pll_lock = 1;
        }

        public override uint ProcessRead(uint Address)
        {
            uint pAddress = Address - 0x3C500000;

            switch((Registers)pAddress)
            {
                case Registers.CLOCK_CONFIG0:
                    return clock1.config0;

                case Registers.CLOCK_CONFIG1:
                    return clock1.config1;

                case Registers.CLOCK_CONFIG2:
                    return clock1.config2;

                case Registers.CLOCK_PLLLOCK:
                    return clock1.pll_lock;

                case Registers.CLOCK_GATES_0:
                    return clock1.cl0_gates;

                case Registers.CLOCK_GATES_1:
                    return clock1.cl1_gates;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            uint pAddress = Address - 0x3C500000;

            switch ((Registers)pAddress)
            {
                case Registers.CLOCK_CONFIG0: {
                        clock1.config0 = Value;
                        break;
                    }

                case Registers.CLOCK_CONFIG1: {
                        clock1.config1 = Value;
                        break;
                    }

                case Registers.CLOCK_CONFIG2: {
                        clock1.config2 = Value;
                        break;
                    }

                case Registers.CLOCK_PLLLOCK: {
                        clock1.pll_lock = Value;
                        break;
                    }

                case Registers.CLOCK_GATES_0: {
                        clock1.cl0_gates = Value;
                        break;
                    }

                case Registers.CLOCK_GATES_1: {
                        clock1.cl1_gates = Value;
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
