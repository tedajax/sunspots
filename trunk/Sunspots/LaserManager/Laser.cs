using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class Laser : CollisionObject
    {
        protected Vector3 position;
        protected Vector3 rotation;
        protected float laserspeed;
        protected float damage;
        protected Source source;
        protected Matrix movementTransformMatrix;
        protected Int16 laserId; //this is only for online play

        //Determines if the laser manager should kill the laser in the cleanup method
        public bool KillThis = false;

        //for making sure everything stays in sync with the game timer
        private float elapsedgametimemultiplier;

        private Obj3d LaserObj;

        PointSpriteParticles LaserParticles;

        protected TimeSpan laserlife;

        OBB[] OBBs = new OBB[1];

        private bool isCollidable;

         public bool IsCollidable
        {
            get { return isCollidable; }
            internal set { this.isCollidable = value; }
        }

        public Matrix MovementTransformMatrix
        {
            get { return movementTransformMatrix; }
            internal set { movementTransformMatrix = value; }
        }

        public enum Source
        {
            Player,
            Enemy,
            Turret,
            UnKnown
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

        public float Damage
        {
            get { return damage; }
            internal set { damage = value; }
        }

        public Source LaserSource
        {
            get { return source; }
            internal set 
            { 
                source = value;
            }
        }

        public float LaserSpeed
        {
            get { return laserspeed; }
            internal set { laserspeed = value; }
        }

        public TimeSpan LaserLife
        {
            get { return laserlife; }
            internal set { laserlife = value; }
        }

        public Laser(Vector3 pos, Vector3 rot, Matrix Matrix, float dmg, Source s, Model lasermodel, Int16 ID)
        {
            Position = pos;
            movementTransformMatrix = Matrix;
            Rotation = rot;
            Damage = dmg;
            source = s;
            laserspeed = 80f;
            isCollidable = true;
            this.laserId = ID; //this is only for online play do not worry about it for offline play

            LaserObj = new Obj3d(lasermodel);
            LaserObj.setScale(5);

            laserlife = new TimeSpan(0, 0, 1);

            OBBs[0] = new OBB(this.position, new Vector3(5, 5, 10)*LaserObj.getScale());
            this.Init(OBBs);

            InitParticles();
        }

        private void InitParticles()
        {
            LaserParticles = new PointSpriteParticles(Game1.Graphics,
                                                      "Content\\Particle",
                                                      "Content\\Effects\\Particle",
                                                      2
                                                     );
            LaserParticles.Initialize();
            LaserParticles.SetSystemToBall(0.5f);
            LaserParticles.RandomColor = false;

            switch (LaserSource)
            {
                case Source.Enemy:
                    LaserParticles.particleColor = Color.Salmon;
                    break;
                case Source.Player:
                    LaserParticles.particleColor = Color.CornflowerBlue;
                    break;
                case Source.Turret:
                    LaserParticles.particleColor = Color.YellowGreen;
                    break;
                case Source.UnKnown:
                    LaserParticles.RandomColor = true;
                    break;
            }

            LaserParticles.RefreshParticles();
            
            //LaserParticles.VaryColor = true;
        }

        public void Update(GameTime gameTime)
        {
            //Update the gametime multiplier
            elapsedgametimemultiplier = gameTime.ElapsedGameTime.Milliseconds / (1000 / Game1.FPS);

            //Lasers are simple to update, they just advance forward based on their rotation
            Vector3 AdvanceVector = Vector3.Forward; //Because the model wants to move forward, duh!
            AdvanceVector = Vector3.Transform(AdvanceVector, movementTransformMatrix);
            AdvanceVector = Vector3.Normalize(AdvanceVector);
            Position += AdvanceVector * LaserSpeed * elapsedgametimemultiplier;

            LaserObj.setPosition(Position);

            LaserParticles.myPosition = Position;

            LaserObj.setRotation(Vector3.Transform(AdvanceVector, movementTransformMatrix));

            OBBs[0] = new OBB(this.position, new Vector3(5, 5, 10));
            this.setCollisionBoxes(OBBs);

            LaserLife -= gameTime.ElapsedGameTime;

            LaserParticles.Update(gameTime);

            if (LaserLife.TotalMilliseconds <= 0) KillThis = true;
        }

        public void Draw(string technique)
        {
            Matrix OldMatrix = movementTransformMatrix;
            movementTransformMatrix *= Matrix.CreateRotationY(MathHelper.Pi);
            LaserParticles.Draw();
            //LaserObj.DisplayModel(CameraClass.getLookAt(), technique, movementTransformMatrix);
            movementTransformMatrix = OldMatrix;
        }

        public Int16 getId() { return this.laserId; }
    }
}
