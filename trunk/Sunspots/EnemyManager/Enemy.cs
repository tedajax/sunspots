using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class Enemy : CollisionObject
    {
        protected Vector3 Position;
        protected Vector3 Rotation;
        //Additional rotation for stuff
        protected Vector3 RotationOffset = Vector3.Zero;
        protected Model EnemyModel;
        protected Obj3d EnemyObj;

        //This key is the refrence ID for the enemy. Each Enemy MUST have one
        protected short Key;

        protected TimeSpan ShakeTimer = new TimeSpan();
        protected bool isShaking = false;
        protected Random Randomizer = new Random();

        protected float Health = 100f;

        protected double scoreValue = 5f;

        //Will be used in some cases such as routine identifier 0
        protected Vector3 TargetPosition;

        //Temporary, will be replaced by AI routine class
        //0 will just be circling around a target position
        protected int RoutineIdentifier = 2;

        //Determines how the enemy should move
        protected bool TurnLeft = false;
        protected bool TurnRight = false;
        protected bool PitchUp = false;
        protected bool PitchDown = false;
        protected bool RollLeft = false;
        protected bool RollRight = false;

        protected TimeSpan TurnTimer = new TimeSpan();
        protected TimeSpan PitchTimer = new TimeSpan();
        protected TimeSpan RollTimer = new TimeSpan();

        protected float TurnAmount = 0f;
        protected float PitchAmount = 0f;
        protected float RollAmount = 0f;

        protected float OldTurn = 0f;
        protected float OldPitch = 0f;
        protected float OldRoll = 0f;
                
        //Enemy speed multiplier
        protected float EnemySpeed = 6f;
        //Rotation modifiers
        //All values should be set in radians (or at least converted using MathHelper.ToRadians(float degrees)
        protected float TurnSpeed = MathHelper.Pi / 450; //Nice slow default value
        protected float PitchSpeed = MathHelper.Pi / 450;
        protected float RollSpeed = MathHelper.Pi / 450;

        //Boolean for telling the rest of the update if the enemy was hit
        //Mostly used in AIRoutines
        protected bool EnemyHit = false;

        //Tells the enemy manager if the enemy is killed
        public bool KillThis = false;
        //Tell the enemy to remove a killed enemy
        public bool RemoveThis = false;
        private TimeSpan KillTimer = new TimeSpan(0, 0, 3);

        //Collision Boxes
        protected OBB[] colboxes = new OBB[1];

        protected float elapsedgamemultiplier;

        protected BoundingSphere AwarenessSphere = new BoundingSphere();
        protected BoundingSphere MovementSphere = new BoundingSphere();

        protected Vector3 advance;

        public float GetHealth() { return Health; }

        #region Accessors
        public Vector3 GetPosition() { return Position; }
        public Vector3 GetRotation() { return Rotation; }
        public Model GetEnemyModel() { return EnemyModel; }
        public int GetRoutineIdentifier() { return RoutineIdentifier; }
        public float GetEnemySpeedMultiplier() { return EnemySpeed; }
        public Vector3 GetRotationOffset() { return RotationOffset; }
        public short getKey() { return this.Key; }
        public bool GetEnemyHit() { return EnemyHit; }

        public double getScoreValue() { return scoreValue; }

        public float EnemyHealth
        {
            get { return Health; }
            internal set 
            {
                Health = value;
                if (Health <= 0)
                    KillThis = true;
                if (Health <= -20)
                    RemoveThis = true;
            }
        }

        public bool IsShaking
        {
            get { return isShaking; }
        }

        #region Movement Bools
        public Vector3 Advance
        {
            get { return advance; }
        }

        public bool TURN_LEFT
        {
            get { return TurnLeft; }
            internal set { TurnLeft = value; }
        }

        public bool TURN_RIGHT
        {
            get { return TurnRight; }
            internal set { TurnRight = value; }
        }

        public bool PITCH_UP
        {
            get { return PitchUp; }
            internal set { PitchUp = value; }
        }

        public bool PITCH_DOWN
        {
            get { return PitchDown; }
            internal set { PitchDown = value; }
        }

        public bool ROLL_LEFT
        {
            get { return RollLeft; }
            internal set { RollLeft = value; }
        }

        public bool ROLL_RIGHT
        {
            get { return RollRight; }
            internal set { RollRight = value; }
        }

        public float TURN_SPEED
        {
            get { return TurnSpeed; }
            internal set { TurnSpeed = value; }
        }

        public float PITCH_SPEED
        {
            get { return PitchSpeed; }
            internal set { PitchSpeed = value; }
        }

        public float ROLL_SPEED
        {
            get { return RollSpeed; }
            internal set { RollSpeed = value; }
        }

        public float ENEMY_SPEED
        {
            get { return EnemySpeed; }
            internal set { EnemySpeed = value; }
        }

        public BoundingSphere AWARENESS_SPHERE
        {
            get { return AwarenessSphere; }
            internal set { AwarenessSphere = value; }
        }

        public BoundingSphere MOVEMENT_SPHERE
        {
            get { return MovementSphere; }
            internal set { MovementSphere = value; }
        }

        #endregion
        #endregion
        #region Mutators
        public void SetPosition(Vector3 pos) { Position = pos; }
        public void SetRotation(Vector3 rot) { Rotation = rot; }
        public void SetEnemyModel(Model mod) { EnemyModel = mod; }
        public void SetRoutineIdentifier(int rid) { RoutineIdentifier = rid; }
        public void SetEnemySpeedMultiplier(float esm) { EnemySpeed = esm; }
        #endregion
        /*
        public Enemy(Model enemymodel, Vector3 position, Vector3 rotation, short KeyVal)
        {
            EnemyModel = enemymodel;
            Position = position;
            Rotation = rotation;
            this.Key = KeyVal;

            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            SetAIRoutine(2);
        }*/

        //Returns the advance vector for some ungodly reason
        private Vector3 AdvanceForward(GameTime gameTime)
        {
            if (TurnRight)
            {
                Rotation.Y += TurnSpeed;
            }
            if (TurnLeft)
            {
                Rotation.Y -= TurnSpeed;
            }
            if (PitchUp)
            {
                Rotation.X += PitchSpeed;
            }
            if (PitchDown)
            {
                Rotation.X -= PitchSpeed;
            }
            if (RollLeft)
            {
                Rotation.Z -= RollSpeed;
            }
            if (RollRight)
            {
                Rotation.Z += RollSpeed;
            }

            //Create a forward pointing vector
            Vector3 AdvanceVector = Vector3.Forward;

            //Transform the vector so it's adjusted to move forward based on rotation values
            AdvanceVector = Vector3.Transform(AdvanceVector, Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));

            //Normalize the vector so all components are between 0 and 1.0
            AdvanceVector = Vector3.Normalize(AdvanceVector);

            //Add the vector times a speed multiplier (for speed up/down) to the position of the enemy
            Position += AdvanceVector * EnemySpeed * elapsedgamemultiplier;

            return AdvanceVector * EnemySpeed * elapsedgamemultiplier;
        }

        public virtual void Update(GameTime gameTime)
        {
            /*
             * Collision detection for player fire will go here
             * If it's hit, then set the enemy hit bool to true
             * That will get passed to the AI so the AI can react
             * if the current AIRoutine allows for it
             */
            elapsedgamemultiplier = gameTime.ElapsedGameTime.Milliseconds / (1000 / Game1.FPS);

            advance = AdvanceForward(gameTime);

            
                        
            if (KillThis)
            {
                TURN_LEFT = false;
                TURN_RIGHT = false;
                PITCH_UP = false;
                PITCH_DOWN = false;
                ROLL_LEFT = false;
                ROLL_RIGHT = true;

                ROLL_SPEED = MathHelper.Pi / 50;

                if (Rotation.X > -70)
                    PITCH_DOWN = true;

                KillTimer -= gameTime.ElapsedGameTime;

                if (KillTimer.TotalMilliseconds <= 0)
                    RemoveThis = true;

                for (int i = 0; i < 1; i++)
                    WindowManager.SmokeParticles.AddParticle(Position, Vector3.Zero);
            }

        }

        protected void UpdateLocations(GameTime gameTime)
        {
            UpdateShake(gameTime);

            //Set the values needed for drawing to the newly updated values
            EnemyObj.setPosition(Position);
            EnemyObj.setRotation(Rotation);

            //Updates the position and orientation of all collision boxes
            UpdateCollision();
        }

        public void Draw(string technique)
        {
            EnemyObj.DisplayModel(CameraClass.getLookAt(), technique, RotationOffset);
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(EnemyObj.getScale())
                   * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                   * Matrix.CreateTranslation(Position);
        }
        

        private void UpdateCollision()
        {
            colboxes[0] = new OBB(Position, new Vector3(132, 34, 80), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.setCollisionBoxes(colboxes);
        }

        private void UpdateAI(GameTime gameTime)
        {
            switch (RoutineIdentifier)
            {
                case 0:
                    StandardAI_Update(gameTime);
                    break;
                case 1:
                    TurnAI_Update(gameTime);
                    break;
                case 2:
                    DefensiveAI_Update(gameTime);
                    break;
                default:
                    StandardAI_Update(gameTime);
                    break;
            }
        }

        private void StandardAI_Update(GameTime gameTime)
        {
            PitchUp = true;
            RollRight = true;
        }
        
        private void TurnAI_Update(GameTime gameTime)
        {
            TurnRight = true;
        }

        private void DefensiveAI_Update(GameTime gameTime)
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

        public void SetAIRoutine(int ai)
        {
            RoutineIdentifier = ai;
            ResetMovementVariables();
            switch (RoutineIdentifier)
            {
                case 0:
                    StandardAI_Init();
                    break;
                case 1:
                    TurnAI_Init();
                    break;
                case 2:
                    DefensiveAI_Init();
                    break;
                default:
                    StandardAI_Init();
                    break;
            }
        }

        private void StandardAI_Init()
        {
            PitchUp = true;
            RollRight = true;
        }

        private void TurnAI_Init()
        {
            TurnRight = true;
        }

        private void DefensiveAI_Init()
        {
            MovementSphere.Center = Position;
            MovementSphere.Radius = 500;

            AwarenessSphere.Center = Position;
            AwarenessSphere.Radius = 200;
        }

        private void ResetMovementVariables()
        {
            TurnRight = false;
            TurnLeft = false;
            PitchUp = false;
            PitchDown = false;
            RollRight = false;
            RollLeft = false;
        }

        #region Shake Stuff
        public void BeginShake()
        {
            ShakeTimer = new TimeSpan(0, 0, 1);
            isShaking = true;
          
        }

        public void UpdateShake(GameTime gameTime)
        {
            if (ShakeTimer.TotalMilliseconds > 0)
            {
                ShakeTimer -= gameTime.ElapsedGameTime;

                //Calculate a rotation based on the amount of time left so the shake is more intense on initial impact
                int timecalc = (int)(ShakeTimer.TotalMilliseconds / 100) * 3;
                
                RotationOffset.X = MathHelper.ToRadians(Randomizer.Next(-timecalc, timecalc));
                RotationOffset.Z = MathHelper.ToRadians(Randomizer.Next(-timecalc, timecalc));
                                
                //Reset the rotation if shaketimer has reached 0
                if (ShakeTimer.TotalMilliseconds <= 0)
                {
                    RotationOffset = Vector3.Zero;
                    isShaking = false;
                }
            }
        }
        #endregion
    }
}
