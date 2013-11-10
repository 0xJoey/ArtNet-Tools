using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ArtLib;

namespace ArtNetController
{
    class Program
    {

        enum Modes
        {
            Off = 0, Scan = 1, Police = 2, Clock = 3, Rainbow = 4, SlowRainbow = 5, Random = 6, ShowAddress = 7, Strobe = 8
        }
        static void Main(string[] args)
        {
            Transmitter an = new Transmitter("127.0.0.1", 16454, false, 0);
            //Transmitter AN = new Transmitter("172.31.41.111", 6454, false);
            //Transmitter an = new Transmitter("192.168.42.255", 6454, false, 0);
            Pixel P1 = new Pixel(255,0,0);
            Pixel P2 = new Pixel(0,0,255);
            Console.WriteLine("What uni to send on?");
            string entryuni = Console.ReadLine();
            short universe = Convert.ToInt16(entryuni);
            Console.WriteLine("Modes:");
            Console.WriteLine("0 - Off");
            Console.WriteLine("1 - Scan");
            Console.WriteLine("2 - Police");
            Console.WriteLine("3 - Clock");
            Console.WriteLine("4 - Rainbow");
            Console.WriteLine("5 - Rainbow(Slow)");
            Console.WriteLine("6 - Random Colors");
            Console.WriteLine("7 - Show Address*");
            Console.WriteLine("8 - Strobe");
            Modes mode = Modes.Rainbow;
            int ModeK = Convert.ToInt32(Console.ReadKey().KeyChar.ToString());
            mode = (Modes)ModeK;
            while (true)
            {
                switch (mode)
	            {
                    case Modes.Off:
                        an.setAllPixels(new Pixel(0, 0, 0));
                        an.sendData();
                        System.Threading.Thread.Sleep(50000);
                     break;
		            case Modes.Scan:
                        an.setUniverse(universe);
                        Pattern1(an, new Pixel(255, 0, 0));
                        System.Threading.Thread.Sleep(25);
                        Pattern1(an, new Pixel(255, 255, 0));
                        System.Threading.Thread.Sleep(25);
                        Pattern1(an, new Pixel(0, 255, 0));
                        System.Threading.Thread.Sleep(25);
                        Pattern1(an, new Pixel(0, 255, 255));
                        System.Threading.Thread.Sleep(25);
                        Pattern1(an, new Pixel(0, 0, 255));
                        System.Threading.Thread.Sleep(25);
                        Pattern1(an, new Pixel(255, 0, 255));
                        System.Threading.Thread.Sleep(25);
                     break;
                    case Modes.Police:
                        an.setUniverse(universe);
                        Pattern2(an, P1, P2);
                        System.Threading.Thread.Sleep(500);
                        Pattern2(an, P2, P1);
                        System.Threading.Thread.Sleep(500);
                     break;
                    case Modes.Clock:
                        an.setUniverse(universe);
                        Clock(an);
                        System.Threading.Thread.Sleep(1000);
                     break;
                    case Modes.Rainbow:
                        an.setUniverse(universe);
                        Pattern3(an, new Pixel(255, 0, 0), new Pixel(0, 255, 0), 20);
                        Pattern3(an, new Pixel(0, 255, 0), new Pixel(0, 0, 255), 20);
                        Pattern3(an, new Pixel(0, 0, 255), new Pixel(255, 0, 0), 20);
                        System.Threading.Thread.Sleep(100);
                     break;
                    case Modes.SlowRainbow:
                        an.setUniverse(universe);
                        Pattern3(an, new Pixel(255, 0, 0), new Pixel(0, 255, 0), 300);
                        Pattern3(an, new Pixel(0, 255, 0), new Pixel(0, 0, 255), 300);
                        Pattern3(an, new Pixel(0, 0, 255), new Pixel(255, 0, 0), 300);
                        System.Threading.Thread.Sleep(100);
                     break;
                    case Modes.Strobe:
                        an.setUniverse(universe);
                        an.setAllPixels(new Pixel(0, 0, 0));
                        an.sendData();
                        System.Threading.Thread.Sleep(100);
                        an.setAllPixels(new Pixel(255, 255, 255));
                        an.sendData();
                        System.Threading.Thread.Sleep(100);
                     break;
                    default:
                        an.setUniverse(universe);
                        Pattern3(an, new Pixel(255, 0, 0), new Pixel(0, 255, 0), 20);
                        Pattern3(an, new Pixel(0, 255, 0), new Pixel(0, 0, 255), 20);
                        Pattern3(an, new Pixel(0, 0, 255), new Pixel(255, 0, 0), 20);
                     break;
                    case Modes.ShowAddress:
                         ShowAddress(an);
                         System.Threading.Thread.Sleep(500);
                     break;
                    case Modes.Random:
                         RandomColor(an);
                         System.Threading.Thread.Sleep(500);
                     break;
	            }
            }
        }
        static void Pattern1(Transmitter AN, Pixel Pi)
        {
            for (int i = 0; i < 150; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    AN.setPixel(j, Pi);
                }
                System.Threading.Thread.Sleep(20);
                AN.sendData();
            }
        }

