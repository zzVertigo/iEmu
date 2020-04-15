using Apollo.ARM11;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.iPhone
{
    public class Emulator
    {
        public static ARMCore CPU;
        public static Memory Memory;

        private static bool IsExecuting = false;
        private static bool IsPaused = false;

        private static Thread emuRunThread;
        private static Thread keyGetThread;

        private static ConsoleKeyInfo key;

        public Emulator()
        {
            Memory = new Memory(this);
            CPU = new ARMCore(Memory, false);

            key = new ConsoleKeyInfo();
        }

        public void emuLoop()
        {
            while (IsExecuting)
            {
                while (IsPaused) ;

                CPU.Execute();
                Memory.Tick();
            }
        }

        public void getKey()
        {
            while (IsExecuting)
            {
                key = Console.ReadKey(true);

                switch ((ConsoleKey)key.Key)
                {
                    case ConsoleKey.P:
                        {
                            if (!IsPaused)
                                IsPaused = true;
                            break;
                        }
                    case ConsoleKey.C:
                        {
                            if (IsPaused)
                                IsPaused = false;

                            if (!IsExecuting)
                                IsExecuting = true;
                            break;
                        }
                    case ConsoleKey.S:
                        {
                            if (IsPaused)
                            {
                                CPU.Execute();
                                Memory.Tick();
                            }
                            break;
                        }
                    case ConsoleKey.D:
                        {
                            if (IsPaused)
                            {
                                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\sram.bin", Memory.SRam);
                            }
                            else
                                Console.WriteLine("Please pause the emulation!");

                            break;
                        }
                    case ConsoleKey.R:
                        {
                            if (IsPaused)
                            {
                                string RVals = null;

                                for (int i = 0; i < 15; i++)
                                {
                                    RVals += "\nR[" + i + "] = 0x" + CPU.Registers[i].ToString("X8");
                                }

                                Console.WriteLine(RVals);
                            }
                            else
                                Console.WriteLine("Please pause the emulation!");

                            break;
                        }
                    case ConsoleKey.K:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }
        }

        public void runEmulator()
        {
            Console.WriteLine("---------------- Commands ----------------");
            Console.WriteLine("C. Continue the emulation");
            Console.WriteLine("P. Pause the emulation");
            Console.WriteLine("S. Step the emulation (only while paused)");
            Console.WriteLine("D. Dump SRAM from iPhone (only while paused)");
            Console.WriteLine("R. Print all registers from iPhone (only while paused)");
            Console.WriteLine("K. Kill the emulation");
            Console.WriteLine("------------------------------------------\n");

            Console.WriteLine("Press S to start the emulation!\n");

            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                CPU.Reset();

                emuRunThread = new Thread(emuLoop);
                keyGetThread = new Thread(getKey);

                keyGetThread.Start();
                emuRunThread.Start();

                IsExecuting = true;
            }
        }
    }
}
