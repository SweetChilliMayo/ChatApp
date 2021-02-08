using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;

public class ClientHandle
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Console.WriteLine($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.Message();
    }
}
