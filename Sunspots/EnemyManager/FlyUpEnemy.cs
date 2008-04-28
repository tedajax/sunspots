using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    class FlyUpEnemy : Enemy
    {
        //float OffsetDistance = 50f;
        //Vector3 ToTheLeftToTheLeft;
        //Vector3 ToTheRightToTheRight;
        //bool moveLeft = false;
        Vector3 CenterPos;
        Matrix Lockon;
        TimeSpan LifeSpan;
        float YDeviation;
        public FlyUpEnemy(Model enemymodel, Vector3 position, Vector3 rotation, OBB Trigger, short KeyVal, Random Randomizer)
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
            Health = 50f;

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            if (Randomizer.Next(0, 1) == 0)
            {
                //moveLeft = true;
            }
            CenterPos = this.Position;
            YDeviation = Randomizer.Next(-150, 150);
            LifeSpan = new TimeSpan(0, 0, 0, 3,500+ Randomizer.Next(0,1000));
           
        }

        public Matrix CreateLockOn(Vector3 Target, Vector3 Position)
        {
            Matrix NewMatrix;
            NewMatrix = Matrix.Identity;
            NewMatrix.Forward = Target - Position;
            NewMatrix.Forward = Vector3.Normalize(NewMatrix.Forward);
            NewMatrix.Right = Vector3.Cross(NewMatrix.Forward, Vector3.Up);
            NewMatrix.Right = Vector3.Normalize(NewMatrix.Right);
            NewMatrix.Up = Vector3.Cross(NewMatrix.Right, NewMatrix.Forward);
            NewMatrix.Up = Vector3.Normalize(NewMatrix.Up);

            return NewMatrix;

        }


        public override void Update(GameTime gameTime, MainShip Playership)
        {
            LifeSpan -= gameTime.ElapsedGameTime;
            if (LifeSpan.TotalSeconds < 0)
            {
                Vector3 TargettingPosition = new Vector3(1150, 1200, -1150);
                TargettingPosition = Vector3.Transform(TargettingPosition, Playership.getNonMovementPositionRotationMatrix());

                Lockon = CreateLockOn(TargettingPosition, this.Position);
                this.Position = Vector3.Lerp(this.Position, TargettingPosition, .03f);
                if (LifeSpan.TotalSeconds < -2)
                {
                    KillThis = true;
                    this.RemoveThis = true;
                }

            }
            else
            {

                Vector3 OldPosition = this.Position;
                base.Update(gameTime, Playership);
                this.Position = OldPosition;
                this.Position.Y = MathHelper.Lerp(this.Position.Y, Playership.getSecondaryPosition().Y + YDeviation, .15f);

                Lockon = CreateLockOn(Playership.Position, this.Position);
            }
            base.UpdateLocations(gameTime);
        }

        public override void Draw(string technique)
        {
            EnemyObj.DisplayModel(CameraClass.getLookAt(), technique, Lockon);
        }
    }
}
