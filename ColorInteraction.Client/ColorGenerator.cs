using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ColorInteraction.Common;
using Newtonsoft.Json;

namespace ColorInteraction.Client
{
    internal class ColorGenerator
    {
        public static IPAddress ServerIpAddress;

        public delegate void ColorGeneratedHandler(Color color);

        public event ColorGeneratedHandler OnColorGenerated;

        public void Start()
        {
            new Thread(Generate).Start();
        }

        private void Generate()
        {
            Random random = new Random();

            while (true)
            {
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

                ColorMessage colorMessage = new ColorMessage
                {
                    Color = color,
                    MachineName = Environment.MachineName
                };

                string data = JsonConvert.SerializeObject(colorMessage);

                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(new IPEndPoint(ServerIpAddress, 2727));

                        using (StreamWriter writer = new StreamWriter(tcpClient.GetStream()))
                        {
                            writer.Write(data);
                        }
                    }

                    OnColorGenerated?.Invoke(color);
                }
                catch
                {
                    Console.WriteLine("Unable to connect to the server.");
                }

                Thread.Sleep(5000);
            }
        }
    }
}