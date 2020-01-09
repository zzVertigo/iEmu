using Apollo.iPhone;
using System;
using System.IO;
using System.Threading;

// 12/11/2019 - executes 17 instructions before crashing (or looping back up again)

namespace Apollo
{
    public class Program
    {
        private static Emulator Emulator;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);

            Emulator = new Emulator();

            Console.WriteLine("iPhone 2G Emulator");

            byte[] bootromFile = File.ReadAllBytes("bootrom_iphone2g.bin");
            byte[] llbFile = File.ReadAllBytes("llb.original.bin");

            // first we load the bootrom into it's own buffer
            Array.Copy(bootromFile, 0, Emulator.Memory.BootROM, 0, bootromFile.Length);

            // after we move the bootrom to low ram where execution begins
            Array.Copy(Emulator.Memory.BootROM, 0, Emulator.Memory.LowRam, 0, 0x10000);

            // finally we copy in the LLB into SRAM since eventually it will be put there anyways
            Array.Copy(llbFile, 0, Emulator.Memory.SRAM, 0, llbFile.Length);

            // setup the emulator and set PC to reset vector 0xfffffffc
            Emulator.Setup(0x0);

            // start the emulator
            Emulator.RunAsync();
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            File.WriteAllBytes("dumps/sram.bin", Emulator.Memory.SRAM);
            File.WriteAllBytes("dumps/ram.bin", Emulator.Memory.RAM);
            File.WriteAllBytes("dumps/lowram.bin", Emulator.Memory.LowRam);
        }
    }
}
