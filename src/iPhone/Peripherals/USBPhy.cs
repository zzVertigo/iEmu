using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class USBPhy : IO32, IDisposable
    {
        public struct usbphy_t
        {
            public uint ophypwr, ophyclk, orstcon, ophytune;
        }

        public enum Registers
        {
            USB_OPHYPWR = 0x0,
            USB_OPHYCLK = 0x4,
            USB_ORSTCON = 0x8,
            USB_OPHYTUNE = 0x20,
            USB_OPHYUNK0 = 0x28,
            USB_OPHYCHRGER = 0x48
        }

        usbphy_t usbphy;

        public USBPhy()
        {
            usbphy = new usbphy_t();
        }

        public override uint ProcessRead(uint Address)
        {
            switch ((Registers)Address)
            {
                case Registers.USB_OPHYPWR:
                    return usbphy.ophypwr;

                case Registers.USB_OPHYCLK:
                    return usbphy.ophyclk;

                case Registers.USB_ORSTCON:
                    return usbphy.orstcon;

                case Registers.USB_OPHYTUNE:
                    return usbphy.ophytune;

                case Registers.USB_OPHYUNK0:
                    return 0x00000000;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            switch ((Registers)Address)
            {
                case Registers.USB_OPHYPWR: {
                        usbphy.ophypwr = Value;
                        break;
                    }

                case Registers.USB_OPHYCLK: {
                        usbphy.ophyclk = Value;
                        break;
                    }

                case Registers.USB_ORSTCON: {
                        usbphy.orstcon = Value;
                        break;
                    }

                case Registers.USB_OPHYTUNE: {
                        usbphy.ophytune = Value;
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
