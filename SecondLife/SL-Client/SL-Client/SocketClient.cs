using System;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;
//using System.Runtime.InteropServices;
//using System.IO;

namespace libsecondlife.TestClient
{
    [Serializable()]
    public class CommandStruct
    {
        public string command = String.Empty;
        public string message = String.Empty;
    }
    /// 
    /// Summary description for Client.
    /// 
    public sealed class SocketClient
    {

        static readonly SocketClient instance = new SocketClient();


        /////////////////////////////////////////////////////////////////////////////
        ///Variables & Properties

        /////////////////////////////////////////////////////////////////////////////
        //private StreamReader clientStreamReader;
        //private StreamWriter clientStreamWriter;
        private ClientManager client;
        IAsyncResult m_asynResult;
        public AsyncCallback pfnCallBack;
        public Socket m_socClient;

        /////////////////////////////////////////////////////////////////////////////
        ///Constructor
        static SocketClient()
        {
        }
        SocketClient()
        {
        }

        public static SocketClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void openConnection()
        {
            try
            {
                Console.WriteLine("Starting Botcontrol-Client...");
                m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // get the remote IP address...
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                int iPortNo = 8787;
                //create the end point
                IPEndPoint ipEnd = new IPEndPoint(ip.Address, iPortNo);
                //connect to the remote host...
                m_socClient.Connect(ipEnd);
                m_socClient.SendBufferSize = 2048;
                //watch for data ( asynchronously )...
                WaitForData();
            }
            catch (SocketException se)
            {
               Console.Error.WriteLine(se.Message);
            }
        }
        /*
        public byte[] SerializeExact(object anything)
        {
            int structsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(structsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] streamdatas = new byte[structsize];
            Marshal.Copy(buffer, streamdatas, 0, structsize);
            Marshal.FreeHGlobal(buffer);
            return streamdatas;
        }
        */
        public void WaitForData()
        {
            try
            {
                if (pfnCallBack == null)
                {
                    pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket();
                theSocPkt.thisSocket = m_socClient;
                // now start to listen for any data...
                m_asynResult = m_socClient.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnCallBack, theSocPkt);
            }
            catch (SocketException se)
            {
                Console.Error.WriteLine(se.Message);
            }
        }
        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
                //end receive...
                int iRx = 0;
                iRx = theSockId.thisSocket.EndReceive(asyn);
                Console.WriteLine("iRX: " + iRx);

                XmlSerializer serializer = new XmlSerializer(typeof(CommandStruct));
                // StreamReader kann Dateien auslesen.
                //byte[] data = Convert.FromBase64String(message);

                // Klasse wird deserialisiert
                try
                {
                    MemoryStream ms = new MemoryStream(theSockId.dataBuffer);
                    CommandStruct cstruct = (CommandStruct)serializer.Deserialize(ms);
                    ms.Close();
                    string result = cstruct.command + " " + cstruct.message;
                    Console.WriteLine("Received: " + result);

                    //char[] chars = new char[iRx + 1];
                    //System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    //int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                    //System.String szData = new System.String(chars);
                    //Call Command
                    //Console.WriteLine("Received from BotControl: " + szData);
                    client.DoCommandAll(result, null, null);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                WaitForData();
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.Error.WriteLine(se.Message);
            }
        }

        public void sendData(byte[] message)
        {
            try
            {
                /*
                Object objData = message;
                Console.WriteLine("Sending msg: " + message);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                 * */
                m_socClient.Send(message,0,message.Length,SocketFlags.None);
            }
            catch (SocketException se)
            {
                Console.Error.WriteLine(se.Message);
            }
        }

        public void closeConnection()
        {
            if (m_socClient != null)
            {
                //m_socClient.Shutdown (SocketShutdown.Send);
                m_socClient.Close();
                m_socClient = null;
            }
            Console.WriteLine("BotControl-Client closed");
        }

        public class CSocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[512];
        }


        /////////////////////////////////////////////////////////////////////////////
        ///Connect to server
        ///
        /*
        private bool ConnectToServer()
        {
            //connect to server at given port
            try
            {
                TcpClient tcpClient = new TcpClient("localhost", 8787);
                Console.WriteLine("Connected to BotControl");
                //get a network stream from server
                NetworkStream clientSockStream = tcpClient.GetStream();
                clientStreamReader = new StreamReader(clientSockStream);
                clientStreamWriter = new StreamWriter(clientSockStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            this.listenForCommands();
            return true;
        }

        public void sendMessage(string message)
        {
            try
            {
                //send message to server
                clientStreamWriter.WriteLine(message);
                clientStreamWriter.Flush();
            }
            catch (Exception se)
            {
                Console.WriteLine(se.StackTrace);
            }
        }

        private void listenForCommands()
        {
            while (true)
            {
                string text = clientStreamReader.ReadLine();
                client.DoCommandAll(text, null, null);
            }

            if (pfnCallBack == null)
                pfnCallBack = new AsyncCallback(OnDataReceived);
            // now start to listen for any data...
            m_asynResult =
            m_socClient.BeginReceive(m_DataBuffer, 0, m_DataBuffer.Length, SocketFlags.None, pfnCallBack, null);
            this.clientStreamReader.beg
        }
         * */

        public void setClient(ClientManager client)
        {
            this.client = client;
        }
    }
}