using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACKSim
{
    ///La La La La... I am a network!
    ///This resembles a simple, single hub network.
    ///Should also function much like one.
    class Network
    {
        Random rand = new Random();
        public Double lossRate = .2;
        public int defaultTransitTime = 2;

        uint time = 1000;
        LinkedList<Socket> sockets = new LinkedList<Socket>();
        LinkedList<TransitPacket> transit = new LinkedList<TransitPacket>();
        public Socket GenerateSocket()
        {
            Socket s = new Socket(this);
            sockets.AddLast(s);
            return s;
        }

        public void Transmit(Packet p, Socket self)
        {
            if (p == null)
            {

                Console.WriteLine(GetTime() + " | No Packet");
            }
            else
            {
                //Console.WriteLine(GetTime() + " | packet recv, from " + p.src + " to " + p.dest + " ack?: " + p.ack + " seq " + p.seq + " ");
                if (rand.NextDouble() > lossRate)
                {
                    Console.WriteLine(GetTime() + " | packet recv, from " + p.src + " to " + p.dest + " ack?: " + p.ack + " seq " + p.seq + " ");
                    //Console.WriteLine("Success");
                    foreach (Socket s in sockets)
                    {
                        if (s != self) s.Recieve(p);
                    }
                }
                else
                {
                    //Console.WriteLine("Loss");
                }
            }
        }

        public void Tick()
        {
            foreach (Socket s in sockets)
            {
                if (s.hasMessages)
                {
                    transit.AddLast(new TransitPacket(s.GetNextPacket(),defaultTransitTime, s));
                    Console.WriteLine(GetTime() + " | packet sent, from " + transit.Last().p.src + " to " + transit.Last().p.dest + " ack?: " + transit.Last().p.ack + " seq " + transit.Last().p.seq + " ");
                }
            }
            foreach ( TransitPacket t in transit)
            {
                t.transitTime--;
            }
            while(transit.Count > 0 && transit.First().transitTime <= 0)
            {

                Transmit(transit.First().p , transit.First().source);
                transit.RemoveFirst();
            }
            time++;
        }

        public uint GetTime()
        {
            return time;
        }

        internal bool hasMessages()
        {
            return transit.Count > 0;
        }
    }

    class TransitPacket
    {
        public Packet p;
        public int transitTime;
        public Socket source;
        public TransitPacket(Packet p,int time,Socket s)
        {
            this.p = p;
            this.transitTime = time;
            this.source = s;
        }
    }
}
