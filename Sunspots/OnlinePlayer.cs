
using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;

namespace StarForce_PendingTitle_
{
    class OnlinePlayer
    {
        /// <summary>
        /// the name of the online player
        /// </summary>
        String Name;
        /// <summary>
        /// the reference number of the online player. Whenever an online player sends a message, it will be sent with this
        /// number being used as the identifier
        /// </summary>
        int ReferenceNumber;
        /// <summary>
        /// messages that were delivered BY this online player from another computer
        /// this is positional data for this players ship
        /// </summary>
        Queue<NetMessage> UnreadMessages;

        public OnlinePlayer(String Name, int ReferenceNumber)
        {
            this.Name = Name;
            this.ReferenceNumber = ReferenceNumber;
            this.UnreadMessages = new Queue<NetMessage>();
        }

        public String getName() { return this.Name; }
        public int getReferenceNumber() { return this.ReferenceNumber; }
        public void enqueueMessage(NetMessage newmessage) { UnreadMessages.Enqueue(newmessage); }
        public NetMessage getLatestMessage()
        {
            if (UnreadMessages.Count == 0) return null;
            //dequeue messages until the latest one is found
            NetMessage latestmessage = null;
            while (UnreadMessages.Count > 0)
            {
                latestmessage = UnreadMessages.Dequeue();
            }
            return latestmessage;
        }
    }
}
