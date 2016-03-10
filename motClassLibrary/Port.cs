using System;
using System.Text;
using System.Net.Sockets;

/// <summary>
/// Port is the bas class that talcs to the TCP/IP port that MOT uses.  The system will layer up from there
/// to encompass parsers to transform XML, JSON, MOT Markup and Delimited text to MOT Markup, and then up to 
/// collection methods including the pit, a monitored directory into which one throws files to import, a Web API,
/// a RESTful/JSON interface and anything else that's needed
/// </summary>

namespace motInboundLib
{
    public class Port
    {
        public TcpClient tcpSocket = null;
        NetworkStream dataStream;

        public string tcp_address { get; set; }
        public string tcp_port { get; set; }

        public Port()
        {
        }

        public Port(string address, string port)
        {
            tcp_address = address;
            tcp_port = port;

            try
            {
                Open();
            }
            catch (SocketException e)
            {
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to open socket: " + address + "/" + port);
                //System.Environment.Exit(1);
            }
        }

        ~Port()
        {
            Close();
        }

        public void Open()
        {
            try
            {
                tcpSocket = new TcpClient(tcp_address, Convert.ToInt32(tcp_port));
                dataStream = tcpSocket.GetStream();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Gateway ArgumentNullException: {0}", e);
                throw new Exception("Invalid Argument");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Gateway SocketException: {0}", e);
                throw;
            }
        }

        public void Close()
        {
            if (tcpSocket != null)
            {
                dataStream.Close();
                tcpSocket.Close();
            }
        }

        public bool Write(string __buf, int __len)
        {
            if (tcpSocket != null)
            {
                dataStream.Write(Encoding.ASCII.GetBytes(__buf), 0, __len);
                return true;
            }

            return false;
        }

        public bool Read(ref string __buf)
        {
            if (tcpSocket != null)
            {
                byte[] __readbuf = new byte[256];
                int __retval = 0;

                __retval = dataStream.Read(__readbuf, 0, 256);
                __buf = Encoding.UTF8.GetString(__readbuf);

                return (__retval == 0);
            }

            return false;
        }

        public bool Reset()
        {
            if (tcpSocket != null)
            {
                try
                {
                    Close();
                    Open();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
