#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Library.Network;
using Ziggyware.Xna;
#endregion

namespace StarForce_PendingTitle_
{

    /// <summary>
    /// All Range ships are constantly moving forward, but they can move around in any direction they want
    /// perfect for dogfights
    /// </summary>
    public class AllRangeShip : MainShip
    {
        //this is the center position of the ship. 
        private Vector3 shipCenterPosition;
                
        //These are rotation values applied for that frame. 
        //Vector3 RotationValues = new Vector3();
       
        //This timespan measures how long the player should do the hit animation
        TimeSpan HitTimespan = new TimeSpan(0, 0, 0);

        Random Randomizer;

        float elapsedtimemultiplier;

        TimeSpan SpawnTimeSpan;

        TimeSpan DeathTimeSpan;
        /// <summary>
        /// this is used by the death routine, do not touch.
        /// </summary>
        float SavedRotationY;
        /// <summary>
        /// this is used by the death routine, do not touch.
        /// </summary>
        Vector3 SavedPosition;
        

        //These are constants about the ship, change them up here and the whole gameplay will be affected
        private const float Speed = 0.01f;
        private const float XAxisSpeed = 0.012f;
        private const float YAxisSpeed = 0.02f;
        private const float XAxisRecover = 0.08f;
        private const float YAxisRecover = 0.03f;
        private const float advanceSpeed = 5;
        private const float XMaxDistFromCenter = 30f;
        private const float YMaxDistFromCenter = 30f;
        private const float BankRotationSpeed = .1f;
        private const float BankRotationResetSpeed = .05f;
        private const float BankRotationAngle = 15f;
        
        
        float ingameadvancespeed = advanceSpeed;
        public Vector3 ShipCenterPosition
        {
            get { return this.shipCenterPosition; }
            internal set { this.shipCenterPosition = value; }
        }

        
        public AllRangeShip(Model pmodel)
        {
            shipCenterPosition = Vector3.Zero;
            this.position = StarForce_PendingTitle_.Playing.StartingPosition;
            velocity = Vector3.Zero;
            Randomizer = new Random();
            Mode = "Spawning";
            SpawnTimeSpan = new TimeSpan(0, 0, 0, 5);
            DeathTimeSpan = new TimeSpan(0, 0, 0, 3);
            SavedPosition = new Vector3();
            SavedRotationY = 0f;
            elapsedtimemultiplier = 0f;
            HitTimespan = new TimeSpan();

            Init(pmodel, 0, false, null);
            

        }
        /// <summary>
        /// This function is called at respawn. Initializes Everything.
        /// </summary>
        public void Init()
        {
            shipCenterPosition = Vector3.Zero;
            this.position = StarForce_PendingTitle_.Playing.StartingPosition;
            velocity = Vector3.Zero;
            Randomizer = new Random();
            Mode = "Spawning";
            SpawnTimeSpan = new TimeSpan(0, 0, 0, 5);
            DeathTimeSpan = new TimeSpan(0, 0, 0, 3);
            SavedPosition = new Vector3();
            SavedRotationY = 0f;
            elapsedtimemultiplier = 0f;
            HitTimespan = new TimeSpan();

            Init(null, 0, false, null);

        }

