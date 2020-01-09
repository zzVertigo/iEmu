using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apollo.iPhone
{
    public class Timer : IO32, IDisposable
    {
        public Emulator device { get; set; }

        public struct timer_t
        {
            public uint config, state, compare_buffer, count_buffer, prescaler, observation, count;

            public void Tick()
            {
                if (Convert.ToBoolean(state & 1))
                {
                    count--;

                    if (count == 0)
                    {
                        if (Convert.ToBoolean(config & 0x7000))
                        {
                            throw new Exception("Timer Interrupt");
                        }
                    }
                }
            }

            public void Update()
            {
                count = (count_buffer * 100) + compare_buffer;
            }

            public uint TimerRead(uint Address)
            {
                switch ((Registers)Address)
                {
                    case Registers.TIMER_CONFIG:
                        return config;

                    case Registers.TIMER_CONTROL:
                        return state;

                    case Registers.TIMER_COMPARE_BUFFER:
                        return compare_buffer;

                    case Registers.TIMER_COUNT_BUFFER:
                        return count_buffer;

                    case Registers.TIMER_PRESCALER:
                        return prescaler;

                    case Registers.TIMER_OBSERVATION:
                        return observation;
                }

                return 0;
            }

            public void TimerWrite(uint Address, uint Value)
            {
                switch ((Registers)Address)
                {
                    case Registers.TIMER_CONFIG:
                        {
                            config = Value;
                            break;
                        }

                    case Registers.TIMER_CONTROL:
                        {
                            state = Value & 1;

                            if (Convert.ToBoolean(Value & 2))
                            {
                                Update();
                            }

                            break;
                        }

                    case Registers.TIMER_COMPARE_BUFFER:
                        {
                            compare_buffer = Value;
                            break;
                        }

                    case Registers.TIMER_COUNT_BUFFER:
                        {
                            count_buffer = Value;
                            break;
                        }

                    case Registers.TIMER_PRESCALER:
                        {
                            prescaler = Value;
                            break;
                        }

                    case Registers.TIMER_OBSERVATION:
                        {
                            observation = Value;
                            break;
                        }
                }
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
            TIMER_CONTROL = 0x4,
            TIMER_COMPARE_BUFFER = 0x8,
            TIMER_COUNT_BUFFER = 0xC,
            TIMER_PRESCALER = 0x10,
            TIMER_OBSERVATION = 0x14,

            TIMER_TICKSHIGH = 0x80,
            TIMER_TICKSLOW = 0x84,
            TIMER_REG0 = 0x88,
            TIMER_REG1 = 0x8C,
            TIMER_REG2 = 0x90,
            TIMER_REG3 = 0x94,
            TIMER_REG4 = 0x98,

            TIMER_INTERRUPT = 0xF8,
            TIMER_IRQSTAT = 0x10000
        }

        timers_t timers;

        public Timer(Emulator Device)
        {
            this.device = Device;

            timers = new timers_t();

            timers.unk_reg_0 = 0xA;
            timers.unk_reg_1 = 0xFFFFFFFF;
            timers.unk_reg_2 = 0xFFFFFFFF;
            timers.unk_reg_3 = 0xFFFFFFFF;

            timers.timers = new timer_t[7];

            for (int i = 0; i < 7; i++)
            {
                timers.timers[i].count = 0;
                timers.timers[i].state = 0;
            }
        }

        public void timersTick()
        {
            for (int i = 0; i < 7; i++)
            {
                if (Convert.ToBoolean(timers.timers[i].state & 1))
                {
                    timers.timers[i].Tick();
                }

                timers.ticks_low++;

                if (timers.ticks_low == 0)
                    timers.ticks_high++;
            }
        }

        public override uint ProcessRead(uint Address)
        {
            if ((Address) <= 0x60)
            {
                uint idx = (Address & 0x60) >> 5;

                Console.WriteLine("Index: " + idx);

                return timers.timers[idx].TimerRead(Address);
            }
            else if ((Address) >= 0xA0 && (Address) <= 0xF8)
            {
                uint idx = ((Address & 0x60) >> 5) - 1;

                Console.WriteLine("Index: " + idx);

                return timers.timers[idx].TimerRead(Address);
            }
            else switch ((Registers)Address)
            {
                case Registers.TIMER_TICKSHIGH:
                    return timers.ticks_high;

                case Registers.TIMER_TICKSLOW:
                    return timers.ticks_low;

                case Registers.TIMER_REG0:
                    return timers.unk_reg_0;

                case Registers.TIMER_REG1:
                    return timers.unk_reg_1;

                case Registers.TIMER_REG2:
                    return timers.unk_reg_2;

                case Registers.TIMER_REG3:
                    return timers.unk_reg_3;

                case Registers.TIMER_REG4:
                    return timers.unk_reg_4;

                case Registers.TIMER_IRQSTAT:
                    return timers.irq_stat;

                case Registers.TIMER_INTERRUPT:
                    return 0xFFFFFFFF;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            if ((Address) <= 0x60)
            {
                uint idx = (Address & 0x60) >> 5;

                Console.WriteLine("Index: " + idx);

                timers.timers[idx].TimerWrite(Address, Value);
            }
            else if ((Address) >= 0xA0 && (Address) <= 0xF8)
            {
                uint idx = ((Address & 0x60) >> 5) - 1;

                Console.WriteLine("Index: " + idx);

                timers.timers[idx].TimerWrite(Address, Value);
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

                    case Registers.TIMER_REG0: {
                            timers.unk_reg_0 = Value;
                            break;
                        }

                    case Registers.TIMER_REG1: {
                            timers.unk_reg_1 = Value;
                            break;
                        }

                    case Registers.TIMER_REG2: {
                            timers.unk_reg_2 = Value;
                            break;
                        }

                    case Registers.TIMER_REG3: {
                            timers.unk_reg_3 = Value;
                            break;
                        }

                    case Registers.TIMER_REG4: {
                            timers.unk_reg_4 = Value;
                            break;
                        }

                    case Registers.TIMER_IRQSTAT: {
                            timers.irq_stat = Value;
                            break;
                        }

                    case Registers.TIMER_INTERRUPT: {
                            device.Interrupt(0x7, false);
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
