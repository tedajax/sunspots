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
#endregion

namespace StarForce_PendingTitle_
{

    /// <summary>
    /// Railship extends mainship and allows the ship to follow a predetermined path created by the way point system
    /// TODO : IMPLEMENT WAYPOINT SYSTEM
    /// </summary>
    public class RailShip : MainShip
    {
        //this is the center position of the ship. Basically this is the thing that moves on rails and then ship is being pulled by this thing
        private Vector3 shipCenterPosition;
          //Offset Position is only a vector 2 because the ship only moves on two axis', obviously this changes when the ship rotates
        //but the point is that in terms of the offset ship, it is only really moving in two axis's
        private Vector3 offsetPosition;
        //used to draw the ship
        private Vector3 combinedPosition;

        private Vector3 TargetPosition;
       /* private Vector3 TargetRotation;
        private Vector3 OriginalPosition;
        private Vector3 OriginalRotation;
        private float WaypointRate;
        private float WaypointTurnRate;
        private float CurrentRate;
        private float CurrentTurnRate;*/

        private Queue<Vector3> Waypoints;

        Matrix TargettingMatrix;

        //WHICH WAYPOINT?????
        private int currwaypoint = -1;

        bool ismovingXAxix = false;
        bool ismovingYAxis = false;

        bool axislocked = false;


        //These are constants about the ship, change them up here and the whole gameplay will be affected
        private const float Speed = .5f;
        private const float advanceSpeed = 6;
        private const float XMaxDistFromCenter = 50f;
        private const float YMaxDistFromCenter = 30f;

        float ingameadvancespeed = advanceSpeed;
        float elapsedtimemultiplier;

        bool LerpTehMatricies;
        TimeSpan LerpTimeSpan;

        public Vector3 ShipCenterPosition
        {
            get { return this.shipCenterPosition; }
            internal set { this.shipCenterPosition = value; }
        }

        public Vector3 OffsetPosition
        {
            get { return this.offsetPosition; }
            internal set { this.offsetPosition = value; }
        }

        public RailShip(Model pmodel, Queue<Vector3> waypointdata)
        {
            shipCenterPosition = Vector3.Zero;
            offsetPosition = Vector3.Zero;
            Waypoints = waypointdata;
            

            Init(pmodel, 0, false, null);
            this.ShipCenterPosition = Playing.StartingPosition;
            this.TargetPosition = Waypoints.Dequeue();
            TargettingMatrix =  CreateLockOn(this.TargetPosition, this.shipCenterPosition);

            this.RotationValues.Y -= MathHelper.ToRadians(20);

            this.Mode = "Playing";
        }

        public Matrix CreateLockOn(Vector3 Target, Vector3 Position)
        {
            Matrix NewMatrix;
            NewMatrix = Matrix.Identity;
            NewMatrix.Forward =Target - Position;
            NewMatrix.Forward = Vector3.Normalize(NewMatrix.Forward);
            NewMatrix.Right = Vector3.Cross(NewMatrix.Forward, Vector3.Up);
            NewMatrix.Right = Vector3.Normalize(NewMatrix.Right);
            NewMatrix.Up = Vector3.Cross(NewMatrix.Right, NewMatrix.Forward);
            NewMatrix.Up = Vector3.Normalize(NewMatrix.Up);

            return NewMatrix;

        }

        public float NewLerp(float amt1, float dest, float add)
        {
            if (amt1 > dest)
            {
                amt1 -= add;
                if (amt1 < dest) amt1 = dest;
                //amt1 = MathHelper.Clamp(amt1, dest, amt1); 
            }
            else
            {
                amt1 += add;
                if (amt1 > dest) amt1 = dest;
                //amt1 = MathHelper.Clamp(amt1, amt1, dest);
            }

            return amt1;
        }

