using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ColorInteraction.Common;

namespace ColorInteraction.Client
{
    internal class Listener
    {
        public delegate void ServerIpAddressChangedHandler(IPAddress ipAddress);

        public event ServerIpAddressChangedHandler OnServerIpAddressChanged;

        public IPAddress ServerIpAddress { get; private set; } = IPAddress.Any;

        public void Start()
        {
            new Thread(Listen).Start();
        }

        private void Listen()
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint serverIpAddress = new IPEndPoint(IPAddress.Any, 2727);

                udpClient.Client.Bind(serverIpAddress);

                while (true)
                {
                    byte[] data = udpClient.Receive(ref serverIpAddress);

                    string message = Encoding.Unicode.GetString(data);

                    if (message == Constants.ImHere && !ServerIpAddress.Equals(serverIpAddress.Address))
                    {
                        ServerIpAddress = serverIpAddress.Address;

                        OnServerIpAddressChanged?.Invoke(ServerIpAddress);
                    }
                }
            }
        }
    }
}