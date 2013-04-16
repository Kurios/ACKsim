using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACKSim
{
    class Socket
    {
        private Network network;

        public UInt16 id;
        public UInt16 dst;

        UInt16 lastRecv = 0;
        UInt16 lastSeq = 0;
        UInt16 head = 0;
        UInt16 window = 0;
        Packet[] networkWindow = new Packet[8];

        public Boolean hasMessages = false;
        public Boolean error = false;

        UInt16 errorCounter = 0;
        UInt16 lastTrans = 0;
        UInt16 transHead = 0;
        UInt16 transWindow = 0;
        Packet[] transmissionWindow = new Packet[8];
        LinkedList<Packet> transmissionQueue = new LinkedList<Packet>();
        
        
        
        
        public Socket(Network network)
        {
            this.network = network;
            this.id = 0;
        }

        public Socket(Network network,UInt16 SocketID)
        {
            this.network = network;
            this.id = SocketID;
        }

        public void Recieve(Packet p)
        {
            //We Only Listen to stuff adressed to us.
            if (p.dest == id)
            {
                if (p.ack)
                {
                    if (p.seq == lastRecv)
                    {
                        //Enter Recovery Mode
                        error = true;
                    }
                    else
                    {
                        
                    }
                    while(transWindow > 0 && transmissionWindow[transHead].seq < p.seq )
                    {
                        transWindow--;
                        transHead = (ushort)((transHead + 1) % 8);
                        lastRecv = p.seq;
                    }
                }
                else
                {
                    if (p.seq == (lastRecv))
                    {
                        //Things are in order. Increment
                        lastRecv++;
                        _TrueRecv(p);
                        if (window != head)
                        {
                            int i = 0;
                            int j = head;
                            while ((j + i) % 8 != window)
                            {
                                if (networkWindow[(j + i) % 8].seq == (lastRecv))
                                {
                                    _TrueRecv(networkWindow[(j + i) % 8]);
                                    head = (ushort)(( head + 1) % 8);
                                    window--;
                                    lastRecv++;
                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if (window > 8)
                        {
                            window++;
                            networkWindow[(window + head - 1) % 8] = p;
                        }
                    }
                    Packet ack = new Packet();
                    ack.ack = true;
                    ack.dest = p.src;
                    ack.src = p.dest;
                    ack.seq = (ushort)(lastRecv);
                    SendP(ack);
                }
            }
        }

        private void _TrueRecv(Packet p)
        {
            Console.WriteLine(network.GetTime() + " | SOCKET " + id + " recieved packet # " + p.seq + " from " + p.src + " sent: " + p.timeStamp);
        }
        public void Transmit(Packet p)
        {
            network.Transmit(p, this);
        }
        public void Send()
        {
            Packet p = new Packet();
            p.dest = dst;
            p.src = id;
            p.seq = lastSeq;
            lastSeq++;
            transmissionQueue.AddLast(p);
            hasMessages = true;
        }
        public void SendP(Packet p)
        {
            transmissionQueue.AddLast(p);
            hasMessages = true;
        }
        public Packet GetNextPacket()
        {
            Packet ret = null;
            if (hasMessages)
            {
                if (transmissionQueue.Count > 0 && transmissionQueue.First().ack){
                    ret = transmissionQueue.First();
                    transmissionQueue.RemoveFirst();
                    if (transmissionQueue.Count == 0) hasMessages = false;
                }
                else if (error)
                {
                    if (errorCounter == 0)
                    {
                        errorCounter = transWindow;
                    }
                    ret = transmissionWindow[(transHead + transWindow - errorCounter) % 8];
                    errorCounter--;
                    if (errorCounter == 0)
                    {
                        errorCounter = 0;
                        error = false;
                    }
                }
                else if (transWindow < 8)
                {
                    ret = transmissionQueue.First();
                    transmissionQueue.RemoveFirst();
                    transWindow++;
                    transmissionWindow[(transWindow + transHead - 1) % 8] = ret;
                    ret.timeStamp = network.GetTime();
                    if (transmissionQueue.Count == 0) hasMessages = false;
                }
                else
                {
                    ret = transmissionWindow[(transHead) % 8];
                    ret.timeStamp = network.GetTime();
                }
            }
            return ret;
        }
    }
}
