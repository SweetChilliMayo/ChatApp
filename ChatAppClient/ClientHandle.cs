using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;

namespace ChatAppClient
{
    public class ClientHandle
    {
        public static void Message(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            Console.WriteLine($"Message from server: {_msg}");
            Client.myId = _myId;
            ClientSend.Message();
        }
    }
}
