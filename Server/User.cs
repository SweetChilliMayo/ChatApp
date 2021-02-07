using System;
using System.Collections.Generic;
using System.Text;

namespace ChatAppServer
{
    class User
    {
        public int id;
        public string username;

        public User(int _id, string _username)
        {
            id = _id;
            username = _username;
        }

        public void SendData(string _input)
        {
            ServerSend.Message(id, _input);
        }
    }
}
