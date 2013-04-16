using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACKSim
{
    /***
     * You need to write a program to emulate the sliding window behavior of a sender 
     * process that uses selective repeat ARQ with a window size of 8 packets.  Your 
     * program should allow users to specify the number of packets to be "sent" 
     * (I put a " " here to imply that the sender needs NOT actually send the packet.) 
     * to a receiver process and which subset of them will be "lost in the network".  
     * In this emulation, you have the liberty to specify the roundtrip delay and the 
     * times these packets arrive at the sender process, ready to be sent out.  You 
     * need NOT write any TCP/UDP socket programs.  Instead, you may simply assume that 
     * each packet that is not "lost in the network" will magically be received and the 
     * sender process will magically get a corresponding acknowledgement from the 
     * receiver.  Then, the timers associated with these lost packets will expire after 
     * a certain amount of time (say 5 seconds) and they will be "resent".
     * 
     * Your program needs to output which packet (identified by a sequence number) 
     * will be "sent" out at what time.
     */

    /*Inputs:
     * 
     * # of packets to be sent
     * % of packets lost
     * packet delay
     * 
     */
    class Program
    {
        static void Main(string[] args)
        {
            Network n = new Network();
            int count = 20;
            if (args.Length == 3)
            {
                count = int.Parse(args[0]);
                n.lossRate = double.Parse(args[1]);
                n.defaultTransitTime = int.Parse(args[2]);
            }
            //Console.WriteLine("Hello World");
            Console.Beep(440, 100);
            
            Socket s1 = n.GenerateSocket();
            s1.dst = 2;
            s1.id = 1;
            Socket s2 = n.GenerateSocket();
            s2.dst = 1;
            s2.id = 2;
            while (count > 0)
            {
                s1.Send();
                count--;
            }
            while (s1.hasMessages || n.hasMessages())
            {
                n.Tick();
                System.Threading.Thread.Sleep(250);
                n.Tick();
                System.Threading.Thread.Sleep(250);
            }
            System.Threading.Thread.Sleep(5000);
            //Console.In.ReadLine();
        }
    }
}
