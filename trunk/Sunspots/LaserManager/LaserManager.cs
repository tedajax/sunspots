using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    public class LaserManager
    {
        private static List<Laser> LaserList;
        private Dictionary<Int16, Laser> LasersFromOnline; //this Dictionary is just there so we can handle lasers recived online seperatly
        private static int LaserCount;
        private static Queue<Laser> LasersOnQueue; //this queue is all the lasers that were created on local machine
        public static Queue<Laser> LasersDestroyed; //this queue is all the lasers that were destroyed on the local machine
        public static Queue<NetMessage> MessagesRecieved; //this queue contains messages sent from online
        public static Int16 IdValue = -32768; //this ID value allows us to assign each laser and ID, so that when sent online they are unique

        private TimeSpan OnlineWaitTime = new TimeSpan(0, 0, 0, 0, 150);
        private TimeSpan TimeWaited = new TimeSpan(0, 0, 0, 0, 150);


        public static Model LaserModel;
        public LaserManager(Model LaserMod)
        {
            LaserList = new List<Laser>();
            LasersOnQueue = new Queue<Laser>();
            MessagesRecieved = new Queue<NetMessage>();
            LasersFromOnline = new Dictionary<short, Laser>();
            LasersDestroyed = new Queue<Laser>();
            LaserCount = 0;
            LaserModel = LaserMod;
        }

        public static void AddLaser(Laser newlaser)
        {
            LaserList.Add(newlaser);
            IdValue++; //Increment the ID Value
            LaserCount = LaserList.Count;
            
            if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
            {

                LasersOnQueue.Enqueue(newlaser);
            }

        }

        private void HandleOnline(GameTime gameTime)
        {
            while (MessagesRecieved.Count > 0)
            {
                //lets read how many lasers we need to dequeue
                NetMessage newlaser = MessagesRecieved.Dequeue();
                //lets find out what type of message this is
                byte type = newlaser.ReadByte();
                if (type == 1) //this is laser creation data
                {
                    //lets dequeue each laser recieved from online and create a laser for it
                    byte number = newlaser.ReadByte();
                    for (int i = 0; i < number; i++)
                    {
                        Int16 Id = newlaser.ReadInt16();
                        Vector3 Position = XnaSerialization.ReadVector3(newlaser);
                        Vector3 Rotation = XnaSerialization.ReadVector3(newlaser);
                        Matrix Matrix = XnaSerialization.ReadMatrix(newlaser);
                        float damage = newlaser.ReadFloat();
                        Laser createlaser = new Laser(Position, Rotation, Matrix, damage, Laser.Source.Player, LaserModel, Id);
                        this.AddLaserFromOnline(createlaser);
                    }
                }
                if (type == 2) //this is laser destruction data
                {
                    byte number = newlaser.ReadByte();
                    for (int i = 0; i < number; i++)
                    {
                        Laser Trylaser;
                        Int16 Id = newlaser.ReadInt16();
                        if (LasersFromOnline.TryGetValue(Id, out Trylaser))
                        {
                            LasersFromOnline.Remove(Id); //we found the laser that was sent online, kill it
                        }
                    }
                }
            }
            TimeWaited -= gameTime.ElapsedGameTime;
            if (TimeWaited.TotalMilliseconds <=0)
            {
                TimeWaited += OnlineWaitTime;
                if (LasersOnQueue.Count > 0)
                {
                    NetMessage LaserMessage = new NetMessage();

                    //We need the identifiers. 7 to specify its gameplay data, 1 to specify its laser data
                    LaserMessage.Write((byte)7);
                    LaserMessage.Write((byte)1);
                    //Now we need another 1 to specify its Laser Creation Data
                    LaserMessage.Write((byte)1);
                    //Now we need to tell it how many lasers are in this package
                    LaserMessage.Write((byte)LasersOnQueue.Count);
                    while (LasersOnQueue.Count > 0)
                    {
                        Laser newlaser = LasersOnQueue.Dequeue();
                        LaserMessage.Write(newlaser.getId());
                        XnaSerialization.Write(LaserMessage, newlaser.Position);
                        XnaSerialization.Write(LaserMessage, newlaser.Rotation);
                        XnaSerialization.WriteMatrix(LaserMessage, newlaser.MovementTransformMatrix);
                        LaserMessage.Write(newlaser.Damage);
                    }

                    //lets send all of our messages
                    NetClientClass.sendMessage(LaserMessage);
                }
                if (LasersDestroyed.Count > 0)
                {
                    NetMessage LaserMessage = new NetMessage();
                    //We need the identifiers. 7 to specify its gameplay data, 1 to specify its laser data
                    LaserMessage.Write((byte)7);
                    LaserMessage.Write((byte)1);
                    //Now we need 2 to specify its laser destruction data
                    LaserMessage.Write((byte)2);
                    //Now we need to tell it how many lasers are in this package
                    LaserMessage.Write((byte)LasersDestroyed.Count);
                    while (LasersDestroyed.Count > 0)
                    {
                        Laser newLaser = LasersDestroyed.Dequeue();
                        //we write the lasers ID so that other computers can destory this laser on their local machines
                        LaserMessage.Write(newLaser.getId());
                    }
                    NetClientClass.sendMessage(LaserMessage);
                }
            }
        }

        public string DrawText()
        {
            return TimeWaited.ToString();
        }

        public void AddLaserFromOnline(Laser newlaser)
        {
            newlaser.IsCollidable = false;
            //register the laser with the online list so we can figure out when it blows up.
            LasersFromOnline.Add(newlaser.getId(), newlaser);
           // LaserList.Add(newlaser);
           // LaserCount = LaserList.Count;
        }

        public List<Laser> GetLaserList() { return LaserList; }
        public int GetLaserCount() { return LaserCount; }

        /// <summary>
        /// Find all the enemies with the "KillThis" bool set to true and remove them from the manager
        /// </summary>
        private void CleanUpLasers()
        {
            for (int i = 0; i < LaserCount; i++)
            {
                if (LaserList[i].KillThis)
                {
                    //Add the laser to the lasersDestoryed list so it can be transmitted online
                    LasersDestroyed.Enqueue(LaserList[i]);
                    LaserList.RemoveAt(i);
                    LaserCount--;
                }
            }   
        }

        public void Update(GameTime gameTime)
        {
            if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
            {
                HandleOnline(gameTime);
            }
            else
            {
                //Keep these lists cleared so they dont cause any problems
                LasersDestroyed = new Queue<Laser>();
                LasersOnQueue = new Queue<Laser>();
            }
            CleanUpLasers();
            foreach (Laser l in LaserList)
            {
                l.Update(gameTime);
            }
            foreach (Laser L in LasersFromOnline.Values)
            {
                L.Update(gameTime);
            }
        }

        public void Draw(string technique)
        {
            foreach (Laser l in LaserList)
            {
                l.Draw(technique);
            }
            foreach (Laser l in LasersFromOnline.Values)
            {
                l.Draw(technique);
            }
        }
    }
}
