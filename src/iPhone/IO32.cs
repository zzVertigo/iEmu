using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.iPhone
{
    public abstract class IO32
    {
        private uint ReadValue;
        private uint WriteValue;

        /// <summary>
        ///     Reads Data from the IO region.
        /// </summary>
        /// <param name="Address">The Address that is being read</param>
        /// <returns>The Data on the Address</returns>
        public byte Read(uint Address)
        {
            ReadValue = ProcessRead(Address);

            return (byte)ReadValue;
        }

        /// <summary>
        ///     Writes Data to the IO region.
        /// </summary>
        /// <param name="Address">The Address where the Value is being written</param>
        /// <param name="Value">The Value that is being written</param>
        public void Write(uint Address, byte Value)
        {
            ProcessWrite(Address, Value);

            WriteValue = Value;
        }

        /// <summary>
        ///     This method is called whenever a new Read request has arrived.
        /// </summary>
        /// <param name="Address">The Address where the Read operation was made</param>
        /// <returns>The Data on the Address</returns>
        public abstract uint ProcessRead(uint Address);

        /// <summary>
        ///     This method is called whenever a new Write operation is made.
        /// </summary>
        /// <param name="Address">The Address where the Write operation was made</param>
        /// <param name="Value">The value being written</param>
        public abstract void ProcessWrite(uint Address, uint Value);
    }
}