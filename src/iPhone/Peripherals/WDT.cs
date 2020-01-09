using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apollo.iPhone
{
    public class WDT : IO32, IDisposable
    {
        public struct wdt_t
        {
            public uint ctrl, count, cnt_period;
        }

        public enum Registers
        {
            WDT_CONTROL = 0x0,
            WDT_COUNT = 0x4
        }

        wdt_t wdt;
        public WDT()
        {
            wdt = new wdt_t();

            wdt.ctrl = 0x1FCA00;
            wdt.cnt_period = ((wdt.ctrl >> 16) & 0xF) * (2048 / 4);
            wdt.cnt_period = wdt.cnt_period;
        }

        public void wdtTick()
        {
            if (Convert.ToBoolean(wdt.ctrl & 0x100000) && (wdt.ctrl & 0xFF) != 0xA5)
            {
                //Console.WriteLine("WDT Count: " + wdt.count);

                wdt.count--;
            }

            if (Convert.ToBoolean(wdt.ctrl & 0x8000))
            {
                if (wdt.count == 0)
                {
                    wdt.count = wdt.cnt_period;

                    throw new Exception("WDT Interrupt");
                }
            }
        }

        public override uint ProcessRead(uint Address)
        {
            switch ((Registers)Address)
            {
                case Registers.WDT_CONTROL:
                    return wdt.ctrl;

                case Registers.WDT_COUNT:
                    return wdt.count;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            switch ((Registers)Address)
            {
                case Registers.WDT_CONTROL:
                    {
                        wdt.ctrl = Value;
                        wdt.cnt_period = ((wdt.ctrl >> 16) & 0xF) * (2048 / 4);

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
