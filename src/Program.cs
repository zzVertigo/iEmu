using Apollo.iPhone;
using System;
using System.IO;
using System.Threading;

namespace Apollo
{
    public class Program
    {
        private static Emulator Emulator;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);

            Console.WriteLine("iPhone 2G Emulator - Written by zzVertigo\n");

            Emulator = new Emulator();

            // begin by reading the necessary files to start emulation
            byte[] bootromFile = File.ReadAllBytes("bootrom_iphone2g.bin");

            // after we move the bootrom to low ram where execution begins
            Array.Copy(bootromFile, 0, Emulator.Memory.LowRam, 0, bootromFile.Length);
            Array.Copy(bootromFile, 0, Emulator.Memory.BootRom, 0, bootromFile.Length);

            Emulator.runEmulator();

            Thread.Sleep(-1);
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            //File.WriteAllBytes("dumps/sram.bin", Emulator.Memory.SRAM);
            //File.WriteAllBytes("dumps/ram.bin", Emulator.Memory.RAM);
            //File.WriteAllBytes("dumps/lowram.bin", Emulator.Memory.LowRam);
        }
    }
}
