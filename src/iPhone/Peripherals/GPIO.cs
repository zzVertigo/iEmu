using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class GPIO : IO32, IDisposable
    {
        public enum Registers
        {
            GPIO_INTLEVEL = 0x80,
            GPIO_INTSTAT = 0xA0,
            GPIO_INTEN = 0xC0,
            GPIO_INTTYPE = 0xE0,
            GPIO_FSEL = 0x1E0
        }

        public override uint ProcessRead(uint Address)
        {
            if (Address == 0x320)
            {
                Console.WriteLine("fsel read");
            }
            else if (Address < 0x300)
            {
                uint type = (Address >> 2) & 7;
                uint port = (Address >> 5) & 0x1f;

                Console.WriteLine("gpio read: " + type + " -> " + port);
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            if (Address == 0x320)
            {
                Console.WriteLine("fsel write");
            }
            else if (Address < 0x300)
            {
                uint type = (Address >> 2) & 7;
                uint port = (Address >> 5) & 0x1f;

                Console.WriteLine("gpio write: " + type + " -> " + port);
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
