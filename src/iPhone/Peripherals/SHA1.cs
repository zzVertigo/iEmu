using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apollo.iPhone
{
    public class SHA1 : IO32, IDisposable
    {
        public struct sha1_t
        {
            public uint config, in_addr, in_size, unk_stat;

            public uint[] hash_out;
        }

        public enum Registers
        {
            SHA1_CONFIG = 0x00,
            SHA1_RESET = 0x04,
            SHA1_UNK = 0x7C,
            SHA1_INADDR = 0x84,
            SHA1_INSIZE = 0x8C
        }

        sha1_t sha1;

        public SHA1()
        {
            sha1 = new sha1_t();

            sha1.config = 0;
            sha1.in_addr = 0;
            sha1.in_size = 0;
            sha1.unk_stat = 0;

            sha1.hash_out = new uint[5];

            for (int i = 0; i < 5; i++)
            {
                sha1.hash_out[i] = 0;
            }
        }

        public override uint ProcessRead(uint Address)
        {
            Console.WriteLine("SHA1 Read: " + (Address & 0xfff).ToString("X"));

            if ((Address & 0xfff) >= 0x020 && (Address & 0xfff) < 0x34)
            {
                Console.WriteLine("HASH OUTTTT");
            }

            switch (Address)
            {
                case 0x0:
                    {
                        return sha1.config;
                    }

                case 0x84:
                    {
                        return sha1.in_addr;
                    }

                case 0x8C:
                    {
                        return sha1.in_size;
                    }
            }

            return 0;
        }

        public override void ProcessWrite(uint Address, uint Value)
        {
            Console.WriteLine("SHA1 Write: " + Enum.GetName(typeof(Registers), Address));

            switch (Address)
            {
                case 0x0:
                    {
                        throw new Exception("SHA1 Interrupt");

                        Console.WriteLine("SHA1_CONFIG (Value & 2): " + Convert.ToBoolean(Value & 2));
                        Console.WriteLine("SHA1_CONFIG (sha1.config & 8): " + Convert.ToBoolean(sha1.config & 8));

                        //if (Convert.ToBoolean(Value & 2) && Convert.ToBoolean(sha1.config & 8))
                        //{
                        //    Debug.WriteLine("init sha1!!");
                        //}

                        sha1.config = Value;
                        break;
                    }

                case 0x4:
                    {
                        Debug.WriteLine("reset sha1!!");
                        break;
                    }

                case 0x7c:
                    {
                        sha1.unk_stat = Value;
                        break;
                    }

                case 0x84:
                    {
                        sha1.in_addr = Value;
                        break;
                    }

                case 0x8C:
                    {
                        sha1.in_size = Value;
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
