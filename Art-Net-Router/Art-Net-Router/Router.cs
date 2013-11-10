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
    class Router
    {

        public static int Verbosity = 5;
        public static bool Monitoring = false;

        Dictionary<Int32, List<Int32>> RoutingTable;
        ArtLib.Transmitter Transmitter;
        byte Sequence = 0;
        int TransmitPort;
        string SubNet;

        public Router(Dictionary<Int32, List<Int32>> RoutingTable, ArtLib.Transmitter Transmitter, int TransmitPort, string SubNet)
        {
            this.RoutingTable = RoutingTable;
            this.Transmitter = Transmitter;
            this.TransmitPort = TransmitPort;
            this.SubNet = SubNet;
        }

        static void Message(int Level, string Message)
        {
            if (Level > Verbosity)
                return;
            Console.WriteLine(Message);
        }

        public void RoutePacket(byte[] Data)
        {
            short universe = (short)(Data[14] * (0x01) + Data[15] * (0x100));
            List<IPEndPoint> RemoteEndPoints = GetRemoteEndPoints(universe);
            Sequence++;
            if (Sequence > 255)
            {
                Sequence = 0;
            }
            Message(4, "Packet from universe " + universe);
            Data[12] = Sequence;

            //If Monitoring, send data to wildcard
            if (Monitoring)
            {
                List<IPEndPoint> Wildcard = GetRemoteEndPoints(-1);
                foreach (IPEndPoint EndPoint in Wildcard)
                {
                    SendPacket(Data, EndPoint);
                }
            }

            //Change universe to 0, so the lamps listen, goddammit.
            Data[14] = 0;
            Data[15] = 0;
            foreach (IPEndPoint EndPoint in RemoteEndPoints)
            {
                SendPacket(Data, EndPoint);
            }
        }

        void SendPacket(byte[] Data, IPEndPoint RemoteEndPoint)
        {
            //if (RemoteEndPoint.Address.Equals(LocalIPAddress()))
            //    return;
            Message(5, "\tIn: [uni] -> " + RemoteEndPoint + "[0]");
            Transmitter.sendRaw(Data, RemoteEndPoint);
        }

        List<IPEndPoint> GetRemoteEndPoints(int uni)
        {
            List<IPEndPoint> RemoteEndPoints = new List<IPEndPoint>();
            if (RoutingTable.ContainsKey(uni))
            {
                List<Int32> SubUniverses = RoutingTable[uni];
                foreach (int SubUniverse in SubUniverses)
                {
                    RemoteEndPoints.AddRange(GetRemoteEndPoints(SubUniverse));
                }
            }
            else
            {
                if (uni < 256) //Universe 255 is excluded
                    RemoteEndPoints.Add(new IPEndPoint(IPAddress.Parse(SubNet + uni.ToString()), TransmitPort));
            }
            return RemoteEndPoints;
        }


        public static IPAddress LocalIPAddress()
        {
            IPHostEntry host;
            IPAddress localIP = null;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                    break;
                }
            }
            return localIP;
        }
    }
}
