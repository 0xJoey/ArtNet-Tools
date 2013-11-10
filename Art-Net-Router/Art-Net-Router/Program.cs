using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtLib;
using System.Net;
using System.Net.Sockets;

namespace Art_Net_Router
{
    class Program
    {
        static int listenPort = 16454;
        static int sendPort = 6454;

        static int Verbosity = 5;

        static void Message(int Level, string Message)
        {
            if (Level > Verbosity)
                return;
            Console.WriteLine(Message);
        }

        static string SubNet = "";

        static Dictionary<Int32, List<Int32>> ReadRoutingTable(string Path)
        {
            Dictionary<Int32, List<Int32>> RoutingTable = new Dictionary<Int32, List<Int32>>();
            string Line;
            System.IO.StreamReader File = new System.IO.StreamReader(Path);
            while ((Line = File.ReadLine()) != null)
            {
                Line = Line.Replace("\t", "").Replace(" ", "");
                if (Line.Length == 0)
                    continue;
                Line = Line.Split('#') [0];
                if (Line.Length == 0)
                    continue;
                string[] Tokens = Line.Split('=');
                if (Tokens[0].ToLower().Equals("subnet"))
                    SubNet = Tokens[1];
                else if (Tokens[0].ToLower().Equals("listening_port"))
                    listenPort = Int32.Parse(Tokens[1]);
                else if (Tokens[0].ToLower().Equals("transmitting_port"))
                    sendPort = Int32.Parse(Tokens[1]);
                else if (Tokens[0].ToLower().Equals("log_level"))
                    Verbosity = Router.Verbosity = Int32.Parse(Tokens[1]);
                else
                {
                    int Key = Int32.Parse(Tokens[0]);
                    Tokens = Tokens[1].Split(',');
                    List<Int32> SubUniverses = new List<Int32>();
                    foreach (string Value in Tokens)
                    {
                        SubUniverses.Add(Int32.Parse(Value));
                    }
                    RoutingTable[Key] = SubUniverses;
                }
            }
            File.Close();
            return RoutingTable;
        }


        static void Main(string[] args)
        {
            Message(3, "Starting router...");
            Dictionary<Int32, List<Int32>> D = ReadRoutingTable(".\\artnetroute.cfg");
            Receiver S = new Receiver(listenPort);
            Transmitter T = new Transmitter(SubNet+"255", sendPort, false, 0);
            Router R = new Router(D, T, 6454, SubNet);
            Message(3,"Router started on "+Router.LocalIPAddress());
            int counter = 0;
            while (true)
            {
                R.RoutePacket(S.receiveRaw());
            }
        }

        
    }
}
