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


namespace StarForce_PendingTitle_
{
    /// <summary>
    /// this is the abstract class that defines a ship. Players control a ship through a controller class. Online players 
    /// also use this same class.
    /// TODO : IMPLEMENT COLLISION
    /// </summary>
    public abstract class MainShip : CollisionObject
    {
       
        
        //this is the model of the ship
        protected Model mainShip;
        //this is the rotation offset applied to the ship. (Incase the ship was drawn with a different rotation in the model editor)
        protected int rotationOffset;
        //this is the position of the ship. This is used by the collision routine to detect collision on the ship and send it back to the ship
        protected Vector3 position;
        //this is the velocity of the ship. This is used by the collision routine to detect collision on the ship and send it back to the ship
        protected Vector3 velocity;
        protected Vector3 addedvelocity;
        //the previous two variables are ONLY used by collision. Any classes that extend mainShip need not worry about these two variables for anything
        //else
        protected Vector3 RotationValues;
        //this is a new rotation using Quaternions, lets see if it actually works
        protected Quaternion newrotation;
        //this bool sets if collision is turned on or off. Online players do not have collision because their collision is handled by their home CPU.
        protected bool hasCollision;
        //these are offesets for each collision box
        protected Vector3[] CollisionBoxOffsets;
        //is the player banking?
        //this rotation is drawn but doesnt get used when doing movement calculations
        //this rotation is still needed though when doing other calculations like collision
        protected Vector3 drawrotation;
        protected Vector3 olddrawrotation;
        //this tells you if you are boosting
        protected int boost;
        //how many lasers do we need to create
        protected int laserstomake;
  

        private TimeSpan RandomAssTimer = new TimeSpan();
        private TimeSpan RandomAssTimer2 = new TimeSpan();

        protected TimeSpan ShakeTimer = new TimeSpan();

        const float heatIncSpeed = 2f;

        /// <summary>
        /// Health the ship has left, based on a max health of 100
        /// </summary>
        protected float health;
        /// <summary>
        /// the amount of heat a ship has accumulated. The ship can only do certain special moves 
        /// when heat is 0. Loops, Boost, Brake.
        /// </summary>
        protected float heat;
        protected bool isheating;
        protected bool pressedHeatKey;

        /// <summary>
        /// What mode the player is in. 
        /// Playing
        /// Dead
        /// Spawning
        /// </summary>
        protected String Mode;
   
        protected TimeSpan TimeToNextShot = new TimeSpan();

        public String mode
        {
            get { return Mode; }
        }

        public int LasersToMake
        {
            get { return laserstomake; }
        }
        
        public float Health
        {
            get { return health; }
            internal set { health = value; }
        }

        public float Heat
        {
            get { return heat; }
            internal set { heat = value; }
        }

        public TimeSpan shakeTimer
        {
            get { return ShakeTimer; }
        }

        #region  Looping stuff that Ted never needs to see
        protected bool isLooping = false;
        protected String LoopType = "None";
        protected Matrix SavedMatrix;
        protected float RotationToReach;
        protected bool PassedMidPoint;
        protected float ZRotationToReach;
        protected Vector3 LoopStartPosition;
        protected Vector3 RotationValueToReach;

        #endregion


        #region Barrel roll stuff that gokul never needs to see
        protected bool isbanking;
        protected bool bankleft;
        protected bool bankright;
        protected bool isbarreling;
        protected bool barrelleft;
        protected bool barrelright;
        protected float barrelrollcounter = 0f;
        public TimeSpan BarrelRollTimerLeft = TimeSpan.Zero;
        public TimeSpan BarrelRollTimerRight = TimeSpan.Zero;
        public float barrelrolltarget = 0f;
        #endregion

        #region accessors
        public bool IsLooping()
        {
            return isLooping;
        }

        public bool isBoosting()
        {
            if (boost > 0) return true;
            return false;
        }

