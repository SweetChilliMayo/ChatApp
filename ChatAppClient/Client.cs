using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Windows.Forms;

namespace ChatAppClient
{
    public class Client
    {
        public static int dataBufferSize = 4096;

        public static string ip = "127.0.0.1";
        public static int port = 26950;
        public static int myId = 0;
        public static TCP tcp;

        public static bool isConnected = false;
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static void ConnectToServer()
        {
            tcp = new TCP();

            InitializeClientData();

            isConnected = true;
            tcp.Connect();

            Form1.Instance.Connected();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receievedData;
            private byte[] receieveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receieveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(ip, port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receievedData = new Packet();

                stream.BeginRead(receieveBuffer, 0, dataBufferSize, ReceieveCallback, null);
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceieveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Client.Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receieveBuffer, _data, _byteLength);

                    receievedData.Reset(HandleData(_data));
                    stream.BeginRead(receieveBuffer, 0, dataBufferSize, ReceieveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error recieving TCP data: {_ex}");
                    Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receievedData.SetBytes(_data);

                if (receievedData.UnreadLength() >= 4)
                {
                    _packetLength = receievedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receievedData.UnreadLength())
                {
                    byte[] _packetBytes = receievedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](_packet);
                        }
                    });

                    _packetLength = 0;
                    if (receievedData.UnreadLength() >= 4)
                    {
                        _packetLength = receievedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            private void Disconnect()
            {
                Client.Disconnect();

                stream = null;
                receievedData = null;
                receieveBuffer = null;
                socket = null;
            }
        }

        private static void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.message, ClientHandle.Message },
        };
            Console.WriteLine("Initialized packets.");
        }

        public static void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();

                Form1.Instance.Disconnected();

                Console.WriteLine("Disconnected from server.");
            }
        }
    }
}
