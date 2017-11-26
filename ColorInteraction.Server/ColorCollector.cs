using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ColorInteraction.Common;
using Newtonsoft.Json;

namespace ColorInteraction.Server
{
    internal class ColorCollector
    {
        public Dictionary<string, (Color Color, DateTime LastUpdate)> Colors { get; } = new Dictionary<string, (Color, DateTime)>();

        public delegate void ColorsChangedHandler(string machine, Color color, DateTime lastUpdate);

        public event ColorsChangedHandler OnColorsChanged;

        public void Start()
        {
            new Thread(Collect).Start();
        }

        private void Collect()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 2727);

            tcpListener.Start();

            while (true)
            {
                using (TcpClient tcpClient = tcpListener.AcceptTcpClient())
                using (StreamReader reader = new StreamReader(tcpClient.GetStream()))
                {
                    string data = reader.ReadToEnd();

                    ColorMessage colorMessage = JsonConvert.DeserializeObject<ColorMessage>(data);

                    string machine = $"{(tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address} - {colorMessage.MachineName}";

                    (Color Color, DateTime LastUpdate) value = (colorMessage.Color, DateTime.Now);

                    if (Colors.ContainsKey(machine))
                    {
                        if (Colors[machine].Color != value.Color)
                        {
                            Colors[machine] = value;

                            OnColorsChanged?.Invoke(machine, value.Color, value.LastUpdate);
                        }
                        else
                        {
                            Colors[machine] = value;
                        }
                    }
                    else
                    {
                        Colors.Add(machine, value);

                        OnColorsChanged?.Invoke(machine, value.Color, value.LastUpdate);
                    }
                }
            }
        }
    }
}