        public float RotationLerp(float amt1, float dest, float add)
        {
            if (amt1 == 280 && dest == 270)
            {
                int i = 0;
            }
            float dist;
            float dist2;

            if (amt1 > dest)
            {
                dist = amt1 - dest;
                dist2 = 360 - amt1 + dest;
                if (dist < dist2)
                {
                    amt1 -= add;
                    if (amt1 < dest) amt1 = dest;
                }
                else
                {
                    amt1 += add;
                    if (amt1 > 360) amt1 = amt1 - 360;
                }
            }
            else
            {
                dist =dest - amt1;
                dist2 = 360 - dest + amt1;

                if (dist < dist2)
                {
                    amt1 += add;
                    if (amt1 > dest) amt1 = dest;
                }
                else
                {
                    amt1 -= add;
                    if (amt1 < 0) amt1 = 360 - Math.Abs(amt1);
                }
            }

            return amt1;
        }


        //the following function gets called automatically and advances the ship along the predermined path.

        /// <summary>
        /// This is a little strange so I'll explain a bit here
        /// The Lerp formula is defined as:
        /// value1 + (value2 - value1) * amount
        /// This means that an amount (the rate value) of 0 will return value1 and an amount of 1 returns value2
        /// What I was doing before was simply moving to that position with 
        /// </summary>
        public void advance()
        {
            
            Vector3 newvector = Vector3.Forward;
            //newvector = Vector3.Transform(newvector, Matrix.CreateFromQuaternion(newrotation));
            newvector = Vector3.Transform(newvector, this.CreateLockOn(TargetPosition, this.shipCenterPosition));

            newvector.Normalize();
            this.shipCenterPosition += newvector * ingameadvancespeed * elapsedtimemultiplier;
        }

        public static float MoveTo(float value, float destination, float speed)
        {
            float amounttomove = speed;
            if (value != destination)
            {
                if (value > destination)
                {
                    if (MathHelper.Distance(value, destination) > amounttomove)
                    {
                        value -= amounttomove;
                    }
                    else
                    {
                        value= destination;
                    }

                }
                else
                {
                    if (MathHelper.Distance(value, destination) > amounttomove)
                    {
                        value += amounttomove;
                    }
                    else
                    {
                        value = destination;
                    }
                }
            }
            return value;
        }



        public override void MoveXAxis(float amount)
        {
            bool canmove = true;
            if (amount < 0 && offsetPosition.X > XMaxDistFromCenter) canmove = false;
            if (amount > 0 && offsetPosition.X < -XMaxDistFromCenter) canmove = false;
            float RotationToReach = MathHelper.ToRadians(30) * amount;
            ismovingYAxis = true;
            
            if (canmove)
            {
                
                offsetPosition += new Vector3(-1.3f, 0, 0) * amount * elapsedtimemultiplier;
                float distanceToMax = offsetPosition.X / XMaxDistFromCenter * -1;
                RotationToReach = MathHelper.ToRadians(30 * distanceToMax);
                this.drawrotation.Y = RotationToReach;
                if (amount != 0)
                {
                    float TempRotation = drawrotation.Z;
                    float sign = amount / Math.Abs(amount);
                    TempRotation += (MathHelper.ToRadians(5*sign) );
                    TempRotation = MathHelper.Clamp(TempRotation, MathHelper.ToRadians(-50), MathHelper.ToRadians(50));
                    drawrotation.Z = TempRotation;
                }
            }
            
        }

        public override void MoveYAxis(float amount)
        {
            bool canmove = true;
            //ismovingYAxis = true;
            if (amount < 0 && offsetPosition.Y > 50) canmove = false;
            if (amount > 0 && offsetPosition.Y < -20) canmove = false;
            float RotationToReach = MathHelper.ToRadians(15) * amount;
            
            if (canmove)
            {
                offsetPosition += new Vector3(0, -1, 0) * amount * elapsedtimemultiplier;
                float distanceToMax = offsetPosition.Y / YMaxDistFromCenter;
                RotationToReach = MathHelper.ToRadians(15) * distanceToMax;
                drawrotation.X = RotationToReach;
               
            }
      
        }
        public override void LockAxis(float amount)
        {
            if (amount > 0)
            {
                axislocked = true;
            }
            else
                axislocked = false;
        }



        /// <summary>
        /// The following functions move the ship. Since the ship is on rails, the following functions basically move
        /// move the ship left and right in respect to the camera (strafe left, right, up, down) like starfox
        /// </summary>
        #region movement functions


        #endregion

