using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ColorInteraction.Common;

namespace ColorInteraction.Server
{
    internal class Announcer   
    {
        public void Start()
        {
            new Thread(BroadcastAnnounce).Start();
        }

        private void BroadcastAnnounce()
        {
            byte[] data = Encoding.Unicode.GetBytes(Constants.ImHere);

            while (true)
            {
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.OperationalStatus is OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation unicast in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (unicast.Address.AddressFamily is AddressFamily.InterNetwork)
                            {
                                IPAddress broadcast = GetBroadcastAddress(unicast.Address, unicast.IPv4Mask);

                                using (UdpClient udpClient = new UdpClient())
                                {
                                    udpClient.Send(data, data.Length, new IPEndPoint(broadcast, 2727));
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();
            byte[] broadcastAddress = new byte[ipAdressBytes.Length];

            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | ~subnetMaskBytes[i]);
            }
            return new IPAddress(broadcastAddress);
        }
    }
}