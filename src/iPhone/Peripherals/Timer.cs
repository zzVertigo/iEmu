using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public class Timer : IO32, IDisposable
    {
        public struct timer_t
        {
            public uint config, state, count_buffer, count_buffer_2, prescaler, unknown_3, count;

            public static uint TimerRead(uint Address)
            {
                return 0;
            }

            public static void TimerWrite(uint Address, uint Value)
            {

            }
        }

        public struct timers_t
        {
            public timer_t[] timers;

            public uint ticks_high, ticks_low, unk_reg_0, unk_reg_1, unk_reg_2, unk_reg_3, unk_reg_4, irq_stat;
        }

        public enum Registers
        {
            TIMER_CONFIG = 0x0,
            TIMER_STATE = 0x4,
            TIMER_COUNT_BUFFER = 0x8,
            TIMER_COUNT_BUFFER2 = 0xC,
            TIMER_PRESCALER = 0x10,
            TIMER_UNKNOWN3 = 0x14,
            TIMER_TICKSHIGH = 0x80,
            TIMER_TICKSLOW = 0x84,
            TIMER_UNKREG0 = 0x88,
            TIMER_UNKREG1 = 0x8C,
            TIMER_UNKREG2 = 0x90,
            TIMER_UNKREG3 = 0x94,
            TIMER_UNKREG4 = 0x98,
            TIMER_INTERRUPT = 0xF8,
            TIMER_IRQSTAT = 0x10000,
            TIMER_IRQLATCH = 0x118
        }

        timers_t timers;

        public Timer()
        {
            timers = new timers_t();

            timers.timers = new timer_t[7];

            for (int i = 0; i < 6; i++)
            {
                timers.timers[i].count = 0;
                timers.timers[i].state = 0;
            }
        }

        public override uint ProcessRead(uint Address)
        {
            //Console.WriteLine("Timer Read: " + Enum.GetName(typeof(Registers), Address));

            if ((Address) <= 0x60)
            {
                uint idx = (Address & 0x60) >> 5;

                Console.WriteLine("Index: " + idx);
            }
            else if ((Address) >= 0xA0 && (Address) <= 0xF8)
            {
                uint idx = ((Address & 0x60) >> 5) - 1;

                Console.WriteLine("Index: " + idx);
            }
            else switch ((Registers)Address)
            {
                case Registers.TIMER_TICKSHIGH:
                    return timers.ticks_high;

                case Registers.TIMER_TICKSLOW:
                    return timers.ticks_low;

                case Registers.TIMER_UNKREG0:
                    return timers.unk_reg_0;

                case Registers.TIMER_UNKREG1:
                    return timers.unk_reg_1;

                case Registers.TIMER_UNKREG2:
                    return timers.unk_reg_2;

                case Registers.TIMER_UNKREG3:
                    return timers.unk_reg_3;

                case Registers.TIMER_UNKREG4:
                    return timers.unk_reg_4;

                case Registers.TIMER_IRQSTAT:
                    return timers.irq_stat;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            //Console.WriteLine("Timer Write: " + Enum.GetName(typeof(Registers), Address));

            if ((Address) <= 0x60)
            {
                uint idx = (Address & 0x60) >> 5;

                Console.WriteLine("Index: " + idx);
            }
            else if ((Address) >= 0xA0 && (Address) <= 0xF8)
            {
                uint idx = ((Address & 0x60) >> 5) - 1;

                Console.WriteLine("Index: " + idx);
            }
            else switch ((Registers)Address)
            {
                    case Registers.TIMER_TICKSHIGH: {
                            timers.ticks_high = Value;
                            break;
                        }

                    case Registers.TIMER_TICKSLOW: {
                            timers.ticks_low = Value;
                            break;
                        }

                    case Registers.TIMER_UNKREG0: {
                            timers.unk_reg_0 = Value;
                            break;
                        }

                    case Registers.TIMER_UNKREG1: {
                            timers.unk_reg_1 = Value;
                            break;
                        }

                    case Registers.TIMER_UNKREG2: {
                            timers.unk_reg_2 = Value;
                            break;
                        }

                    case Registers.TIMER_UNKREG3: {
                            timers.unk_reg_3 = Value;
                            break;
                        }

                    case Registers.TIMER_UNKREG4: {
                            timers.unk_reg_4 = Value;
                            break;
                        }

                    case Registers.TIMER_IRQSTAT: {
                            timers.irq_stat = Value;
                            break;
                        }

                   // case Registers.TIMER_INTERRUPT:
                    //    {
                    //        Console.WriteLine("Timer Interrupt");
                    //        break;
                    //    }
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
