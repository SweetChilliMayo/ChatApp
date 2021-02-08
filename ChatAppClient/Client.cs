using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class Client
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Console.WriteLine("Instance already exists");
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        //if (UIManager.instance.ipField.text != "")
        //{
        //    ip = UIManager.instance.ipField.text;
        //}

        tcp = new TCP();

        InitializeClientData();

        isConnected = true;
        tcp.Connect();
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
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
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
                    instance.Disconnect();
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
            instance.Disconnect();

            stream = null;
            receievedData = null;
            receieveBuffer = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
        };
        Console.WriteLine("Initialized packets.");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();

            Console.WriteLine("Disconnected from server.");
        }
    }
}
