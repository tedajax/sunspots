using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;

namespace StarForce_PendingTitle_
{
    public class EnemyManager
    {
        private static Dictionary<Int16, Enemy> EnemyList; //its not really a list, but you'll just have to live with that
        private int EnemyCount;
        public static Dictionary<Int16,Enemy> OnlineEnemiesHit; //Keeps Track of All Enemies Hit so we can send them online
        public static Queue<NetMessage> MessagesRecieved;
        TimeSpan timetillnextupdate = new TimeSpan(0, 0, 0, 0, 200);
        TimeSpan elapsedTime = new TimeSpan(0, 0, 0, 0, 200);

        public static Int16 EnemyId = -32768;
        //if you are creating more than 32768*2 enemies in one level, you have problems, please dont talk to me

        public enum EnemyAI
        {
            Death,
            Defensive,
            Loopy
        }

        public EnemyManager()
        {
            
            EnemyList = new Dictionary<short, Enemy>();
            OnlineEnemiesHit = new Dictionary<short, Enemy>();
            MessagesRecieved = new Queue<NetMessage>();
            EnemyCount = 0;
        }

        public void AddEnemy(Enemy newenemy)
        {            
            EnemyList.Add(EnemyId, newenemy);
            EnemyId++;
            EnemyCount++;
        }

        public Dictionary<short,Enemy> GetEnemyList() { return EnemyList; }
        public int GetEnemyCount() { return EnemyList.Count; }

        

        private void HandleOnline(GameTime gameTime)
        {
            elapsedTime -= gameTime.ElapsedGameTime;
            if (elapsedTime.TotalMilliseconds < 0)
            {
                elapsedTime = timetillnextupdate;
                if (OnlineEnemiesHit.Count > 0)
                {
                   
                    NetMessage EnemyMessage = new NetMessage();
                    //We need the identifiers. 7 to specify its gameplay data, 2 to specify its enemy data
                    EnemyMessage.Write((byte)7);
                    EnemyMessage.Write((byte)2);
                    //Now we need to tell it how many enemy keys are in this package
                    EnemyMessage.Write(OnlineEnemiesHit.Count);
                    foreach(Enemy E in OnlineEnemiesHit.Values)
                    {
                        EnemyMessage.Write(E.getKey());
                        EnemyMessage.Write(E.EnemyHealth);
                    }
                    OnlineEnemiesHit = new Dictionary<short, Enemy>(); //renew the list
                    NetClientClass.sendMessageReliable(EnemyMessage);
                }
            }
            while (MessagesRecieved.Count > 0)
            {
                NetMessage EnemyMessage = MessagesRecieved.Dequeue();
                int Number = EnemyMessage.ReadInt();
                for (int i = 0; i < Number; i++)
                {
                    short Died = EnemyMessage.ReadInt16();
                    Enemy E;
                    if (EnemyList.TryGetValue(Died, out E))
                    {
                        E.EnemyHealth = EnemyMessage.ReadFloat();
                        E.BeginShake();
                    }
                    else
                    {
                        //if the enemy doesnt exist anymore, just read the float anyway so it doesnt interfere
                        EnemyMessage.ReadFloat();
                    }
                }
            }

        }

        /// <summary>
        /// Find all the enemies with the "KillThis" bool set to true and remove them from the manager
        /// </summary>
        public void CleanUpEnemies()
        {
            List<Enemy> EnemiesToRemove = new List<Enemy>(); 
            foreach (Enemy enemy in EnemyList.Values)
            {
                if (enemy.RemoveThis)
                {
                    EnemiesToRemove.Add(enemy);
                }
            }


            for (int i = 0; i < EnemiesToRemove.Count; i++)
            {
                
                Enemy Enemy = EnemiesToRemove[i];
                EnemyList.Remove(Enemy.getKey());
                
                int numOfParticles = 0;
                if (Specs.ParticleEffects == Specs.Detail.High)
                    numOfParticles = 20;
                else if (Specs.ParticleEffects == Specs.Detail.Medium)
                    numOfParticles = 10;
                else
                    numOfParticles = 0;

                for (int explosion = 0; explosion < numOfParticles; explosion++)
                {
                    WindowManager.ExplosionParticles.AddParticle(Enemy.GetPosition(),Enemy.Advance * 2);
                    WindowManager.ExplosionSmokeParticles.AddParticle(Enemy.GetPosition(), Enemy.Advance * 2);
                }

                EnemyCount--;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
            {
                HandleOnline(gameTime);
            }
            CleanUpEnemies();
            foreach (Enemy e in EnemyList.Values)
            {
                e.Update(gameTime);
            }
        }

        public void Draw(string technique)
        {
            foreach (Enemy e in EnemyList.Values)
            {
                e.Draw(technique);
            }
        }

        public void ConvertEnemy(Int16 EnemyKey, EnemyAI NewAI)
        {
            Enemy Ene;
            if (EnemyList.TryGetValue(EnemyKey, out Ene))
            {
                EnemyList.Remove(EnemyKey);

                switch (NewAI)
                {
                    case EnemyAI.Death:
                        Ene = new Defensive(Ene);
                        break;
                    case EnemyAI.Defensive:
                        Ene = new Defensive(Ene);
                        break;
                    case EnemyAI.Loopy:
                        Ene = new LoopyEnemy(Ene);
                        break;
                }

                EnemyList.Add(EnemyKey, Ene);
            }
        }
    }
}
