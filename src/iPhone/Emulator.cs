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
            CPU = new ARMCore(Memory);
        }

        public void LoadFile(string FileName, uint Address)
        {
            byte[] Buffer = File.ReadAllBytes(FileName);

            for (uint i = 0; i < Buffer.Length; i++)
            {
                Memory.WriteUInt8(Address + i, Buffer[i]);
            }

            CPU.Reset();

            CPU.Registers[0] = 0x48; // helps fix early crash at beginning of bootrom - not exactly sure why though..
            CPU.Registers[15] = 0x0; // start from the top!

            CPU.ReloadPipeline();
        }

        public void Tick()
        {
            Memory.Timer.timersTick();
        }

        public void RunAsync()
        {
            ControlThread = new Thread(Control);

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

                // Halt Emulation
                if (key.Key == ConsoleKey.H)
                {
                    IsExecuting = false;

                    File.WriteAllBytes(@"C:\Users\Vertigo\Documents\GitHub\iEmu\src\bin\Debug\netcoreapp3.0\dumps\sdram.bin", Memory.SDRAM);
                }
                else if (key.Key == ConsoleKey.C)
                {
                    IsExecuting = true;

                    while (IsExecuting)
                    {
                        CPU.Execute();
                        Tick(); // oop
                    }
                }
                else if (key.Key == ConsoleKey.S)
                {

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
