using System;
using System.Diagnostics;

namespace Apollo.iPhone
{
    public class SPI : IO32, IDisposable
    {
        public Emulator device { get; set; }
        public struct spi_t
        {
            public uint cmd, ctrl, setup, status, pin, tx_data, rx_data, clk_div, cnt, idd, interrupt_count;

            public byte interrupt;
        }

        public enum Registers
        {
            SPI_CONTROL = 0x0,
            SPI_SETUP = 0x4,
            SPI_STATUS = 0x8,
            SPI_SPIPIN = 0xC,
            SPI_TXDATA = 0x10,
            SPI_RXDATA = 0x20,
            SPI_CLKDIVIDER = 0x30,
            SPI_SPCNT = 0x34,
            SPI_SPIDD = 0x38,
            SPI_REG0 = 0x3C,
            SPI_REG1 = 0x40,
            SPI_REG2 = 0x44,
            SPI_REG3 = 0x48,
            SPI_TXLEN = 0x4C
        }

        public spi_t spi;

        public SPI(Emulator emulator)
        {
            this.device = emulator;

            spi = new spi_t();
        }

        public void spiTick()
        {
            if (spi.interrupt_count > 0)
            {
                spi.interrupt_count--;

                if (Convert.ToBoolean(spi.interrupt_count))
                {
                    //this.device.Interrupt(spi.interrupt, true);
                }
            }
        }

        public override uint ProcessRead(uint Address)
        {
            switch ((Registers)Address)
            {
                case Registers.SPI_CONTROL:
                    return spi.ctrl;

                case Registers.SPI_SETUP:
                    return spi.setup;

                case Registers.SPI_STATUS:
                    return spi.status;

                case Registers.SPI_SPIPIN:
                    return spi.pin;

                case Registers.SPI_TXDATA:
                    return spi.tx_data;

                case Registers.SPI_RXDATA:
                    {
                        switch (spi.cmd)
                        {
                            case 0x95:
                                return 0x01;

                            case 0xDA:
                                return 0x71;

                            case 0xDB:
                                return 0xC2;

                            case 0xDC:
                                return 0x00;
                        }

                        return 0;
                    }

                case Registers.SPI_CLKDIVIDER:
                    return spi.clk_div;

                case Registers.SPI_SPCNT:
                    return spi.cnt;

                case Registers.SPI_SPIDD:
                    return spi.idd;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            switch ((Registers)Address)
            {
                case Registers.SPI_CONTROL: {
                        if (Convert.ToBoolean(Value & 1))
                        {
                            spi.status |= 0xff2;
                            spi.cmd = spi.tx_data;
                            spi.interrupt_count = 12000;
                        }

                        spi.ctrl = Value;

                        break;
                    }

                case Registers.SPI_SETUP: {
                        spi.setup = Value;
                        break;
                    }

                case Registers.SPI_STATUS: {
                        spi.status = Value & 0x00000fff;

                        //this.device.Interrupt(spi.interrupt, false);
                        break;
                    }

                case Registers.SPI_SPIPIN: {
                        spi.pin = Value;
                        break;
                    }

                case Registers.SPI_TXDATA: {
                        spi.tx_data = Value;
                        break;
                    }

                case Registers.SPI_RXDATA: {
                        spi.rx_data = Value;
                        break;
                    }

                case Registers.SPI_CLKDIVIDER: {
                        spi.clk_div = Value;
                        break;
                    }

                case Registers.SPI_SPCNT: {
                        spi.cnt = Value;
                        break;
                    }

                case Registers.SPI_SPIDD: {
                        spi.idd = Value;
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