        public Vector3 Drawrotation
        {
            get { return drawrotation; }
            internal set { this.drawrotation = value; }
        }
        public bool Isbarreling
        {
            get { return isbarreling; }
            internal set { this.isbarreling = value; }
        }

        public Quaternion NewRotation
        {
            get { return newrotation; }
            internal set { this.newrotation = value; }
        }

        public Vector3 rotationValues
        {
            get { return RotationValues; }
            internal set { this.RotationValues = value; }
        }


        public Vector3 Velocity
        {
            get { return velocity; }
            internal set { velocity = value; }
        }


        public Vector3 Position
        {
            get { return this.position; }
            internal set { this.position = value; }
        }

        #endregion
        /// <summary>
        /// this function needs to be called by any class that extends this one
        /// </summary>
        /// <param name="mainShip">this is the ship's model</param>
        /// <param name="rotationOffset">this is the rotation offset used when drawing the ship</param>
        /// <param name="hasCollision">does the collision manager need to check collision on this ship?</param>
        /// <param name="Boxes">collision boxes for this object (can be null)</param>
        public void Init(Model mainShip, int rotationOffset, bool hasCollision, OBB[] Boxes) { 
            if (mainShip != null)
                this.mainShip = mainShip; 

            this.rotationOffset = rotationOffset; 
            this.hasCollision = hasCollision;
            health = 100f;
            isLooping = false;
            LoopType = "None";
            SavedMatrix = new Matrix();
            RotationToReach = 0f;
            PassedMidPoint = false;
            ZRotationToReach = 0f;
            LoopStartPosition = new Vector3();
            health = 100f;
            heat = 0f;
            isheating = false;
            pressedHeatKey = false;
            RotationValueToReach = new Vector3();

            RandomAssTimer = new TimeSpan();
            RandomAssTimer2 = new TimeSpan();
            TimeToNextShot = new TimeSpan();
            ShakeTimer = new TimeSpan();
            laserstomake = 0;
            boost = 0;

            drawrotation = new Vector3();
            olddrawrotation = new Vector3();

            newrotation = Quaternion.Identity;
            RotationValues = new Vector3();

          

            OBB newbox = new OBB(this.position, new Vector3(3, 2, 5));
            Boxes = new OBB[3];
            Boxes[0] = newbox;
            newbox = new OBB(this.position, new Vector3(5, 1, 8));
            Boxes[1] = newbox;
            newbox = new OBB(this.position, new Vector3(5, 1, 8));
            Boxes[2] = newbox;
            //newbox = new OBB(this.position, new Vector3(100,100, 5000));
           // Boxes[3] = newbox;
           

            CollisionBoxOffsets = new Vector3[3];
            CollisionBoxOffsets[0] = new Vector3(0, 0, 0);
            CollisionBoxOffsets[1] = new Vector3(-5, 0, 5);
            CollisionBoxOffsets[2] = new Vector3(5, 0, 5);
            //CollisionBoxOffsets[3] = new Vector3(0,0, -5000-350);
            
           

            base.Init(Boxes);
            
        }
        /// <summary>
        /// all this function does is tell you if you can use any method that requires the ship to use some of its heat
        /// so methods like boost need to call this method before they allow the play to boost
        /// </summary>
        /// <returns></returns>
        protected bool canHeat()
        {
            if (heat == 0f)
            {
                isheating = true;
                return true;
            }
            else
            {
                if (isheating && heat < 100)
                {
                    return true;
                }
            }
            return false;

        }
        /// <summary>
        /// any method that causes the ship to heat needs to call this method
        /// </summary>
        protected void Heating()
        {
            heat += heatIncSpeed;
            if (heat >= 100)
            {
                heat = 100f;
            }
            else
            {
                isheating = true;
                pressedHeatKey = true;
            }
        }

        protected void Heating(float multiplier)
        {
            heat += multiplier* heatIncSpeed;
            if (heat >= 100)
            {
                heat = 100f;
            }
            else
            {
                isheating = true;
                pressedHeatKey = true;
            }
        }


