using System.Collections;
using System.Collections.Generic;

namespace ChatAppClient
{
    public class ClientSend
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.tcp.SendData(_packet);
        }

        #region Packets
        public static void Message()
        {
            using (Packet _packet = new Packet((int)ClientPackets.messageReceieved))
            {
                _packet.Write(Client.myId);
                _packet.Write(Form1.usernameField);

                SendTCPData(_packet);
            }
        }
        #endregion
    }
}