using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class GPIOIC : IO32, IDisposable
    {
        public struct gpioic_t
        {
            public uint[] int_level, int_stat, int_enable, int_type;
        }

        gpioic_t gpioic;

        public GPIOIC()
        {
            gpioic = new gpioic_t();

            gpioic.int_level = new uint[7];
            gpioic.int_stat = new uint[7];
            gpioic.int_enable = new uint[7];
            gpioic.int_type = new uint[7];

            for (int i = 0; i < 7; i++)
            {
                gpioic.int_level[i] = 0;
                gpioic.int_stat[i] = 0xffffffff;
                gpioic.int_enable[i] = 0;
                gpioic.int_type[i] = 0xffffffff;
            }
        }

        public override uint ProcessRead(uint Address)
        {
            if ((Address & 0xFE0) == 0x80)
                return gpioic.int_level[(Address & 0x1C) >> 2];

            if ((Address & 0xFE0) == 0xA0)
                return gpioic.int_stat[(Address & 0x1C) >> 2];

            if ((Address & 0xFE0) == 0xC0)
                return gpioic.int_enable[(Address & 0x1C) >> 2];

            if ((Address & 0xFFF) == 0xE0)
                return gpioic.int_type[(Address & 0x1C) >> 2];

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            if ((Address & 0xFE0) == 0x80)
                gpioic.int_level[(Address & 0x1C) >> 2] = Value;

            if ((Address & 0xFE0) == 0xA0)
                gpioic.int_stat[(Address & 0x1C) >> 2] = Value;

            if ((Address & 0xFE0) == 0xC0)
                gpioic.int_enable[(Address & 0x1C) >> 2] = Value;

            if ((Address & 0xFFC) == 0xE0)
                gpioic.int_type[(Address & 0x1C) >> 2] = Value;
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
