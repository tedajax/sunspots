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
/*
namespace StarForce_PendingTitle_
{

    /// <summary>
    /// Railship extends mainship and allows the ship to follow a predetermined path created by the way point system
    /// TODO : IMPLEMENT WAYPOINT SYSTEM
    /// </summary>
    public class RailShipOld : MainShip
    {
        //this is the center position of the ship. Basically this is the thing that moves on rails and then ship is being pulled by this thing
        private Vector3 shipCenterPosition;
        private Vector3 cameraRotation;
        private Vector3 combinedPosition;
        private Vector3 combinedRotation;
        //Offset Position is only a vector 2 because the ship only moves on two axis', obviously this changes when the ship rotates
        //but the point is that in terms of the offset ship, it is only really moving in two axis's
        private Vector2 offsetPosition;

        private Vector3 TargetPosition;
        private Vector3 TargetRotation;
        private Vector3 OriginalPosition;
        private Vector3 OriginalRotation;
        private float WaypointRate;
        private float WaypointTurnRate;
        private float CurrentRate;
        private float CurrentTurnRate;

        //WHICH WAYPOINT?????
        private int currwaypoint = -1;

        //These are constants about the ship, change them up here and the whole gameplay will be affected
        private const float Speed = .1f;
        private const float advanceSpeed = 1;
        private const float XMaxDistFromCenter = 30f;
        private const float YMaxDistFromCenter = 30f;


        public Vector3 ShipCenterPosition
        {
            get { return this.shipCenterPosition; }
            internal set { this.shipCenterPosition = value; }
        }

        public Vector2 OffsetPosition
        {
            get { return this.offsetPosition; }
            internal set { this.offsetPosition = value; }
        }

        public RailShip(Model pmodel)
        {
            shipCenterPosition = Vector3.Zero;
            cameraRotation = Vector3.Zero;
            velocity = Vector3.Zero;
            offsetPosition = Vector2.Zero;

            Init(pmodel, 270,false);
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
            
            shipCenterPosition.X = MathHelper.Lerp(OriginalPosition.X, TargetPosition.X, CurrentRate);
            shipCenterPosition.Y = MathHelper.Lerp(OriginalPosition.Y, TargetPosition.Y, CurrentRate);
            shipCenterPosition.Z = MathHelper.Lerp(OriginalPosition.Z, TargetPosition.Z, CurrentRate);
            

            shipCenterPosition.X = NewLerp(shipCenterPosition.X, TargetPosition.X, WaypointRate);
            shipCenterPosition.Y = NewLerp(shipCenterPosition.Y, TargetPosition.Y, WaypointRate);
            shipCenterPosition.Z = NewLerp(shipCenterPosition.Z, TargetPosition.Z, WaypointRate);

            Playing.stringtodraw = cameraRotation.ToString() + "\n" + TargetRotation.ToString();
            cameraRotation.X = RotationLerp(cameraRotation.X, TargetRotation.X, WaypointTurnRate);
            cameraRotation.Y = RotationLerp(cameraRotation.Y, TargetRotation.Y, WaypointTurnRate);
            cameraRotation.Z = RotationLerp(cameraRotation.Z, TargetRotation.Z, WaypointTurnRate);
             

            CurrentRate += WaypointRate;
            CurrentRate = MathHelper.Clamp(CurrentRate, 0, 1);

            CurrentTurnRate += WaypointTurnRate;
            CurrentTurnRate = MathHelper.Clamp(CurrentTurnRate, 0, 1);
        }



        /// <summary>
        /// The following functions move the ship. Since the ship is on rails, the following functions basically move
        /// move the ship left and right in respect to the camera (strafe left, right, up, down) like starfox
        /// </summary>
        #region movement functions
        public override void MoveUp()
        {
            base.MoveUp();

            Vector2 newvector = new Vector2(0, 1);
            this.offsetPosition += newvector;
            
            if (RightLeftPress)
                maxrotation.X = 20f;
            else
                maxrotation.X = 40f;
        }

        public override void MoveDown()
        {
            base.MoveDown();

            Vector2 newvector = new Vector2(0, -1);
            this.offsetPosition += newvector;

            if (RightLeftPress)
                maxrotation.X = -20f;
            else
                maxrotation.X = -40f;
        }

        public override void MoveLeft()
        {
            base.MoveLeft();

            Vector2 newvector = new Vector2(-1, 0);
            this.offsetPosition += newvector; 

            if (UpDownPress)
                maxrotation.Z = 40f;
            else
                maxrotation.Z = 80f;
        }
        public override void MoveRight()
        {
            base.MoveRight();

            Vector2 newvector = new Vector2(1, 0);
            this.offsetPosition += newvector;

            if (UpDownPress)
                maxrotation.Z = -40f;
            else
                maxrotation.Z = -80f;
        }

        public override void NoLeftRight()
        {
            base.NoLeftRight();

            maxrotation.Z = 0f;
        }

        public override void NoUpDown()
        {
            base.NoUpDown();

            maxrotation.X = 0f;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            float WaypointDistance =DistanceFromTarget();
            int cwaypnt = GetCurrentWaypoint();
            float cwaypntrate;
            if (cwaypnt >= 0)
                cwaypntrate = Level.Waypoints[cwaypnt].GetWaypointRate();
            else
                cwaypntrate = 1f;

            if (WaypointDistance <= cwaypntrate)
            {
                SetCurrentWaypoint(cwaypnt + 1);
                cwaypnt++;

                if (cwaypnt > Level.Waypoints.Count - 1)
                {
                    SetCurrentWaypoint(0);
                    cwaypnt = 0;
                }

                SetWaypointVars(Level.Waypoints[cwaypnt]);
            }
                 

            shipCenterPosition = this.position;
            cameraRotation = this.rotation;

            advance();
            this.combinedPosition = new Vector3(this.shipCenterPosition.X + ((float)Math.Cos(MathHelper.ToRadians(rotation.Y+90)) * offsetPosition.X), this.shipCenterPosition.Y + this.offsetPosition.Y, this.shipCenterPosition.Z + ((float)Math.Sin(MathHelper.ToRadians(this.rotation.Y+90)) * this.offsetPosition.X));
            this.combinedRotation = new Vector3(this.cameraRotation.X + this.rotation.X, this.cameraRotation.Y + this.rotation.Y, this.cameraRotation.Z + this.rotation.Z);

            this.position = shipCenterPosition;
            this.rotation = cameraRotation;

            float NewZRotation = MathHelper.SmoothStep(this.Rotation.Z, this.MaxRotation.Z, .1f);
            float NewXRotation = MathHelper.SmoothStep(this.Rotation.X, this.MaxRotation.X, .1f);
            this.SetRotation(new Vector3(NewXRotation, this.Rotation.Y, NewZRotation));

            //velocity *= .95f;
            //position += velocity;
            //this.RealPosition = new Vector3((float)Math.Cos(MathHelper.ToRadians(rotation.Y)) * position.X, position.Y,(float) Math.Sin(MathHelper.ToRadians(rotation.Y)) * position.Z);
            if (this.offsetPosition.Y > YMaxDistFromCenter) this.offsetPosition.Y = YMaxDistFromCenter;
            if (this.offsetPosition.Y < -YMaxDistFromCenter) this.offsetPosition.Y = -YMaxDistFromCenter;
            if (this.offsetPosition.X > XMaxDistFromCenter) this.offsetPosition.X = XMaxDistFromCenter;
            if (this.offsetPosition.X < -XMaxDistFromCenter) this.offsetPosition.X = -XMaxDistFromCenter;
        }

        public override void Draw()
        {
            //DisplayModel(this.shipCenterPosition);
            DisplayModel(combinedPosition, combinedRotation);
        }

        public override float DistanceFromTarget()
        {
            return (float)(Math.Sqrt(Math.Pow((double)(TargetPosition.X - shipCenterPosition.X), 2) + Math.Pow((double)(TargetPosition.Y - shipCenterPosition.Y), 2) + Math.Pow((double)(TargetPosition.Z - shipCenterPosition.Z), 2)));
        }

        public override int GetCurrentWaypoint() { return currwaypoint; }
        public override void SetCurrentWaypoint(int cwp)
        {
            currwaypoint = cwp;
        }

        public override void SetWaypointVars(Waypoint GotoPoint)
        {
            OriginalPosition = Position;
            OriginalRotation = Rotation;

            TargetPosition = GotoPoint.GetWaypointPosition();
            TargetRotation = GotoPoint.GetWaypointRotation();
            WaypointRate = GotoPoint.GetWaypointRate();
            WaypointTurnRate = GotoPoint.GetWaypointTurnRate();

            CurrentRate = WaypointRate;
            CurrentTurnRate = WaypointTurnRate;
        }

        public override string ToString()
        {
            return "Rail";
        }
    }
}
*/