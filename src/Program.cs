using Apollo.iPod;
using System;

namespace Apollo
{
    public class Program
    {
        private static Emulator Emulator;

        private static void Main(string[] args)
        {
            Emulator = new Emulator();

            Console.WriteLine("iPod 2g Emulator");

            Emulator.LoadROM("bootrom.bin", 0x0);           

            do
            {
                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey();

                    if (keyPressed.Key == ConsoleKey.S)
                    {
                        Emulator.Step();
                    }
                }
            }
            while (true);
        }
    }
}
