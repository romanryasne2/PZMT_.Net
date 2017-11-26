using System;
using System.Drawing;
using System.Net;

namespace ColorInteraction.Client
{
    internal class Client
    {
        private bool IsColorGeneratorStarted;

        public void Start()
        {
            Listener listener = new Listener();

            listener.OnServerIpAddressChanged += Reconfigure;

            listener.Start();
        }

        private void Reconfigure(IPAddress ipAddress)
        {
            ColorGenerator.ServerIpAddress = ipAddress;

            Console.WriteLine($"Server IP Address is {ipAddress}");

            if (IsColorGeneratorStarted)
            {
                return;
            }

            ColorGenerator colorGenerator = new ColorGenerator();

            colorGenerator.OnColorGenerated += WriteColor;

            colorGenerator.Start();

            IsColorGeneratorStarted = true;
        }

        private void WriteColor(Color color)
        {
            Console.WriteLine(color);
        }
    }
}