        //the following function gets called automatically and advances the ship straight forward.
        public void advance()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.B) || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) ingameadvancespeed = 0;
            
            //Vector3 newvector = new Vector3((float)Math.Cos(MathHelper.ToRadians(this.rotation.Y)) * advanceSpeed, 0, (float)Math.Sin(MathHelper.ToRadians(this.rotation.Y)) * advanceSpeed);
            Vector3 newvector = Vector3.Forward;
            newvector = Vector3.Transform(newvector, Matrix.CreateFromQuaternion(newrotation));
            newvector.Normalize();
            this.position += newvector * ingameadvancespeed * elapsedtimemultiplier;
            this.position += addedvelocity;
            addedvelocity = Vector3.Lerp(addedvelocity, Vector3.Zero, .05f);
        }


        public Vector3[] findCollisionVectors(OBB O)
        {
            BoundingBox B = O.LocalBoundingBox;


            Vector3[] Position = new Vector3[8];
            Position[0] = new Vector3(B.Min.X, B.Min.Y, B.Min.Z);
            Position[1] = new Vector3(B.Max.X, B.Min.Y, B.Min.Z);
            Position[2] = new Vector3(B.Max.X, B.Min.Y, B.Max.Z);
            Position[3] = new Vector3(B.Min.X, B.Min.Y, B.Max.Z);

            Position[4] = new Vector3(B.Min.X, B.Max.Y, B.Min.Z);
            Position[5] = new Vector3(B.Max.X, B.Max.Y, B.Min.Z);
            Position[6] = new Vector3(B.Max.X, B.Max.Y, B.Max.Z);
            Position[7] = new Vector3(B.Min.X, B.Max.Y, B.Max.Z);

            return Position;
        }

        public override void BeginShake()
        {
            olddrawrotation = drawrotation;
            ShakeTimer = new TimeSpan(0, 0, 0, 0, 1000);
        }

        public bool CanGetHurt()
        {
            return ShakeTimer.TotalSeconds <= 0;
        }

        public override void CheckCollision()
        {
            if (this.CollisionData.foundoverallcollision)
            {
                //Okay we know collision happened somewhere, lets find out where

                float CollisionMoverMultiplier = 2f;

               
                String CheckCollision = "None";
                if (CollisionData.CollisionFound[1] && CollisionData.CollisionFound[2])
                {
                    int LeftCounter = 0;
                    int RightCounter = 0;
                    OBB LeftWing = CollisionData.getCollisionBoxes()[1];
                    OBB RightWing = CollisionData.getCollisionBoxes()[2];
                    foreach (bool b in LeftWing.CollisionVerts)
                    {
                        if (b == true) LeftCounter++;
                    }
                    foreach (bool b in RightWing.CollisionVerts)
                    {
                        if (b == true) RightCounter++;
                    }
                    for (int i = 0; i < LeftWing.CollisionVerts.Length; i++)
                    {
                        LeftWing.CollisionVerts[i] = false;
                    }
                    for (int i = 0; i < RightWing.CollisionVerts.Length; i++)
                    {
                        RightWing.CollisionVerts[i] = false;
                    }
                    if (LeftCounter > RightCounter)
                    {
                        CheckCollision = "Left";
                    }
                    else
                    {
                        CheckCollision = "Right";
                    }
                    if (LeftCounter == RightCounter)
                    {
                        CheckCollision = "Both";
                    }
                }
                else
                {
                    CheckCollision = "Left";
                    if (CollisionData.CollisionFound[2])
                    {
                        CheckCollision = "Right";
                    }
                }
                if (!isLooping)
                {
                    if (CheckCollision == "Left")
                    {
                        //The Left Wing was hit
                        OBB LeftWing = CollisionData.getCollisionBoxes()[1];
                        Vector3[] Position = findCollisionVectors(LeftWing);


                        Vector3 Point1 = Position[0];
                        Vector3 Point2 = Position[1];

                        Point1 = Vector3.Transform(Point1, LeftWing.WorldTransform);
                        Point2 = Vector3.Transform(Point2, LeftWing.WorldTransform);

                        Vector3 DirectionVec = Point1 - Point2;

                        DirectionVec = new Vector3(1, 0, 0);
                        DirectionVec = Vector3.Transform(DirectionVec, Matrix.CreateFromQuaternion(newrotation));

                        DirectionVec.Normalize();

                        if (CanGetHurt())
                        {
                            Health -= 10f;
                        }

                        Hit(DirectionVec, CollisionMoverMultiplier);

                      

                    }
                    if (CheckCollision == "Right")
                    {
                        //The Right Wing was hit
                        OBB RightWing = CollisionData.getCollisionBoxes()[2];
                        Vector3[] Position = findCollisionVectors(RightWing);


                        Vector3 Point1 = Position[0];
                        Vector3 Point2 = Position[1];

                        Point1 = Vector3.Transform(Point1, RightWing.WorldTransform);
                        Point2 = Vector3.Transform(Point2, RightWing.WorldTransform);

                        Vector3 DirectionVec = Point2 - Point1;

                        DirectionVec = new Vector3(-1, 0, 0);
                        DirectionVec = Vector3.Transform(DirectionVec, Matrix.CreateFromQuaternion(newrotation));

                        DirectionVec.Normalize();


                        if (CanGetHurt())
                        {
                            Health -= 10f;
                        }

                        Hit(DirectionVec, CollisionMoverMultiplier);

                        
                    }
                    if (CheckCollision == "Both")
                    {
                       

                       Vector3 DirectionVec = new Vector3(0, 1, 0);
                        DirectionVec = Vector3.Transform(DirectionVec, Matrix.CreateFromQuaternion(newrotation));

                        DirectionVec.Normalize();

                        if (CanGetHurt())
                        {
                            Health -= 20f;
                        }

                        Hit(DirectionVec, CollisionMoverMultiplier);

                      

                    }
                }
                

                CollisionData.CollisionFound[0] = false;
                CollisionData.CollisionFound[1] = false;
                CollisionData.CollisionFound[2] = false;
               
                CollisionData.foundoverallcollision = false;
            }
        }


        private void Hit(Vector3 DirectionVec, float CollisionMoverMultiplier)
        {
            //Bounce them
            addedvelocity += DirectionVec * CollisionMoverMultiplier;

            //Make them face forward again
            RotationValues.X = 0f;

            //Invulnerability time stuff
            if (ShakeTimer.TotalMilliseconds <= 0)
            {
                BeginShake();
            }
        }

        /// <summary>
        /// the following move the ship. up and down use the y axis, while left and right increase y rotation
        /// </summary>
        #region movement functions
        public override void Boost(GameTime gameTime, float amount, float old)
        {
            float maxfov = 60f;
            float fisheyespeed = .1f;
            
            if (amount > 0 && !isLooping && canHeat() && Mode == "Playing")
            {
                ingameadvancespeed = MathHelper.Lerp(ingameadvancespeed, 4 * advanceSpeed, .99f);
                Game1.FieldOfView = MathHelper.Lerp(Game1.FieldOfView, maxfov, fisheyespeed);
                boost = 2;
            }
            else
            {
                amount = 0; //incase boost failed for any of the other reasons we dont want to sound to start.
                Game1.FieldOfView = MathHelper.Lerp(Game1.FieldOfView, Game1.DefaultFieldOfView, fisheyespeed);
            }
            base.Boost(gameTime, amount, old);
        }

        public override void MoveYAxis(float amount)
        {

            if (!(amount > 0 && Position.Y >= Boundries.MaxHeight) && !isLooping && (Mode == "Playing" || Mode == "Victory"))
            {
                    RotationValues.X += amount * XAxisSpeed * elapsedtimemultiplier;
            }
            
        }

        public override void   MoveXAxis(float amount)
        {
            amount *= elapsedtimemultiplier;   
            if (!isbarreling && !isbanking && !isLooping && (Mode == "Playing" || Mode == "Victory"))
            {
                RotationValues.Y += amount * YAxisSpeed;
                if (!isbarreling)
                {
                    RotationValues.Z += amount * YAxisSpeed;

                    drawrotation.X = MathHelper.Lerp(drawrotation.X, 0, BankRotationResetSpeed);
                }
            }
            else if (bankleft)
            {
                if (amount > 0)
                {
                    RotationValues.Y += 2f * YAxisSpeed * amount;
                    drawrotation.X = MathHelper.Lerp(drawrotation.X, MathHelper.ToRadians(BankRotationAngle) * amount, BankRotationSpeed);

                }
                else
                {
                    RotationValues.Y += .5f * YAxisSpeed * amount;
                }
                if (amount == 0) drawrotation.X = MathHelper.Lerp(drawrotation.X, 0, BankRotationResetSpeed);
            }
            else if (bankright)
            {
                if (amount < 0)
                {
                    RotationValues.Y += 2f * YAxisSpeed * amount;
                    drawrotation.X = MathHelper.Lerp(drawrotation.X, MathHelper.ToRadians(BankRotationAngle) * amount*-1, BankRotationSpeed);
                }
                else
                {
                    RotationValues.Y += .5f * YAxisSpeed * amount;
                }
                if (amount == 0)
                {
                    drawrotation.X = MathHelper.Lerp(drawrotation.X, 0, BankRotationResetSpeed);
                }
            }
         
        }

        public override void BankLeft()
        {
            if (!isLooping && (Mode == "Playing" || Mode == "Victory"))
            {
                isbanking = true;
                RotationValues.Z += 4 * YAxisSpeed * elapsedtimemultiplier;
                RotationValues.Z = MathHelper.Min(RotationValues.Z, MathHelper.PiOver2);
                bankleft = true;
                base.BankLeft();
            }
        }

        public override void BankRight()
        {
            if (!isLooping && (Mode == "Playing" || Mode == "Victory"))
            {
                isbanking = true;
                RotationValues.Z -= 4 * YAxisSpeed * elapsedtimemultiplier;
                RotationValues.Z = MathHelper.Max(RotationValues.Z, -MathHelper.PiOver2);
                bankright = true;
                base.BankRight();
            }
        }

        public override void BarrelLeft(float Target)
        {
            if ( Mode == "Playing")
            {
                isbanking = false;
                bankleft = false;
                bankright = false;
                isbarreling = true;
                barrelleft = true;
                barrelright = false;
                barrelrollcounter = 0f;
                base.BarrelLeft(Target);
            }
        }

        public override void BarrelRight(float Target)
        {
            if (Mode == "Playing")
            {
                isbanking = false;
                bankleft = false;
                bankright = false;
                isbarreling = true;
                barrelleft = false;
                barrelright = true;
                barrelrollcounter = 0f;
                base.BarrelRight(Target);
            }
        }

        public override void BarrelRoll()
        {
            if (barrelleft)
            {
                drawrotation.Z += .3f;

                if (drawrotation.Z >= barrelrolltarget)
                {
                    isbarreling = false;
                    barrelleft = false;
                    barrelright = false;
                    drawrotation.Z = barrelrolltarget - MathHelper.TwoPi;
                }
            }

            if (barrelright)
            {
                drawrotation.Z -= .3f;

                if (drawrotation.Z <= -MathHelper.TwoPi)
                {
                    isbarreling = false;
                    barrelleft = false;
                    barrelright = false;
                    drawrotation.Z = barrelrolltarget - MathHelper.TwoPi;
                }
            }
        }

        protected override void Loop()
        {
            if (this.isLooping)
            {
                if (this.LoopType == "Loop")
                {
                    RotationValues.X += 3.5f * YAxisSpeed * elapsedtimemultiplier;
                    if (RotationValues.X > this.RotationToReach - MathHelper.Pi)
                    {
                        PassedMidPoint = true;
                    }
                    if (RotationValues.X >= RotationToReach && PassedMidPoint == true)
                    {
                        RotationValues.X = RotationToReach - MathHelper.TwoPi;
                        EndLoop();
                    }
                }
                //if (this.LoopType == "Reverse")
                else
                {
                    bool HasRotated = false;
                    if (this.LoopType == "Left")
                    {
                        RotationValues.Y -= 3 * YAxisSpeed;
                        if (RotationValues.Y <= RotationValueToReach.Y) HasRotated = true;
                    }
                    if (this.LoopType == "Right")
                    {
                        RotationValues.Y += 3 * YAxisSpeed;
                        if (RotationValues.Y >= RotationValueToReach.Y) HasRotated = true;
                    }
                    if (PassedMidPoint == false)
                    {
                        ingameadvancespeed = advanceSpeed;
                        RotationValues.X += 4* YAxisSpeed * elapsedtimemultiplier;
                        if (RotationValues.X > this.RotationToReach - MathHelper.Pi)
                        {
                            PassedMidPoint = true;
                            BarrelLeft((float)Math.PI);
                        }
                      
                    }
                    else
                    {
                        if (this.LoopType == "Reverse")
                        {
                            //ingameadvancespeed = advanceSpeed;
                            if (Isbarreling == false && Vector3.Distance(this.position, new Vector3(LoopStartPosition.X, position.Y, LoopStartPosition.Z)) > 250)
                            {
                                RotationValues.X = RotationToReach - MathHelper.TwoPi;
                                RotationValues.Y += MathHelper.Pi;
                                drawrotation.Z = 0f;
                                EndLoop();
                            }
                        }
                        else
                        {
                            if (isbarreling == false && HasRotated)
                            {
                                RotationValues.X = RotationToReach - MathHelper.TwoPi;
                                RotationValues.Y += MathHelper.Pi;
                                drawrotation.Z = 0f;
                                EndLoop();
                            }
                        }

                    }
                }
            }
            base.Loop();
           
        }
        #endregion


        public Matrix Playing(GameTime gameTime)
        {

            //Reset advance speed (incase they were boosting)
            ingameadvancespeed = MathHelper.Lerp(ingameadvancespeed, advanceSpeed, .3f);

            //Caculate how the player is going to rotate (based on his movement)
            Quaternion additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), RotationValues.Y)
                * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
            * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;

            //reset the X axis
            float localXAxisRecover = XAxisRecover * elapsedtimemultiplier;
            float localYAxisRecover = YAxisRecover * elapsedtimemultiplier;

            if (!isLooping) RotationValues.X = (float)MathHelper.Lerp(RotationValues.X, 0, localXAxisRecover / 4);
            //reset the Z axis (if your not banking)
            if (!isbanking)
                RotationValues.Z = (float)MathHelper.Lerp(RotationValues.Z, 0, localYAxisRecover);
            //Advance the player
            advance();

            CheckCollision();

            //add the new velocity to the position
            this.position += this.velocity;

            if (isbarreling)
                BarrelRoll();

            Loop();

            UpdateShake(gameTime.ElapsedGameTime);

            //Clamp the player Y (so he cant get below the water)

            if (position.Y > Boundries.MaxHeight) position.Y = Boundries.MaxHeight;
            //this.position.Y = MathHelper.Clamp(position.Y, 110, position.Y + 1);
            //reset the velocity
            this.velocity = Vector3.Lerp(velocity, Vector3.Zero, .4f);

            //From here on the player has moved so its stuff not dealing with movement-----
            //Reset the values that need to be reset
            isbanking = false;
            bankleft = false;
            bankright = false;
            boost--;
            if (boost < 0) boost = 0;

            //lets do a quick check to see if the player died
            if (health <= 0)
            {
                SetDeath();
            }
            //and another one to see if mission was completed
            if (StarForce_PendingTitle_.Playing.MissionComplete && Mode != "Victory")
            {
                SetVictory();
            }


            return base.Update(gameTime);

        }

        public Matrix Spawning(GameTime gameTime)
        {
            Matrix ReturnMatrix;

            SpawnTimeSpan -= gameTime.ElapsedGameTime;

            ingameadvancespeed = MathHelper.Lerp(ingameadvancespeed, advanceSpeed, .3f);

            RotationValues.Y += YAxisSpeed * elapsedtimemultiplier;

            Quaternion additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), RotationValues.Y)
                * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
            * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;

            //isLooping = false;

            ReturnMatrix = this.getMovementPositionRotationMatrix();

             additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), 0)
                 * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
             * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;

            if (RotationValues.Y >= 2 * Math.PI)
            {
                RotationValues.Y = 0;
                if (SpawnTimeSpan.TotalSeconds <= 0)
                {
                    Mode = "Playing";
                }
            }

            base.Update(gameTime);


            return ReturnMatrix;

        }

        public Matrix Death(GameTime gameTime)
        {
            Matrix ReturnMatrix;

            DeathTimeSpan -= gameTime.ElapsedGameTime;

            ingameadvancespeed = MathHelper.Lerp(ingameadvancespeed, advanceSpeed, .3f);

            Vector3 OldPosition = this.position;

            RotationValues.Y += YAxisSpeed * elapsedtimemultiplier;

            /*Quaternion additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), RotationValues.Y)
                * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
            * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;*/

            Vector3 newposition = Vector3.Backward;
            newposition = Vector3.Transform(newposition, Matrix.CreateFromQuaternion(newrotation));
            SavedPosition += newposition * ingameadvancespeed * elapsedtimemultiplier;
            position = SavedPosition;

            ReturnMatrix = this.getMovementPositionRotationMatrix();

            //Revert the player back to his old rotation so that it looks to the viewer like the camera is spinning around the player
            Quaternion additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), SavedRotationY)
                * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
            * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;
            this.position = OldPosition;

            if (DeathTimeSpan.TotalSeconds <=0)
            {
                SetSpawn();
            }
           
            base.Update(gameTime);


            return ReturnMatrix;

        }

        public override Matrix Update(GameTime gameTime)
        {
            elapsedtimemultiplier = gameTime.ElapsedGameTime.Milliseconds / (1000 / Game1.FPS);
            Matrix CameraMatrix = new Matrix();

            if (Mode == "Playing") CameraMatrix = Playing(gameTime);
            if (Mode == "Spawning") CameraMatrix = Spawning(gameTime);
            if (Mode == "Death") CameraMatrix =  Death(gameTime);
            if (Mode == "Victory")
            {
                Playing(gameTime);
                CameraMatrix = SavedMatrix;
            }
            return CameraMatrix;
        }

        private void UpdateShake(TimeSpan Elapsed)
        {
            //Time to shake?
            if (ShakeTimer.TotalMilliseconds > 0)
            {
                //Update shake timer
                ShakeTimer -= Elapsed;
                //Shake shake shake... shake shake shake... shake your booty
                ShakeShip();
            }
            else
            {
                //make it all normal pl0x
                if (olddrawrotation != Vector3.Zero)
                {
                    drawrotation = Vector3.Zero;
                    olddrawrotation = Vector3.Zero;
                }
            }
        }

        private void ShakeShip()
        {
            //Calculate a rotation based on the amount of time left so the shake is more intense on initial impact
            int timecalc = (int)(ShakeTimer.TotalMilliseconds / 100) * 3;
                       
            drawrotation.Z = MathHelper.ToRadians(Randomizer.Next(-timecalc, timecalc));
            
            //really freaking lazy
            if (drawrotation.Z == 0f) drawrotation.Z = MathHelper.ToRadians(0.1f);

            //Reset the rotation if shaketimer has reached 0
            if (ShakeTimer.TotalMilliseconds <= 0)
            {
                drawrotation = olddrawrotation;
            }
        }


        public override void Draw2D(SpriteBatch batch, SpriteFont font)
        {
            if (Mode == "Spawning")
            {
                if (SpawnTimeSpan.Seconds < 0) SpawnTimeSpan = new TimeSpan(0, 0, 0);
                Vector2 Size = font.MeasureString(SpawnTimeSpan.Seconds.ToString());
                batch.DrawString(font, SpawnTimeSpan.Seconds.ToString(), new Vector2(400 - Size.X / 2, 300 - Size.Y / 2), Color.Black);
            }
           // batch.DrawString(font, (this.position).ToString(), new Vector2(0, 200), Color.White);
            base.Draw2D(batch, font);
        }


        public void SetDeath()
        {
            Mode = "Death";
            SavedRotationY = RotationValues.Y;
            SavedPosition = this.position;
            int numOfParticles = 0;
            if (Specs.ParticleEffects == Specs.Detail.High)
                numOfParticles = 30;
            else if (Specs.ParticleEffects == Specs.Detail.Medium)
                numOfParticles = 15;
            else
                numOfParticles = 0;

            for (int explosion = 0; explosion < numOfParticles; explosion++)
            {
                WindowManager.ExplosionParticles.AddParticle(this.position, Vector3.Zero);
                WindowManager.ExplosionSmokeParticles.AddParticle(this.position, Vector3.Zero);
            }
        }

        public void SetVictory()
        {
            Mode = "Victory";
            SavedMatrix = this.getMovementPositionRotationMatrix();
        }

        public void SetSpawn()
        {
            Init();
        }
        

    }
}
