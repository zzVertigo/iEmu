using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class Power : IO32, IDisposable
    {
        public struct power_t
        {
            public uint config0, config1, config2, setstate, state, powered_on_devices;
        }

        public enum Registers
        {
            POWER_CONFIG0 = 0x0,
            POWER_CONFIG1 = 0x20,
            POWER_CONFIG2 = 0x6C,
            POWER_CONFIG3 = 0x74,
            POWER_ONCTRL = 0xC,
            POWER_OFFCTRL = 0x10,
            POWER_SETSTATE = 0x8,
            POWER_STATE = 0x14,
            POWER_ID = 0x44
        }

        power_t power;

        public Power()
        {
            power = new power_t();

            power.config0 = 0x01123009;
            power.config1 = 0x00000020;
            power.config2 = 0x00000000;

            power.powered_on_devices = 0x10EC;
        }

        public override uint ProcessRead(uint Address)
        {
            //Console.WriteLine("Power Read: " + Enum.GetName(typeof(Registers), Address));

            switch ((Registers)Address)
            {
                case Registers.POWER_CONFIG0:
                    return power.config0;

                case Registers.POWER_CONFIG1:
                    return power.config1;

                case Registers.POWER_CONFIG2:
                    return power.config2;

                case Registers.POWER_SETSTATE:
                    return power.setstate;

                case Registers.POWER_STATE:
                    return power.state;

                case Registers.POWER_ID:
                    return (0x3 << 24);
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            //Console.WriteLine("Power Write: " + Enum.GetName(typeof(Registers), Address));

            switch ((Registers)Address)
            {
                case Registers.POWER_CONFIG0: {
                        power.config0 = Value;
                        break;
                    }

                case Registers.POWER_CONFIG1: {
                        power.config1 = Value;
                        break;
                    }

                case Registers.POWER_CONFIG2: {
                        power.config2 = Value;
                        break;
                    }

                case Registers.POWER_ONCTRL: {
                        power.powered_on_devices |= Value;
                        power.setstate |= Value;
                        power.state |= Value;
                        break;
                    }

                case Registers.POWER_OFFCTRL: {
                        power.powered_on_devices &= ~Value;
                        power.setstate &= ~Value;
                        power.state &= ~Value;
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
