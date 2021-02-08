﻿using System.Collections;
using System.Collections.Generic;

public class ClientSend
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets
    public static void Message()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            //_packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }
    #endregion
}
