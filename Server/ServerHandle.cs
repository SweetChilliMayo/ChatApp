using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ChatAppServer
{
    class ServerHandle
    {
        public static void WelcomeReceieved(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now client {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Client \"{_username}\" (ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

        public static void MessageReceieved(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"Message receieved by {_fromClient}");
        }
    }
}
