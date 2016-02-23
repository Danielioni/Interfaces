using System;
using System.Text;
using System.Net.Sockets;

/// <summary>
/// Port is the bas class that talcs to the TCP/IP port that MOT uses.  The system will layer up from there
/// to encompass parsers to transform XML, JSON, MOT Markup and Delimited text to MOT Markup, and then up to 
/// collection methods including the pit, a monitored directory into which one throws files to import, a Web API,
/// a RESTful/JSON interface and anything else that's needed
/// </summary>
namespace motInbound
{
    public class Port
    {
        public TcpClient tcpSocket = null;
        NetworkStream strm;

        string tcp_address { get; set; }
        string tcp_port { get; set; }

        public Port()
        {
        }

        public Port(string address, string port)
        {
            tcp_address = address;
            tcp_port = port;

            if (!Open())
            {
                System.Environment.Exit(1);
            }
        }

        ~Port()
        {
            Close();
        }

        private bool Open()
        {
            try
            {
                tcpSocket = new TcpClient(tcp_address, Convert.ToInt32(tcp_port));
                strm = tcpSocket.GetStream();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Gateway ArgumentNullException: {0}", e);
                return false;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Gateway SocketException: {0}", e);
                return false;
            }

            return true;
        }

        private void Close()
        {
            if (tcpSocket != null)
            {
                strm.Close();
                tcpSocket.Close();
            }
        }

        public bool Write(string __buf, int __len)
        {
            if (tcpSocket != null)
            {
                strm.Write(Encoding.ASCII.GetBytes(__buf), 0, __len);
                return true;
            }

            return false;
        }

        public bool Read(ref string __buf, int __len)
        {
            if (tcpSocket != null)
            {
                byte[] __readbuf = new byte[256];
                int __retval = 0;

                __retval = strm.Read(__readbuf, 0, 256);
                __buf = Encoding.UTF8.GetString(__readbuf);

                return (__retval == 0);
            }

            return false;
        }

        public bool Reset()
        {
            if (tcpSocket != null)
            {
                Close();
                return Open();
            }

            return false;
        }
    }
}
}