        public Matrix getDrawYawPitchRoll()
        {
            return Matrix.CreateFromYawPitchRoll(drawrotation.Y, drawrotation.X, drawrotation.Z);
        }

        public virtual void MoveXAxis(float amount) { }

        public virtual void MoveYAxis(float amount) { }

        public void StartLoopOverride(String loopType)
        {
            if (this.mode == "Playing" && this.isLooping == false)
            {
                Heating();
                RotationToReach = MathHelper.TwoPi;
                PassedMidPoint = false; this.LoopType = loopType;
                SavedMatrix = this.getMovementPositionRotationMatrix();
                if (loopType == "Reverse")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(120, 100, -50);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                }
                if (loopType == "Left")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(0, 100, 0);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                    RotationValueToReach = rotationValues + new Vector3(0, -MathHelper.PiOver2, 0);
                }
                if (loopType == "Right")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(0, 100, 0);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                    RotationValueToReach = rotationValues + new Vector3(0, MathHelper.PiOver2, 0);
                }


                isLooping = true;
            }
            
        }
        public virtual void StartLoop(string loopType)
        {
            if (this.mode == "Playing" && this.isLooping == false && canHeat())
            {
                Heating();
                RotationToReach = MathHelper.TwoPi;
                PassedMidPoint = false; this.LoopType = loopType;
                SavedMatrix = this.getMovementPositionRotationMatrix();
                if (loopType == "Reverse")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(120, 100, -50);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                }
                if (loopType == "Left")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(0, 100, 0);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                    RotationValueToReach = rotationValues + new Vector3(0, -MathHelper.PiOver2, 0);
                }
                if (loopType == "Right")
                {
                    Vector3 OldPosition = this.position;
                    Vector3 Movement = new Vector3(0, 100, 0);
                    Movement = Vector3.Transform(Movement, Matrix.CreateFromQuaternion(newrotation));
                    position += Movement;
                    SavedMatrix = this.getMovementPositionRotationMatrix();
                    this.position = OldPosition;
                    LoopStartPosition = this.position;
                    RotationValueToReach = rotationValues + new Vector3(0, MathHelper.PiOver2, 0);
                }
                

                isLooping = true;
            }
            
        }
        public virtual void EndLoop() { isLooping = false; this.LoopType = "None"; }

        protected virtual void Loop() { if (this.isLooping) Heating(2f); }


        public virtual void BankLeft() { }
        public virtual void BankRight() { }

        public virtual void BarrelLeft(float Target) { barrelrolltarget = Target; }
        public virtual void BarrelRight(float Target) { barrelrolltarget = Target; }
        public virtual void BarrelRoll() { }

        public virtual void BeginShake() { }

        public virtual void Boost(GameTime gameTime, float amount, float old)
        {
            //Is Boosting
            if (amount > 0 && canHeat())
            {
                Heating();

                //Pause the normal engine
                SoundFX.PauseSound("Engine");

                //If it just turned on
                if (old == 0)
                {
                    //Play the boost on sound
                    SoundFX.PlaySound("Boost On");

                    //set the timer up so it waits before playing the boost loop
                    RandomAssTimer2 = new TimeSpan(0, 0, 0, 0, 750);
                }
                //Has not turned on recently
                else
                {
                    //If the boost on timer is done
                    if (RandomAssTimer2.TotalMilliseconds <= 0)
                    {
                        //If the boost loop sound has already been called
                        if (SoundFX.LoopingSoundExist("Boost Loop"))
                            //Resume playing the previously paused sound
                            SoundFX.ResumeSound("Boost Loop");
                        else
                            //Start looping the sound
                            SoundFX.LoopSound("Boost Loop");
                    }
                    else
                    {
                        //decrease the timer
                        RandomAssTimer2 -= gameTime.ElapsedGameTime;
                    }
                }
            }
            // not boosting
            else
            {
                //just stopped boosting
                if (old > 0)
                {
                    //pause the boost loop
                    SoundFX.PauseSound("Boost Loop");
                    //play boost off
                    SoundFX.PlaySound("Boost Off");

                    //set the timer so that it waits before playing normal engine sound
                    RandomAssTimer = new TimeSpan(0, 0, 0, 0, 500);
                }

                //If the timer is down
                if (RandomAssTimer.TotalMilliseconds <= 0)
                {
                    //if it already has been played resume the sound
                    if (SoundFX.LoopingSoundExist("Engine"))
                        SoundFX.ResumeSound("Engine");
                    else
                        //begin looping the sound
                        SoundFX.LoopSound("Engine");
                }
                else
                {
                    //decrease the timer
                    RandomAssTimer -= gameTime.ElapsedGameTime;
                }
            }
        }
        /// <summary>
        /// used by any camera class to figure out what position the camera should point it.
        /// the camera can use this value and then offset it as it pleases.
        /// </summary>
        /// <returns>The position it should point at in Vector3 format</returns>
        public virtual Vector3 getPointingAt()
        {
            return this.position;

        }

        public virtual Matrix getMovementPositionRotationMatrix() {
            Matrix MAT;
            if (!isLooping)
            {
                 MAT = Matrix.CreateScale(1.0f)

               * Matrix.CreateFromQuaternion(newrotation)
               * Matrix.CreateTranslation(Position);
            }
            else
            {
                MAT = SavedMatrix;
            }
            return MAT;
        }

        public virtual Matrix getNonMovementPositionRotationMatrix()
        {
            Matrix MAT = Matrix.CreateScale(1.0f)
                * getDrawYawPitchRoll()
            *Matrix.CreateFromQuaternion(newrotation)
            * Matrix.CreateTranslation(Position); 
                        return MAT;
        }


        public virtual void CheckCollision() { }

        public virtual Matrix Update(GameTime gameTime) {
          
            for (int i =0; i<CollisionData.getCollisionBoxes().Length;i++)
            {
                OBB CollisionBox = CollisionData.getCollisionBoxes()[i];
                CollisionBox.Center = this.CollisionBoxOffsets[i];
               
                CollisionBox.Center = Vector3.Transform(CollisionBox.Center, getNonMovementPositionRotationMatrix());
                //CollisionBox. = getNonMovementPositionRotationMatrix();
                CollisionBox.Rotation = Matrix.CreateFromQuaternion(newrotation);
            }
            if (!canHeat())
            {
                heat -= .6f * heatIncSpeed;
                heat = MathHelper.Max(0, heat);
            }
            if (pressedHeatKey == false)
            {
                isheating = false;
            }
            pressedHeatKey = false;
            MakeLaser();
            TimeToNextShot -= gameTime.ElapsedGameTime;

            return this.getMovementPositionRotationMatrix();
        }


        protected virtual void MakeLaser()
        {
            if (laserstomake > 0 && TimeToNextShot.TotalMilliseconds <= 0)
            {
                laserstomake--;

                Vector3 newrotation = new Vector3();
                newrotation.Y -= MathHelper.Pi;

                Laser newlaser = new Laser(this.Position, newrotation, Matrix.CreateFromQuaternion(this.NewRotation), 20f, Laser.Source.Player, Playing.LaserModel, LaserManager.IdValue);

                Playing.LaserManagement.AddLaser(newlaser);

                TimeToNextShot = new TimeSpan(0, 0, 0, 0, 100);
            }
        }

        public void FireMissle()
        {
            Playing.PlayerMissile.FireMissle(this.Position, Vector3.Zero, Matrix.CreateFromQuaternion(this.NewRotation));
        }

        public void FireMissle(Vector3 target)
        {
            Playing.PlayerMissile.FireMissle(this.Position, Vector3.Zero, Matrix.CreateFromQuaternion(this.NewRotation));
            Playing.PlayerMissile.TargetVector = target;
        }

        public void CancelLaser()
        {
            laserstomake = 0;
            TimeToNextShot = new TimeSpan();
        }

        /// <summary>
        /// this function draws the ship on the screen based on a position and a rotation
        /// </summary>
        /// <param name="Position">Position of the ship</param>
        /// <param name="Rotation">Rotation of the ship</param>
        public void DisplayModelDebug(Matrix ViewMatrix)
        {

            Matrix[] transforms = new Matrix[mainShip.Bones.Count];
            mainShip.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in mainShip.Meshes)
            {
                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationY(MathHelper.ToRadians(rotationOffset))
                        * getNonMovementPositionRotationMatrix();

                    effect.View = ViewMatrix;
                    effect.Projection = CameraClass.getPerspective();

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }

            DebugManager.drawDebugBox(this.CollisionData, getNonMovementPositionRotationMatrix(), ViewMatrix);
            

        }

        public Matrix GetWorldMatrix()
        {
            return getNonMovementPositionRotationMatrix();
        }
        
        public void DisplayModelDebug(Matrix ViewMatrix, string Technique, SpriteBatch Batch, SpriteFont Font)
        {
            if (Mode != "Death")
            {
                Matrix[] transforms = new Matrix[mainShip.Bones.Count];
                mainShip.CopyAbsoluteBoneTransformsTo(transforms);
                //Draw the model, a model can have multiple meshes, so loop
                foreach (ModelMesh mesh in mainShip.Meshes)
                {
                    //This is where the mesh orientation is set, as well as our camera and projection
                    foreach (Effect effect in mesh.Effects)
                    {

                        /* effect.EnableDefaultLighting();
                         effect.PreferPerPixelLighting = true;*/
                        effect.CurrentTechnique = effect.Techniques[Technique];

                        Matrix localWorld = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateRotationY(MathHelper.ToRadians(rotationOffset))
                            * getNonMovementPositionRotationMatrix();

                        effect.Parameters["World"].SetValue(localWorld);
                        effect.Parameters["View"].SetValue(ViewMatrix);
                        effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                        /*
                        effect.Parameters["World"].SetValue(localWorld);
                        effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                        effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/



                    }
                    //Draw the mesh, will use the effects set above.
                    mesh.Draw();
                }
            }
         
            //DebugManager.drawDebugBox(this.CollisionData, getNonMovementPositionRotationMatrix(), ViewMatrix);
        }

        public void DisplayModelDebug(Matrix ViewMatrix, SpriteBatch Batch, SpriteFont Font, Effect CustomEffect)
        {
            CustomEffect.CurrentTechnique = CustomEffect.Techniques["SpecularPerPixel"];
            CustomEffect.Begin();
            //Draw the model, a model can have multiple meshes, so loop
            foreach (EffectPass pass in CustomEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                foreach (ModelMesh mesh in mainShip.Meshes)
                {
                    Matrix[] transforms = new Matrix[mainShip.Bones.Count];
                    mainShip.CopyAbsoluteBoneTransformsTo(transforms);
                    Matrix World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(rotationOffset))
                                   * getNonMovementPositionRotationMatrix();

                    Matrix View = ViewMatrix;
                    Matrix Projection = CameraClass.getPerspective();

                    CustomEffect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
                    CustomEffect.Parameters["world"].SetValue(World);
                    CustomEffect.Parameters["viewInverse"].SetValue(View);
                    CustomEffect.Parameters["lightDir"].SetValue(new Vector3(1, 0, 0));

                    //This is where the mesh orientation is set, as well as our camera and projection
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        
                    }
                    //Draw the mesh, will use the effects set above.
                    mesh.Draw();
                }
                pass.End();
            }
            CustomEffect.End();

            // DebugManager.drawDebugBox(this.CollisionData, getNonMovementPositionRotationMatrix(), ViewMatrix);
        }

        public virtual void LockAxis(float amount) { }

        public virtual void Draw2D(SpriteBatch batch, SpriteFont font) { }

        public void setHasCollision(bool has) { this.hasCollision = has; }

        public void createLasers(int Number) { this.laserstomake = Number; }

        
        
    }
}
