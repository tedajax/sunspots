using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;

namespace StarForce_PendingTitle_
{
    static class NetClientClass
    {
        public static Profile SelectedProfile;
        public static NetClient Client;
        public static NetAppConfiguration Config;
        public static Queue<NetMessage> MessagesRecieved = new Queue<NetMessage>();
        public static Queue<NetMessage> PositionDataRecieved = new Queue<NetMessage>();
        //this is the reference number that this client uses to identify it self
        public static int ReferenceNumber;

        public static bool AllClientsLoaded = false;
        
        public static List<OnlinePlayer> Players = new List<OnlinePlayer>();

        private static OnlinePlayer findPlayer(int ReferenceNumber)
        {
            foreach (OnlinePlayer o in Players)
            {
                if (o.getReferenceNumber() == ReferenceNumber) return o;
            }
            return null;
        }

        private static void HandleMessage(NetMessage msg)
        {
            // we received a new message
            try
            {
                //find the data code for the message
                byte bit = msg.ReadByte();
                if (bit == 1)
                {
                    //its just a text message
                    MessagesRecieved.Enqueue(msg);
                }
                if (bit == 2)
                {
                    //its position data
                    //find its owner
                    int owner = msg.ReadInt();
                    OnlinePlayer ownplayer = findPlayer(owner);
                    if (ownplayer != null)
                    {
                        ownplayer.enqueueMessage(msg);
                    }
                    //PositionDataRecieved.Enqueue(msg);
                }
               
                if (bit == 3)
                {
                    //A new Player has joined the game. Lets update the playerlist
                    OnlinePlayer newplayer = new OnlinePlayer(msg.ReadString(), msg.ReadInt());
                    Players.Add(newplayer);
                }

                   
                if (bit == 4)
                {
                    //Someone has disconnected, lets remove him
                    //later gameplay needs to figure out which player disconnected and convert him to ai
                    int refnumber = msg.ReadInt();
                    //find the player to remove
                    OnlinePlayer remove = findPlayer(refnumber);
                    if (remove != null)
                    {
                        //if the player exists, remove him
                        Players.Remove(remove);
                    }
                   
                }
                if (bit == 5)
                {
                    //we have succesfully registered with the server
                    //we now have a reference number to send data out with. WooHoo!
                    ReferenceNumber = msg.ReadInt();
                    //lets read the number of clients that are already in the game
                    int NumberofClients = msg.ReadInt();
                    while (NumberofClients > 0)
                    {
                        //lets create a new online player for each client
                        String name = msg.ReadString();
                        int RefNum = msg.ReadInt();
                        OnlinePlayer newplayer = new OnlinePlayer(name, RefNum);
                        Players.Add(newplayer);
                    }
                    //okay, all systems a go!
                }
                if (bit == 6)
                {
                    AllClientsLoaded = true;
                }
                if (bit == 7)
                {
                    //This is Gameplay Data
                    //Lets sort it out a bit further
                    byte TypeData = msg.ReadByte();
                    if (TypeData == 1)
                    {
                        //This is laser data
                        LaserManager.MessagesRecieved.Enqueue(msg);
                    }
                    if (TypeData == 2)
                    {
                        //this is enemy data
                        EnemyManager.MessagesRecieved.Enqueue(msg);
                    }

                }


                    
                
            }
            catch { } // ignore disposal problems
        }

        public static void Update()
        {
            if (Client != null)
            {
                Client.Heartbeat();

                NetMessage msg;
                // read a packet if available
                while ((msg = Client.ReadMessage()) != null)
                    HandleMessage(msg);
            }
        }

        public static void sendMessage(NetMessage sendmsg)
        {
            if (Client != null)
                Client.SendMessage(sendmsg, NetChannel.Unreliable);
                
        }
        public static void sendMessageReliable(NetMessage sendmsg)
        {
            if (Client != null)
                Client.SendMessage(sendmsg, NetChannel.ReliableUnordered);

        }


    }
}