        static void Pattern2(Transmitter AN, Pixel P1, Pixel P2)
        {
            for (int i = 75; i < 150; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    AN.setPixel(j, P1);
                }
            }
            for (int i = 0; i < 75; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    AN.setPixel(j, P2);
                }
            }
            AN.sendData();
        }

        static void Pattern3(Transmitter AN, Pixel P1, Pixel P2, int dly)
        {
            int dR = P2.R - P1.R;
            int dG = P2.G - P1.G;
            int dB = P2.B - P1.B;
            Pixel P = P1;
            for (byte i = 0; i < 255; i++)
            {
                P = new Pixel((byte)(P.R + (dR / 255f)), (byte)(P.G + (dG / 255f)), (byte)(P.B + (dB / 255f)));
                AN.setAllPixels(P);
                AN.sendData();
                System.Threading.Thread.Sleep(dly);
            }
        }

        static void Pattern4(Transmitter AN, Pixel P1, Pixel P2, Pixel P3, Pixel P4)
        {
            for (int i = 0; i < 10; i++)
            {
                AN.setPixel(i, P1);
            }
            for (int i = 10; i < 20; i++)
            {
                AN.setPixel(i, P2);
            }
            for (int i = 20; i < 30; i++)
            {
                AN.setPixel(i, P3);
            }
            for (int i = 30; i < 40; i++)
            {
                AN.setPixel(i, P4);
            }
            for (int i = 40; i < 50; i++)
            {
                AN.setPixel(i, new Pixel(255,255,0));
            }
        }



        static void blinkAddress(Transmitter an)
        {

        }

        static void RandomColor(Transmitter an)
        {
            Random RNG = new Random();
            byte[] colors = new byte[3];

            for (int j = 0; j < 150; j++)
            {
                RNG.NextBytes(colors);
                int R = colors[0] % 3 * 127;
                int G = colors[1] % 3 * 127;
                int B = colors[2] % 3 * 127;
                an.setPixel(j, new Pixel((byte)R, (byte)G, (byte)B));
            }
            an.sendData();
        }

        static void ShowAddress(Transmitter AN)
        {
            Console.WriteLine("Run");
            for (short i = 1; i <= 255; i++)
            {
                short uni = (short)(i);
                Pixel[] pixels = GetHexColors(i);
                Console.WriteLine(i.ToString("X"));
                Pattern4(AN, pixels[0], pixels[1], pixels[2], pixels[3]);
                AN.sendData(uni);
                System.Threading.Thread.Sleep(100);
            }
        }

        static Pixel[] GetHexColors(short Uni)
        {
            Pixel[] pixels = new Pixel[4];
            string hex = Uni.ToString("X");
            if (hex.Length == 1)
            {
                hex = "0" + hex;
            }
            for (int i = 0; i < 2; i++)
            {
                switch (hex.Substring(i, 1))
                {
                    case "0":
                        pixels[i * 2 + 1] = new Pixel(255, 0, 0);
                        pixels[i * 2] = new Pixel(255, 0, 0);
                        break;
                    case "1":
                        pixels[i * 2 + 1] = new Pixel(0, 255, 0);
                        pixels[i * 2] = new Pixel(255, 0, 0);
                        break;
                    case "2":
                        pixels[i * 2 + 1] = new Pixel(0, 0, 255);
                        pixels[i * 2] = new Pixel(255, 0, 0);
                        break;
                    case "3":
                        pixels[i * 2 + 1] = new Pixel(255, 255, 255);
                        pixels[i * 2] = new Pixel(255, 0, 0);
                        break;
                    case "4":
                        pixels[i * 2 + 1] = new Pixel(255, 0, 0);
                        pixels[i * 2] = new Pixel(0, 255, 0);
                        break;
                    case "5":
                        pixels[i * 2 + 1] = new Pixel(0, 255, 0);
                        pixels[i * 2] = new Pixel(0, 255, 0);
                        break;
                    case "6":
                        pixels[i * 2 + 1] = new Pixel(0, 0, 255);
                        pixels[i * 2] = new Pixel(0, 255, 0);
                        break;
                    case "7":
                        pixels[i * 2 + 1] = new Pixel(255, 255, 255);
                        pixels[i * 2] = new Pixel(0, 255, 0);
                        break;
                    case "8":
                        pixels[i * 2 + 1] = new Pixel(255, 0, 0);
                        pixels[i * 2] = new Pixel(0, 0, 255);
                        break;
                    case "9":
                        pixels[i * 2 + 1] = new Pixel(0, 255, 0);
                        pixels[i * 2] = new Pixel(0, 0, 255);
                        break;
                    case "A":
                        pixels[i * 2 + 1] = new Pixel(0, 0, 255);
                        pixels[i * 2] = new Pixel(0, 0, 255);
                        break;
                    case "B":
                        pixels[i * 2 + 1] = new Pixel(255, 255, 255);
                        pixels[i * 2] = new Pixel(0, 0, 255);
                        break;
                    case "C":
                        pixels[i * 2 + 1] = new Pixel(255, 0, 0);
                        pixels[i * 2] = new Pixel(255, 255, 255);
                        break;
                    case "D":
                        pixels[i * 2 + 1] = new Pixel(0, 255, 0);
                        pixels[i * 2] = new Pixel(255, 255, 255);
                        break;
                    case "E":
                        pixels[i * 2 + 1] = new Pixel(0, 0, 255);
                        pixels[i * 2] = new Pixel(255, 255, 255);
                        break;
                    case "F":
                        pixels[i * 2 + 1] = new Pixel(255, 255, 255);
                        pixels[i * 2] = new Pixel(255, 255, 255);
                        break;
                }
            }
            return pixels;
        }

        static void Clock(Transmitter AN)
        {
            DateTime Time = DateTime.Now;
            byte sec_part = (byte)((Time.Second / 60f) * 60f);
            byte min_part = (byte)((Time.Minute / 60f) * 60f);
            byte hr_part = (byte)((Time.Hour / 24f) * 24f);
            Console.Write(hr_part.ToString() + "-");
            Console.Write(min_part.ToString() + "-");
            Console.WriteLine(sec_part.ToString());
            for (int i = 0; i < 60; i++)
            {
                byte R = (sec_part >= i) ? (byte)255 : (byte)0;
                Pixel Clk = new Pixel(R, 0, 0);
                AN.setPixel(i, Clk);
            }
            for (int i = 60; i < 120; i++)
            {
                byte G = (min_part + 60 >= i) ? (byte)255 : (byte)0;
                Pixel Clk = new Pixel(0, G, 0);
                AN.setPixel(i, Clk);
            }
            for (int i = 120; i < 144; i++)
            {
                byte B = (hr_part + 120 >= i) ? (byte)255 : (byte)0;
                Pixel Clk = new Pixel(0, 0, B);
                AN.setPixel(i, Clk);
            }
            for (int i = 144; i < 150; i++)
            {
                Pixel Clk = new Pixel(128, 0, 128);
                AN.setPixel(i, Clk);
            }
            //byte G = (min_part >= i) ? (byte)255 : (byte)0;
            //byte B = (hr_part >= i) ? (byte)255 : (byte)0;
            AN.sendData();
        }
    }
}
