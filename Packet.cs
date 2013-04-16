using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACKSim
{
    class Packet
    {
        public UInt16 seq;
        public Boolean ack;
        public UInt16 dest;
        public UInt16 src;
        public uint timeStamp;
    }
}
