using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Apollo.iPhone
{
    public class SHA1 : IO32, IDisposable
    {
        private Emulator device { get; set; }

        public struct sha1_t
        {
            public uint config, in_addr, in_size, unk_stat;

            public byte[] hash_out;
        }

        public enum Registers
        {
            SHA1_CONFIG = 0x0,
            SHA1_RESET = 0x4,
            SHA1_UNKSTAT = 0x7C,
            SHA1_INADDR = 0x84,
            SHA1_INSIZE = 0x8C
        }

        sha1_t sha1;

        public SHA1(Emulator emulator)
        {
            this.device = emulator;

            initSHA1();
        }

        private void initSHA1()
        {
            sha1.config = 0;
            sha1.in_addr = 0;
            sha1.in_size = 0;
            sha1.unk_stat = 0;

            sha1.hash_out = new byte[5];

            for (int i = 0; i < 5; i++)
            {
                sha1.hash_out[i] = 0;
            }
        }

        private void hashSHA1()
        {
            uint actual_in_size = sha1.in_size + 0x20;

            byte[] mem = new byte[actual_in_size];

            for (int i = 0; i < (actual_in_size >> 2); i += 4)
            {
                Console.WriteLine("Read: 0x" + (sha1.in_addr + i).ToString("X"));

                //mem[i] = this.device.Memory.ReadUInt8((uint)(sha1.in_addr + i));
            }

            System.Security.Cryptography.SHA1 sha = new SHA1CryptoServiceProvider();

            int bytesWritten;

            bool result = sha.TryComputeHash(mem, sha1.hash_out, out bytesWritten);

            if (result)
                Console.WriteLine("SHA1: hashSHA1() successfully computed " + bytesWritten + " bytes!");
            else
                Console.WriteLine("SHA1: hashSHA1() failed!");
        }

        public override uint ProcessRead(uint Address)
        {
            if (Address >= 0x20 && Address < 0x34)
            {
                return sha1.hash_out[(Address - 0x20) >> 2];
            }

            switch ((Registers)Address)
            {
                case Registers.SHA1_CONFIG:
                    return sha1.config;

                case Registers.SHA1_INADDR:
                    return sha1.in_addr;

                case Registers.SHA1_INSIZE:
                    return sha1.in_size;
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            Console.WriteLine("SHA1 Write: 0x" + Address.ToString("X"));

            switch ((Registers)Address)
            {
                case Registers.SHA1_CONFIG:
                    {
                        if (Convert.ToBoolean(Value & 2) && Convert.ToBoolean(sha1.config & 8)) 
                        {
                            //if (sha1.in_addr == null || sha1.in_size == null)
                            //{
                            //    return;
                            //}

                            hashSHA1();

                            //this.device.Interrupt(0x28, true);
                            //this.device.Interrupt(0x28, false);
                        }

                        sha1.config = Value;

                        break;
                    }

                case Registers.SHA1_RESET:
                    {
                        initSHA1();

                        break;
                    }

                case Registers.SHA1_UNKSTAT:
                    {
                        sha1.unk_stat = Value;

                        break;
                    }

                case Registers.SHA1_INADDR:
                    {
                        sha1.in_addr = Value;

                        break;
                    }

                case Registers.SHA1_INSIZE:
                    {
                        sha1.in_size = Value;

                        break;
                    }
            }
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                // dispose components
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
