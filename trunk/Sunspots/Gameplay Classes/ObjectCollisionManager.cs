using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class ObjectCollisionManager
    {
        protected EnemyManager enemymngr;
        protected LaserManager lasermngr;
        protected List<LocalPlayer> LocalPlayers = new List<LocalPlayer>();
        //protected List<LocalPlayer> players; //Implement later

        Boundries bounds;

        public EnemyManager EnemyMngr
        {
            get { return enemymngr; }
            internal set { enemymngr = value; }
        }

        public LaserManager LaserMngr
        {
            get { return lasermngr; }
            internal set { lasermngr = value; }
        }

        public void AddLocalPlayer(LocalPlayer lp)
        {
            LocalPlayers.Add(lp);
        }

        public void AddBoundaryBoxes(Boundries boxes)
        {
            bounds = boxes;
        }

        public ObjectCollisionManager()
        {

        }

        Vector3 MissleTarget = new Vector3();
        int TargetedEnemy = -1;

        /// <summary>
        /// This is where all object collisions will go
        /// </summary>
        public void CheckObjectCollisions()
        {
            if (bounds != null && 1==2)
            {
                if (!bounds.getCollisionData().checkCollisionSecret(LocalPlayers[0].MainShip.getCollisionData()))
                {
                    if (!LocalPlayers[0].MainShip.IsLooping())
                    {
                        LocalPlayers[0].MainShip.StartLoopOverride("Reverse");
                    }
                }
            }

            /*
             * Check lasers against enemies
             * if the laser manager and enemy managers are not null
             */

            if (this.lasermngr != null && this.enemymngr != null)
            {
                foreach (Laser laser in this.lasermngr.GetLaserList())
                {
                    foreach (Enemy enemy in this.enemymngr.GetEnemyList().Values)
                    {
                        if (laser.IsCollidable && laser.LaserSource == Laser.Source.Player && laser.getCollisionData().checkCollision(enemy.getCollisionData()))
                        {
                            //get rid of the laser and hit the enemy
                            laser.KillThis = true;
                            enemy.EnemyHealth -= laser.Damage;
                            enemy.BeginShake();

                            if (enemy.KillThis)
                            {
                                LocalPlayers[0].KilliedEnemy(enemy.getScoreValue());
                            }

                            //we need to register this enemy with the online component
                            EnemyManager.OnlineEnemiesHit[enemy.getKey()] = enemy; //this will add or override

                            //Remove the enemy if falling and hit again
                            if (enemy.KillThis)
                                enemy.RemoveThis = true;

                            //kinda stupid but whatever
                            WindowManager.ExplosionParticles.AddParticle(laser.Position, enemy.Advance);
                        }
                    }
                }
            }
               /*
            if (this.LocalPlayers != null && this.enemymngr != null)
            {
                foreach (LocalPlayer lp in LocalPlayers)
                {
                    OBB[] boxlist = new OBB[3];
                    for (int i = 0; i < 3; i++)
                        boxlist[i] = lp.MainShip.getCollisionData().getCollisionBoxes()[i];
                    CollisionData ExcludeRaycast = new CollisionData(boxlist);
                    ParticleSystem.Targeted = false;
                    int enecount = 0;
                    foreach (Enemy enemy in enemymngr.GetEnemyList().Values)
                    {
                        if (ExcludeRaycast.checkCollision(enemy.getCollisionData()) && lp.MainShip.shakeTimer.TotalMilliseconds <= 0)
                        {
                            lp.MainShip.BeginShake();
                            enemy.BeginShake();

                            lp.MainShip.Health -= 20f;
                            enemy.EnemyHealth -= 50f;
                        }

                        if (!Playing.PlayerMissile.IsTargeted)
                        {
                            MissleTarget = Vector3.Zero;
                            OBB enemybox = enemy.getCollisionData().getCollisionBoxes()[0];
                            
                            if (lp.MainShip.getCollisionData().getCollisionBoxes()[3].Intersects(enemybox))
                            {
                                MissleTarget = enemy.GetPosition();
                                TargetedEnemy = enecount;
                                ParticleSystem.Targeted = true;
                                lp.TargetingPosition = MissleTarget;
                                break;
                            }                            
                        }
                        else
                        {
                            if (Playing.PlayerMissile.Stage == 2)
                            {
                                if (enecount == TargetedEnemy)
                                    Playing.PlayerMissile.TargetVector = enemy.GetPosition();
                            }
                        }

                        if (Playing.PlayerMissile.Stage == 0)
                        {
                            TargetedEnemy = -1;
                            lp.TargetingPosition = Vector3.Zero;
                        }

                        if (Playing.PlayerMissile.getCollisionData().checkCollision(enemy.getCollisionData()))
                        {
                            if (Playing.PlayerMissile.Stage == 2)
                                Playing.PlayerMissile.Explode();

                            if (Playing.PlayerMissile.Stage == 3)
                            {
                                if (!enemy.IsShaking)
                                {
                                    float enemydamage = 150 - (Vector3.Distance(enemy.GetPosition(), Playing.PlayerMissile.Position) / 10);
                                    enemydamage = MathHelper.Max(enemydamage, 0);
                                    if (enemydamage < 100)
                                        enemy.BeginShake();
                                    enemy.EnemyHealth -= enemydamage;
                                    if (enemy.KillThis) enemy.RemoveThis = true;
                                }
                            }
                        }

                        enecount++;
                    }
                
            
                    
                    if (enecount == 0 && Playing.PlayerMissile.Stage == 0)
                    {
                        TargetedEnemy = -1;
                        lp.TargetingPosition = Vector3.Zero;
                    }
                }
            }*/
        }
    }
}
