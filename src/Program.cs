using Apollo.iPhone;
using System;
using System.Threading;

// 12/11/2019 - executes 17 instructions before crashing (or looping back up again)

namespace Apollo
{
    public class Program
    {
        private static Emulator Emulator;

        private static void Main(string[] args)
        {
            Emulator = new Emulator();

            Console.WriteLine("iPod 2G Emulator");

            // Bootrom mapped to 0x0
            Emulator.LoadFile("bootrom.bin", 0x0);;

            // LLB mapped to 0x22000000
            Emulator.LoadFile("llb.bin", 0x22000000);

            Emulator.RunAsync();

            //Thread.Sleep(-1);
        }
    }
}
