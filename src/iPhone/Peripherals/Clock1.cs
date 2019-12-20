using System;

namespace Apollo.iPhone
{
    public class Clock1 : IO32, IDisposable
    {
        public struct pll
        {
            public uint con, cnt;
        }

        public struct clock1_t
        {
            public uint config0, config1, config2;

            public uint pll_lock;

            public pll[] plls;

            public uint pll_mode;

            public uint cl0_gates, cl1_gates, cl2_gates, cl3_gates;
        }

        public enum Registers
        {
            CLOCK_CONFIG0 = 0x0,
            CLOCK_CONFIG1 = 0x4,
            CLOCK_CONFIG2 = 0x8,
            CLOCK_PLL0CON = 0x20,
            CLOCK_PLL1CON = 0x24,
            CLOCK_PLL2CON = 0x28,
            CLOCK_PLL3CON = 0x2C,
            CLOCK_PLL0LCNT = 0x30,
            CLOCK_PLL1LCNT = 0x34,
            CLOCK_PLL2LCNT = 0x38,
            CLOCK_PLL3CNT = 0x3C,
            CLOCK_PLLLOCK = 0x40,
            CLOCK_PLLMODE = 0x44,
            CLOCK_GATES_0 = 0x48,
            CLOCK_GATES_1 = 0x4C,
            CLOCK_GATES_2 = 0x58,
            CLOCK_GATES_3 = 0x68,
            CLOCK_GATES_4 = 0x6C
        }

        clock1_t clock1;

        public Clock1()
        {
            clock1 = new clock1_t();

            clock1.plls = new pll[4];

            clock1.config0 = 0;
            clock1.pll_lock = 1;

            for (int i = 0; i < 3; i++)
            {
                clock1.plls[i].cnt = 0;
                clock1.plls[i].con = 0;
            }
        }

        public override uint ProcessRead(uint Address)
        {
            //Console.WriteLine("Clock1 Read: " + Enum.GetName(typeof(Registers), Address));

            switch((Registers)Address)
            {
                case Registers.CLOCK_CONFIG0:
                    return clock1.config0;

                case Registers.CLOCK_CONFIG1:
                    return clock1.config1;

                case Registers.CLOCK_CONFIG2:
                    return clock1.config2;

                case Registers.CLOCK_PLL0CON:
                    return clock1.plls[0].con;

                case Registers.CLOCK_PLL1CON:
                    return clock1.plls[1].con;

                case Registers.CLOCK_PLL2CON:
                    return clock1.plls[2].con;

                case Registers.CLOCK_PLL0LCNT:
                    return clock1.plls[0].cnt;

                case Registers.CLOCK_PLL1LCNT:
                    return clock1.plls[1].cnt;

                case Registers.CLOCK_PLL2LCNT:
                    return clock1.plls[2].cnt;

                case Registers.CLOCK_PLLLOCK:
                    return 1;

                case Registers.CLOCK_PLLMODE:
                    return clock1.pll_mode;

                case Registers.CLOCK_GATES_0:
                    return clock1.cl0_gates;

                case Registers.CLOCK_GATES_1:
                    return clock1.cl1_gates;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            //Console.WriteLine("Clock1 Write: " + Enum.GetName(typeof(Registers), Address));

            switch ((Registers)Address)
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

                case Registers.CLOCK_PLL0CON: {
                        clock1.plls[0].con = Value;
                        break;
                    }

                case Registers.CLOCK_PLL1CON: {
                        clock1.plls[1].con = Value;
                        break;
                    }

                case Registers.CLOCK_PLL2CON: {
                        clock1.plls[2].con = Value;
                        break;
                    }

                case Registers.CLOCK_PLL0LCNT: {
                        clock1.plls[0].cnt = Value;
                        break;
                    }

                case Registers.CLOCK_PLL1LCNT: {
                        clock1.plls[1].cnt = Value;
                        break;
                    }

                case Registers.CLOCK_PLL2LCNT: {
                        clock1.plls[2].cnt = Value;
                        break;
                    }

                case Registers.CLOCK_PLLLOCK: {
                        clock1.pll_lock = 1;
                       break;
                    }

                case Registers.CLOCK_PLLMODE: {
                        clock1.pll_mode = Value & 0xf01ff;
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
