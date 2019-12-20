using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class VIC : IO32, IDisposable
    {
        public unsafe struct vic_t
        {
            public uint irq_status, fiq_status;
            public uint raw_intr, int_select, int_enable, soft_int, protection, sw_priority_mask;

            public uint[] vect_addr, vect_priority;

            public uint address;

            public uint current_intr;
            public uint current_highest_intr;

            public int stack_i;

            public uint[] priority_stack;

            public byte[] irq_stack;

            public uint priority;

            public uint daisy_vect_addr, daisy_priority;

            public byte daisy_input;

            public byte[] id;
        }

        public enum Registers
        {
            VIC_IRQSTATUS = 0x0,
            VIC_FIQSTATUS = 0x4,
            VIC_RAWINT = 0x8,
            VIC_INTSELECT = 0xC,
            VIC_INTENABLE = 0x10,
            VIC_INTCLEAR = 0x14,
            VIC_SOFTINT = 0x18,
            VIC_SOFTINTCLEAR = 0x1C,
            VIC_REGPROTMODE = 0x20,
            VIC_SOFTPRIORITY = 0x24,
            VIC_DAISYPRIORITY = 0x28, // TODO: Double check
            VIC_VECTADDR = 0x100,
            VIC_PRIORITYLEVEL = 0x200
        }

        vic_t vic;

        public VIC()
        {
            vic = new vic_t();

            vic.vect_priority = new uint[32];
            vic.vect_addr = new uint[32];
            vic.priority_stack = new uint[33];
            vic.irq_stack = new byte[33];
            vic.id = new byte[8];

            for (int i = 0; i < 32; i++)
            {
                vic.vect_priority[i] = 0xF;
                vic.vect_addr[i] = 0;
            }

            for (int i = 0; i < 17; i++)
            {
                vic.priority_stack[i] = 0x10;
                vic.irq_stack[i] = 33;
            }

            vic.sw_priority_mask = 0xFFFF;
            vic.daisy_priority = 0xF;
            vic.current_intr = 33;
            vic.current_highest_intr = 33;
            vic.stack_i = 31;

            vic.priority = 0x10;

            vic.id[0] = 0x92;
            vic.id[1] = 0x11;
            vic.id[2] = 0x04;
            vic.id[3] = 0x00;
            vic.id[4] = 0x0D;
            vic.id[5] = 0xF0;
            vic.id[6] = 0x05;
            vic.id[7] = 0xB1;
        }

        public override uint ProcessRead(uint Address)
        {
            Console.WriteLine("VIC Read: " + Enum.GetName(typeof(Registers), Address));

            if (Convert.ToBoolean(Address & 3))
                return 0;

            if (Address >= 0xFE0 && Address < 0x1000)
                return vic.id[(Address >> 2) & 7];

            if (Address >= 0x100 && Address < 0x180)
                return vic.vect_addr[(Address >> 2) & 0x1F];

            if (Address >= 0x200 && Address < 0x280)
                return vic.vect_priority[(Address >> 2) & 0x1F];

            switch ((Registers)Address)
            {
                case Registers.VIC_IRQSTATUS:
                    return vic.irq_status;

                case Registers.VIC_FIQSTATUS:
                    return vic.fiq_status;

                case Registers.VIC_RAWINT:
                    return vic.raw_intr;

                case Registers.VIC_INTSELECT:
                    return vic.int_select;

                case Registers.VIC_INTENABLE:
                    return vic.int_enable;

                case Registers.VIC_INTCLEAR:
                    return vic.int_enable;

                case Registers.VIC_SOFTINT:
                    return vic.soft_int;

                case Registers.VIC_REGPROTMODE:
                    return vic.protection;

                case Registers.VIC_SOFTPRIORITY:
                    return vic.sw_priority_mask;

                case Registers.VIC_DAISYPRIORITY: // TODO: double check..
                    return vic.daisy_priority;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            Console.WriteLine("VIC Write: " + Enum.GetName(typeof(Registers), Address));

            if (Convert.ToBoolean(Address & 3))
                return;

            if (Address >= 0x100 && Address < 0x180)
                vic.vect_addr[(Address & 0x7F) >> 2] = Value;

            if (Address >= 0x200 && Address < 0x280)
                vic.vect_priority[(Address & 0x7F) >> 2] = Value;

            switch ((Registers)Address) 
            {
                case Registers.VIC_INTSELECT:
                    {
                        vic.int_select = Value;
                        break;
                    }

                case Registers.VIC_INTENABLE:
                    {
                        vic.int_enable |= Value;
                        break;
                    }

                case Registers.VIC_INTCLEAR:
                    {
                        vic.int_enable &= ~Value;
                        break;
                    }

                case Registers.VIC_SOFTINT:
                    {
                        vic.soft_int |= Value;
                        break;
                    }

                case Registers.VIC_SOFTINTCLEAR:
                    {
                        vic.soft_int &= ~Value;
                        break;
                    }

                case Registers.VIC_REGPROTMODE:
                    {
                        vic.protection = Value & 1;
                        break;
                    }

                case Registers.VIC_SOFTPRIORITY:
                    {
                        vic.sw_priority_mask = Value & 0xFFFF;
                        break;
                    }

                case Registers.VIC_DAISYPRIORITY: // TODO: double check..
                    {
                        vic.daisy_priority = Value & 0xF;
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
