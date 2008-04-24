using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class Missle : CollisionObject
    {
        protected byte stage = 0;
        private TimeSpan StageLife = new TimeSpan();
        private TimeSpan Stage1Life = new TimeSpan(0, 0, 0, 0, 300);
        private TimeSpan Stage2Life = new TimeSpan(0, 0, 3);
        private TimeSpan Stage3Life = new TimeSpan(0, 0, 2);
        protected Vector3 position = new Vector3();
        protected Vector3 rotation = new Vector3();
        protected Vector3 target = new Vector3();
        protected bool isTargeted = false;
        protected Matrix rotMat = new Matrix();

        private OBB[] colboxes = new OBB[1];

        public byte Stage
        {
            get { return stage; }
        }

        public Vector3 Position
        {
            get { return position; }
            internal set { position = value; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            internal set { rotation = value; }
        }

        public Vector3 TargetVector
        {
            get { return target; }
            internal set
            { 
                target = value;
                isTargeted = true;
            }
        }

        public bool IsTargeted
        {
            get { return isTargeted; }
        }

        public Matrix RotMat
        {
            get { return rotMat; }
            internal set { rotMat = value; }
        }

        Obj3d MissleObj;

        private float MissleDropStartSpeed;
        private float MissleDropMaxSpeed;
        private float MissleDropInterpolation;
        private float MissleDropSpeed;
        private float MissleForwardStartSpeed;
        private float MissleForwardMaxSpeed;
        private float MissleSpeedInterpolation;
        private float MissleForwardSpeed;

        float elapsedmultiplier;

        Random Randomizer = new Random();

        protected Matrix TargettingWorldMatrix;

        public Matrix TargetWorld
        {
            get { return TargettingWorldMatrix; }
        }

        public Missle(Model MissleModel)
        {
            

            MissleObj = new Obj3d(MissleModel);
            MissleObj.setScale(15);

            MissleDropStartSpeed = 1f;
            MissleDropMaxSpeed = 4f;
            MissleDropInterpolation = 0.9f;
            MissleDropSpeed = MissleDropStartSpeed;
            MissleForwardStartSpeed = 10f;
            MissleForwardMaxSpeed = 20;
            MissleSpeedInterpolation = 0.9f;
            MissleForwardSpeed = MissleForwardStartSpeed;

            colboxes[0] = new OBB(this.position, new Vector3(1, 1, 2.3f));
            this.Init(colboxes);
        }

        public void FireMissle(Vector3 pos, Vector3 rot, Matrix rotmat)
        {
            if (stage == 0)
            {
                position = pos;
                rotation = rot;
                rotMat = rotmat;

                colboxes[0] = new OBB(this.position, new Vector3(1, 1, 2.3f));
                this.setCollisionBoxes(colboxes);

                stage = 1;
                StageLife = Stage1Life;
            }
            else if (stage == 2)
            {
                if (StageLife.TotalMilliseconds < 1500)
                    Explode();
            }
        }

        private void Advance()
        {
            Vector3 AdvanceVector = new Vector3();
           switch (stage)
            {
                case 1:
                    AdvanceVector = new Vector3(0, -1, -1);
                    break;
                case 2:
                    if (isTargeted)
                    {
                        AdvanceVector = this.TargetVector - this.Position;
                        AdvanceVector = Vector3.Normalize(AdvanceVector);
                        //We need to calculate a Matrix that tells us which direction to point at
                    }
                    else
                        AdvanceVector = Vector3.Forward;
                    break;
                default:
                    AdvanceVector = Vector3.Zero; //Just in case
                    break;
            }
            if (IsTargeted == false)
            {

                AdvanceVector = Vector3.Transform(AdvanceVector, rotMat);
                AdvanceVector = Vector3.Normalize(AdvanceVector);
            }

            Vector3 Velocity = new Vector3();

            switch (stage)
            {
                case 1:
                    Velocity = AdvanceVector * MissleDropSpeed * elapsedmultiplier;
                    MissleDropSpeed = MathHelper.Lerp(MissleDropSpeed, MissleDropMaxSpeed, MissleDropInterpolation);
                    break;
                case 2:
                    Velocity = AdvanceVector * MissleForwardSpeed * elapsedmultiplier;
                    MissleForwardSpeed = MathHelper.Lerp(MissleForwardSpeed, MissleForwardMaxSpeed, MissleSpeedInterpolation);
                    break;
                default:
                    Velocity = Vector3.Zero;
                    MissleForwardSpeed = MissleForwardStartSpeed;
                    MissleDropSpeed = MissleDropStartSpeed;
                    break;
            }
            if (((Vector3)(position - target)).Length() > 5)
            {
                position += Velocity;
            }
            TargettingWorldMatrix = Matrix.CreateScale(MissleObj.getScale())* Matrix.CreateTranslation(this.position);

            TargettingWorldMatrix.Forward = this.target - TargettingWorldMatrix.Translation;
            
            TargettingWorldMatrix.Forward = Vector3.Normalize(TargettingWorldMatrix.Forward);
            TargettingWorldMatrix.Forward *= MissleObj.getScale();
            TargettingWorldMatrix.Right = Vector3.Cross(TargettingWorldMatrix.Forward, Vector3.Up);
            TargettingWorldMatrix.Right = Vector3.Normalize(TargettingWorldMatrix.Right);
            TargettingWorldMatrix.Right *= MissleObj.getScale();
            TargettingWorldMatrix.Up = Vector3.Cross(TargettingWorldMatrix.Forward, TargettingWorldMatrix.Right);
            TargettingWorldMatrix.Up = Vector3.Normalize(TargettingWorldMatrix.Up);
            TargettingWorldMatrix.Up *= MissleObj.getScale();
            //TargettingWorldMatrix *= Matrix.CreateScale(MissleObj.getScale());*/
        }

        public void Update(GameTime gameTime)
        {
            elapsedmultiplier = gameTime.ElapsedGameTime.Milliseconds / (1000 / Game1.FPS);

            Advance();

            //Update the stage
            switch (stage)
            {
                case 1:
                    if (StageLife.TotalMilliseconds <= 0) { stage++; StageLife = Stage2Life; }
                    break;
                case 2:
                    if (StageLife.TotalMilliseconds <= 0 && !isTargeted)
                    {
                        Explode();
                    }
                    break;
                case 3:
                    if (StageLife.TotalMilliseconds <= 0)
                    {
                        isTargeted = false;
                        target = Vector3.Zero;
                        stage = 0;
                    }
                    break;
            }
            
            if (stage == 1 || stage == 2)
            {
                WindowManager.SmokeTrailParticles.AddParticle(this.position, Vector3.Zero);
            }

            StageLife -= gameTime.ElapsedGameTime;

            MissleObj.setPosition(position);
            MissleObj.setRotation(rotation);
            colboxes[0].Center = position;
            colboxes[0].Rotation = rotMat;
            this.setCollisionBoxes(colboxes);
        }

        public void Explode()
        {
            stage++; StageLife = Stage3Life;
            int numofparticles = 0;

            int explodeBox = 400;
            colboxes[0] = new OBB(this.Position, new Vector3(explodeBox, explodeBox, explodeBox), rotMat);
            this.setCollisionBoxes(colboxes);

            //Set the number of particles based on specifications settings
            if (Specs.ParticleEffects == Specs.Detail.High) numofparticles = 20;
            else if (Specs.ParticleEffects == Specs.Detail.Medium) numofparticles = 10;
            else numofparticles = 5;

            for (int explode = 0; explode < numofparticles; explode++)
            {
                Vector3 partpos = this.position;
                int partspacing = 200;
                partpos.X += Randomizer.Next(-partspacing, partspacing);
                partpos.Y += Randomizer.Next(-partspacing, partspacing);
                partpos.Z += Randomizer.Next(-partspacing, partspacing);
                WindowManager.ExplosionParticles.AddParticle(partpos, Vector3.Zero);
                WindowManager.ExplosionSmokeParticles.AddParticle(partpos, Vector3.Zero);
            }
        }

        public void Draw(string technique)
        {
            if (stage == 1 || stage == 2)
            {
                if (IsTargeted)
                {
                    MissleObj.DisplayModelWorldMatrix(TargettingWorldMatrix, technique);
                }
                else
                {
                    MissleObj.DisplayModel(CameraClass.getLookAt(), technique, rotMat);
                }
            }
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(MissleObj.getScale())
                   * RotMat
                   * Matrix.CreateTranslation(Position);
        }
    }
}
