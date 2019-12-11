﻿using Apollo.ARM11;
using System;
using System.IO;
using System.Threading;

namespace Apollo.iPod
{
    public class Emulator
    {
        public ARMCore CPU;
        public Memory Memory;

        private bool IsExecuting;

        public Emulator()
        {
            Memory = new Memory();
            CPU = new ARMCore(Memory);
        }

        public void LoadFile(string FileName, uint Address)
        {
            byte[] Buffer = File.ReadAllBytes(FileName);

            for (uint i = 0; i < Buffer.Length; i++)
            {
                //Console.WriteLine("{0} -> {1}", (Address + i).ToString("X8"), Buffer[i]);

                Memory.WriteUInt8(Address + i, Buffer[i]);
            }

            CPU.Reset();

            CPU.Registers[0] = 0x48; // helps fix early crash at beginning of bootrom
            CPU.Registers[15] = 0x0;

            CPU.ReloadPipeline();
        }

        public void RunAsync()
        {
            Thread ExecutionThread = new Thread(Run);

            ExecutionThread.Start();
        }

        public void Step()
        {
            CPU.Execute();
        }

        private void Run()
        {
            IsExecuting = true;

            while (IsExecuting)
                CPU.Execute();
        }
    }
}
