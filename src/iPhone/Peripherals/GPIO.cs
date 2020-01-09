using System;
namespace Apollo.iPhone
{
    public class GPIO : IO32, IDisposable
    {
        public struct regs
        {
            public uint con, dat, pud1, pud2, conslp1, conslp2, pudslp1, pudslp2;
        }

        public struct fesl
        {
            public uint umask, reserved1, minor_port, reserved2, major_port, reserverd3;

            public uint whole;
        }

        public struct gpio_t
        {
            public regs[] regs;

            public fesl fesl;
        }

        public enum Registers
        {
            GPIO_INTLEVEL = 0x80,
            GPIO_INTSTAT = 0xA0,
            GPIO_INTEN = 0xC0,
            GPIO_INTTYPE = 0xE0,
            GPIO_FSEL = 0x1E0
        }

        gpio_t gpio;

        public GPIO()
        {
            gpio = new gpio_t();

            gpio.regs = new regs[32];

            gpio.fesl.whole = 0;
            gpio.regs[0x16].dat = 0x000000a0;
        }

        public override uint ProcessRead(uint Address)
        {
            if ((Address) == 0x320)
            {
                return gpio.fesl.whole;
            }
            else if ((Address) < 0x300) // any higher would just override the fesl??
            {
                uint type = (Address >> 2) & 7;
                uint port = (Address >> 5) & 0x1f;

                switch (type)
                {
                    case 0:
                        return gpio.regs[port].con;

                    case 1:
                        return gpio.regs[port].dat;

                    case 2:
                        return gpio.regs[port].pud1;

                    case 3:
                        return gpio.regs[port].pud2;

                    case 4:
                        return gpio.regs[port].conslp1;

                    case 5:
                        return gpio.regs[port].conslp2;

                    case 6:
                        return gpio.regs[port].pudslp1;

                    case 7:
                        return gpio.regs[port].pudslp2;
                }
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            if ((Address) == 0x320)
            {
                gpio.fesl.whole = Value & 0x001f070f;

                if ((gpio.fesl.umask & 0xE) == 0xE)
                {
                    uint port = decodeFESL();

                    gpio.regs[gpio.fesl.major_port].dat |= (uint)((gpio.fesl.umask & 1) << (byte)gpio.fesl.minor_port);
                }
            }
            else if (Address < 0x300)
            {
                uint type = (Address >> 2) & 7;
                uint port = (Address >> 5) & 0x1f;

                switch (type)
                {
                    case 0:
                        gpio.regs[port].con = Value;
                        break;

                    case 1:
                        gpio.regs[port].dat = Value;
                        break;

                    case 2:
                        gpio.regs[port].pud1 = Value;
                        break;

                    case 3:
                        gpio.regs[port].pud2 = Value;
                        break;

                    case 4:
                        gpio.regs[port].conslp1 = Value;
                        break;

                    case 5:
                        gpio.regs[port].conslp2 = Value;
                        break;

                    case 6:
                        gpio.regs[port].pudslp1 = Value;
                        break;

                    case 7:
                        gpio.regs[port].pudslp2 = Value;
                        break;
                }
            }
        }

        private uint decodeFESL()
        {
            uint portLo = gpio.fesl.minor_port;
            uint portHi = gpio.fesl.major_port << 8;

            uint port = portHi | portLo;

            return port;
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
