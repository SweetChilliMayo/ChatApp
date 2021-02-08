using System;
using System.Collections.Generic;
using System.Text;

namespace ChatAppServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxClients; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxClients; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        #region Packets
        public static void Message(int _fromClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.message))
            {
                _packet.Write(_msg);
                _packet.Write(_fromClient);

                Console.WriteLine($"Sending message: {_msg}");

                SendTCPDataToAll(_fromClient, _packet);
            }
        }
        #endregion
    }
}
