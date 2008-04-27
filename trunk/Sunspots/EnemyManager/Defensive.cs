using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    class Defensive : Enemy
    {
        public Defensive(Model enemymodel, Vector3 position, Vector3 rotation, OBB Trigger, short KeyVal)
        {
            EnemyModel = enemymodel;
            Position = position;
            Rotation = rotation;
            this.Key = KeyVal;
            this.scoreValue = 10f;
            this.TriggerOBB = Trigger;
            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            MovementSphere.Center = Position;
            MovementSphere.Radius = 1500;

            AwarenessSphere.Center = Position;
            AwarenessSphere.Radius = 200;
        }

        public Defensive(Enemy enemy)
        {
            EnemyModel = enemy.GetEnemyModel();
            Position = enemy.GetPosition();
            Rotation = enemy.GetRotation();
            this.Key = enemy.getKey();
            Health = enemy.GetHealth();

            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            MovementSphere.Center = Position;
            MovementSphere.Radius = 1500;

            AwarenessSphere.Center = Position;
            AwarenessSphere.Radius = 200;
        }

        public override void  Update(GameTime gameTime, MainShip PlayerShip)
        {
            base.Update(gameTime, PlayerShip);

            if (!KillThis)
            {
                //Reposition the awareness sphere 
                AwarenessSphere.Center = Position;

                //If the Enemy leaves their movement sphere it should turn around
                if (Vector3.Distance(Position, MovementSphere.Center) > MovementSphere.Radius)
                {
                    if (TurnAmount == 0)
                        TurnAmount = MathHelper.Pi;
                }
                else
                {
                    TurnAmount = 0;
                }

                if (TurnAmount != 0)
                {
                    TurnSpeed = MathHelper.Pi / 50;

                    if (TurnAmount > 0)
                    {
                        TurnRight = true;
                        TurnAmount -= TURN_SPEED;
                    }
                    else
                    {
                        TurnLeft = true;
                        TurnAmount += TURN_SPEED;
                    }
                }
                else
                {
                    TurnSpeed = MathHelper.Pi / 450f;

                    TurnRight = false;
                    TurnLeft = false;
                }
            }

            base.UpdateLocations(gameTime);
        }
    }
}
