using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WCSC
{
    public class Device
    {
        public TcpClient clientSocket = null;
        public NetworkStream serverStream = default(NetworkStream);
        public string device_ip = string.Empty;
        public string device_number = string.Empty;
        public string AorK = string.Empty;
        public int device_name;

        public Device(TcpClient clientSockett, NetworkStream serverStreamm, string device_ipp, string device_numberr, int device_namee, string AorKK)
        {
            clientSocket = clientSockett;
            serverStream = serverStreamm;
            device_ip = device_ipp;
            device_number = device_numberr;
            device_name = device_namee;
            AorK = AorKK;
        }

    }
}
