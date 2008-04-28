using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    class Annoying : Enemy
    {
        Matrix RotationMatrix;

        TimeSpan ChangePositionTimer;
        int TimeChangeSeconds = 5;

        Random TargetRandom = new Random();

        public Annoying(Model enemymodel, Vector3 position, Vector3 rotation, OBB Trigger, short KeyVal)
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

            ChangePositionTimer = new TimeSpan();
        }

        public override void Update(GameTime gameTime, MainShip PlayerShip)
        {
            base.Update(gameTime, PlayerShip);

            if (!KillThis)
            {
                ChangePositionTimer += gameTime.ElapsedGameTime;

                if (ChangePositionTimer.TotalSeconds > TimeChangeSeconds)
                {
                    ChangePositionTimer = new TimeSpan();

                    TargetPosition.X = TargetRandom.Next(-30, 30);
                    TargetPosition.Y = TargetRandom.Next(-30, 30);
                }

                Vector3 InFront = Vector3.Forward * 50;
                InFront.X = TargetPosition.X;
                InFront.Y = TargetPosition.Y;
                Matrix rot = Matrix.CreateFromQuaternion(PlayerShip.NewRotation);
                Position = Vector3.Transform(InFront, rot) + PlayerShip.Position;

                RotationMatrix = rot;
            }

            base.UpdateLocations(gameTime);
        }

        public override void Draw(string technique)
        {
            EnemyObj.DisplayModel(CameraClass.getLookAt(), technique, RotationMatrix);
        }
    }
}
