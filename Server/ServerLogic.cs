using System;
using System.Collections.Generic;
using System.Text;

namespace ChatAppServer
{
    class ServerLogic
    {
        public static void Update()
        {
            //foreach (Client _client in Server.clients.Values)
            //{
            //    if (_client.user != null)
            //    {
            //        _client.user.Update();
            //    }
            //}

            ThreadManager.UpdateMain();
        }
    }
}
