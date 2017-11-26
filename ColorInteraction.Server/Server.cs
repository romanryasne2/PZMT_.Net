using System;
using System.Drawing;
using ColorInteraction.Common;

namespace ColorInteraction.Server
{
    internal class Server
    {
        private readonly Database.Database Database = new Database.Database();

        public void Start()
        {
            new Announcer().Start();

            ColorCollector colorCollector = new ColorCollector();

            colorCollector.OnColorsChanged += WriteNewColor;

            colorCollector.Start();
        }

        private void WriteNewColor(string machine, Color color, DateTime lastUpdate)
        {
            Console.WriteLine($"{machine} {color} {lastUpdate}");

            Database.Log(new ColorMessage
            {
                Color = color,
                MachineName = machine
            });
        }
    }
}