        public override Matrix Update(GameTime gameTime)
        {
            ingameadvancespeed = MathHelper.Lerp(ingameadvancespeed, advanceSpeed, .3f);
            elapsedtimemultiplier = gameTime.ElapsedGameTime.Milliseconds / (1000 / Game1.FPS);

            if (Vector3.Distance(ShipCenterPosition, TargetPosition) < 10)
            {
                TargetPosition = Waypoints.Dequeue();
            }
            TargettingMatrix = Matrix.Lerp(TargettingMatrix, this.CreateLockOn(TargetPosition, this.shipCenterPosition), 0.1f);

            //Caculate how the player is going to rotate (based on his movement)
            Quaternion additionalrotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), RotationValues.Y)
                * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), RotationValues.X)
            * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValues.Z);
            
            //make the players rotation equal to the new calculation
            this.newrotation = additionalrotation;

            
            if (!ismovingYAxis)
            {
                drawrotation.Y = RailShip.MoveTo(drawrotation.Y, 0f, 0.05f);
            }
            drawrotation.Z = MathHelper.Lerp(drawrotation.Z, 0, .1f);
            if (drawrotation.Z != 0)
            {
                float temprotation = drawrotation.Z;
                float sign = drawrotation.Z / Math.Abs(drawrotation.Z);
                temprotation = Math.Abs(temprotation);
                temprotation -= MathHelper.ToRadians(1);
                temprotation = MathHelper.Clamp(temprotation, 0, temprotation + MathHelper.ToRadians(1));
                drawrotation.Z = temprotation * sign;
            }
          

            ismovingYAxis = false;
          

            advance();
          
            combinedPosition = offsetPosition;
            combinedPosition = Vector3.Transform(combinedPosition, TargettingMatrix);
            this.position = combinedPosition + shipCenterPosition;
           
            base.Update(gameTime);

            ismovingXAxix = false;
            ismovingYAxis = false;

            Matrix MAT = Matrix.CreateScale(1.0f)
               * TargettingMatrix 
               * Matrix.CreateTranslation(this.shipCenterPosition);
            return MAT;
        }
        protected override void MakeLaser()
        {
            if (laserstomake > 0 && TimeToNextShot.TotalMilliseconds <= 0)
            {
                laserstomake--;

                Vector3 newrotation = new Vector3();
                newrotation.Y -= MathHelper.Pi;

                Laser newlaser = new Laser(this.Position, newrotation,  this.getDrawYawPitchRoll()*TargettingMatrix, 20f, Laser.Source.Player, Playing.LaserModel, LaserManager.IdValue);

                Playing.LaserManagement.AddLaser(newlaser);

                TimeToNextShot = new TimeSpan(0, 0, 0, 0, 100);
            }
        }

        public override Matrix getMovementPositionRotationMatrix()
        {
            Matrix MAT;
            if (!isLooping)
            {
                MAT = Matrix.CreateScale(1.0f)

              * TargettingMatrix
              * Matrix.CreateTranslation(Position);
            }
            else
            {
                MAT = SavedMatrix;
            }
            return MAT;    
        
        }

        public override Matrix getNonMovementPositionRotationMatrix()
        {
            Matrix MAT = Matrix.CreateScale(1.0f)

              * getDrawYawPitchRoll()

            * TargettingMatrix
            * Matrix.CreateTranslation(Position);
            
            return MAT;
        }

        public override Vector3 getPointingAt()
        {
            return this.shipCenterPosition;

        }

        public override void Draw2D(SpriteBatch batch, SpriteFont font)
        {
            base.Draw2D(batch, font);
            batch.DrawString(font, offsetPosition.ToString(), Vector2.Zero, Color.Wheat);
        }


        public override Vector3 getSecondaryPosition()
        {
            return this.shipCenterPosition;
        }
    
      /*  public override void SetWaypointVars(Waypoint GotoPoint)
        {
            OriginalPosition = Position;
            OriginalRotation = Rotation;

            TargetPosition = GotoPoint.GetWaypointPosition();
            TargetRotation = GotoPoint.GetWaypointRotation();
            WaypointRate = GotoPoint.GetWaypointRate();
            WaypointTurnRate = GotoPoint.GetWaypointTurnRate();

            CurrentRate = WaypointRate;
            CurrentTurnRate = WaypointTurnRate;
        }*/

        public override string ToString()
        {
            return "Rail";
        }
    }
}
