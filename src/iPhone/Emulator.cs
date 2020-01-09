using Apollo.ARM11;
using System;
using System.IO;
using System.Threading;

namespace Apollo.iPhone
{
    public class Emulator
    {
        public ARMCore CPU;
        public Memory Memory;

        private bool IsExecuting;

        private Thread ExecutionThread;
        private Thread ControlThread;

        public Emulator()
        {
            Memory = new Memory(this);
            CPU = new ARMCore(Memory, false);
        }

        //public void LoadFile(string Filename, byte[] Area)
        //{
        //    byte[] Buffer = File.ReadAllBytes(Filename);

        //    for (uint i = 0; i < Buffer.Length; i++)
        //    {
        //        Area[i] = Buffer[i];
        //    }
        //}

        public void Setup(uint Address)
        {
            CPU.Reset();

            CPU.Registers[15] = Address;

            CPU.ReloadPipeline();
        }

        public void RunAsync()
        {
            ControlThread = new Thread(Control);

            ControlThread.Name = "Thread #0";

            ControlThread.Start();
        }

        public void Step()
        {
            CPU.Execute();
        }

        public void Interrupt(int num, bool level)
        {
            num &= 0x3f;

            Console.WriteLine("Interrupt {0} {1}", num.ToString("X8"), level ? "triggered" : "lowered");

            if (num < 0x20)
            {
                if (level)
                {
                    // trigger interrupt on vic
                } 
                else
                {
                    // lower interrupt on vic
                }
            }
            else
            {
                if (level)
                {
                    // trigger interrupt on vic
                }
                else
                {
                    // lower interrupt on vic
                }
            }
        }

        private void Control()
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.C)
                {
                    IsExecuting = true;

                    while (IsExecuting)
                    {
                        CPU.Execute();
                        Memory.Tick(); // ticks timers
                    }
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            //if (Console.KeyAvailable)
            //{
            //    var keyPressed = Console.ReadKey();

            //    // Halt Emulation
            //    if (keyPressed.Key == ConsoleKey.H)
            //    {
            //        IsExecuting = false;

            //        File.WriteAllBytes(@"C:\Users\Vertigo\Documents\GitHub\iEmu\src\bin\Debug\netcoreapp3.0\dumps\sdram.bin", Memory.SDRAM);
            //    }
            //    // Start/Continue emulation
            //    else if (keyPressed.Key == ConsoleKey.C)
            //    {
            //        //IsExecuting = true;

            //        //do
            //        //{
            //        //    CPU.Execute();
            //        //}
            //        //while (IsExecuting);
            //    }
            //    // Step by step (only if IsExecuting is false)
            //    else if (keyPressed.Key == ConsoleKey.S && IsExecuting == false)
            //    {
            //        CPU.Execute();
            //    }
            //}
        }
    }
}
