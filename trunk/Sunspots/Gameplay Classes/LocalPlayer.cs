using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class LocalPlayer :  Controller
    {
        const float CameraXZdistance = 105f;
        //const float CameraXZdistance = 185f;
        const float CameraYdistance = 17;
        //const float CameraYdistance = 99;
        const float catchupvalue = .2f;

        private const int MillisecondsToNextUpdate = 100;
        const float MillisecondsForCombo = 5000;
        private TimeSpan TimeTillOnlineUpdate;

        Vector2 OldBanking;
        float OldShooting;
        float OldBoosting;
        float OldMissle;

        /// <summary>
        /// keep track of the score the player has accumulated
        /// </summary>
        double score;

        double ComboValue;

        double ComboMultiplier;

        TimeSpan ComboTimeSpan;

        bool ComboStarted;

        /// <summary>
        /// gets or sets the current score for this local player
        /// </summary>
        public double Score
        {
            get { return score; }
            set { score = value; }
        }

        public Hud HUD
        {
            get { return this.hud; }
        }

        public MyControls MyControls
        {
            get { return this.Controls; }
        }
        /// <summary>
        /// call this function to update the score value if the player kills an enemy. Handles combo's and all
        /// </summary>
        public void KilliedEnemy(double ScoreValue)
        {
            ComboTimeSpan = new TimeSpan(0, 0, 0, 0,(int) MillisecondsForCombo);
            ComboStarted = true;
            ComboValue += ScoreValue;
            ComboMultiplier++;
        }

        //the players controls
        MyControls Controls;
        /// <summary>
        /// Hud used to draw health and other info
        /// </summary>
        Hud hud;

        protected Vector3 TargetPosition = new Vector3();

        public Vector3 TargetingPosition
        {
            get { return TargetPosition; }
            internal set { TargetPosition = value; }
        }

        public LocalPlayer(MainShip ship, MyControls controls, Hud Hud)
        {
            TimeTillOnlineUpdate = new TimeSpan(0, 0, 0, 0, MillisecondsToNextUpdate);
            Controls = controls;
            score = 0;
            ComboMultiplier = 0;
            ComboValue = 0;
            ComboTimeSpan = new TimeSpan(0, 0, 0, 0, (int)MillisecondsForCombo);
            ComboStarted = false;
            this.hud = Hud;
            this.setUp(ship);
        }

        public void handleOnline(GameTime gameTime)
        {
            if (NetClientClass.Client != null)
            {
                TimeTillOnlineUpdate -= gameTime.ElapsedGameTime;
                if (TimeTillOnlineUpdate.TotalMilliseconds <= 0)
                {
                    NetMessage updatemessage = new NetMessage();
                    updatemessage.Write((byte)2);
                    updatemessage.Write(NetClientClass.ReferenceNumber);
                    XnaSerialization.Write(updatemessage, mainShip.Position);
                    XnaSerialization.Write(updatemessage, mainShip.rotationValues);
                    //XnaSerialization.Write(updatemessage, mainShip.Rotation);
                    NetClientClass.sendMessage(updatemessage);
                    TimeTillOnlineUpdate = new TimeSpan(0, 0, 0, 0, MillisecondsToNextUpdate);
                }
            }
        }

        public override void update(GameTime gameTime, KeyboardState newstate, KeyboardState oldstate)
        {
            //----------------------------------Controls----------------------------\\
            mainShip.LockAxis(Controls.getLock());
            
            //Banking Controls
            Vector2 Banking = Controls.getBanking();

            if (OldBanking.X == 1 && !(OldBanking.Y == 1) && !(Banking.X==1))
            {
                mainShip.BarrelRollTimerLeft = new TimeSpan(0, 0, 0, 0, 150);
                mainShip.BarrelRollTimerRight = TimeSpan.Zero;
               // mainShip.OldRotation = mainShip.rotationValues;
            }
            if (OldBanking.Y == 1 && !(OldBanking.X==1) && !(Banking.Y == 1))
            {
                mainShip.BarrelRollTimerRight = new TimeSpan(0, 0, 0, 0, 150);
                mainShip.BarrelRollTimerLeft = TimeSpan.Zero;
               // mainShip.OldRotation = mainShip.rotationValues;
            }

            if (Banking.X == 1 && !(Banking.Y==1) && !mainShip.Isbarreling)
            {
                if (mainShip.BarrelRollTimerLeft.TotalMilliseconds <= 0)
                    mainShip.BankLeft();
                else
                    mainShip.BarrelLeft(MathHelper.TwoPi);
            }
            if (Banking.Y==1 && !(Banking.X==1) && !mainShip.Isbarreling)
            {
                if (mainShip.BarrelRollTimerRight.TotalMilliseconds <= 0)
                    mainShip.BankRight();
                else
                    mainShip.BarrelRight(MathHelper.TwoPi);
            }
            if (mainShip.BarrelRollTimerLeft.TotalMilliseconds > 0)
                mainShip.BarrelRollTimerLeft -= gameTime.ElapsedGameTime;
            if (mainShip.BarrelRollTimerRight.TotalMilliseconds > 0)
                mainShip.BarrelRollTimerRight -= gameTime.ElapsedGameTime;
                       

            //Movement Controls
            Vector2 MovementVectors = Controls.GetMovement();
            mainShip.MoveYAxis(MovementVectors.Y);
            mainShip.MoveXAxis(-1*MovementVectors.X);
            mainShip.Boost(gameTime, Controls.getBoost(), OldBoosting);
            

            if (Controls.getRightThumbStick().Y > .5)
            {
                mainShip.StartLoop("Loop");
            }
            if (Controls.getRightThumbStick().Y < -.5)
            {
                mainShip.StartLoop("Reverse");
            }
            if (Controls.getRightThumbStick().X < -.5)
            {
                mainShip.StartLoop("Left");
            }
            if (Controls.getRightThumbStick().X > .5)
            {
                mainShip.StartLoop("Right");
            }

            if (Controls.getMissle() != 1)
            {
                if ((Controls.getShoot() == 1) && !(OldShooting == 1))
                {
                    mainShip.createLasers(3);
                }
                if (Controls.getShoot() != 1 && mainShip.LasersToMake > 0)
                {
                    mainShip.CancelLaser();
                }
            }
            else
            {
                if (Controls.getShoot() == 1 && !(OldShooting == 1))
                {
                    if (TargetPosition == Vector3.Zero) //What are the odds of this happening?
                        mainShip.FireMissle();
                    else
                    {
                        mainShip.FireMissle(TargetPosition);
                    }
                }
            }

           
            //-----------------------------------END CONTROLS--------------------------------\\
            if (Playing.MissionComplete)
            {
                this.ComboStarted = false;
                this.score += ComboValue * ComboMultiplier;
                ComboValue = 0;
                ComboMultiplier = 0;
            }
            if (this.ComboStarted)
            {
                this.ComboTimeSpan -= gameTime.ElapsedGameTime;
                if (ComboTimeSpan.TotalSeconds <= 0)
                {
                    this.score += ComboValue * ComboMultiplier;
                    ComboStarted = false;
                    ComboValue = 0f;
                    ComboMultiplier = 0f;
                }
            }


            Matrix CameraMatrix = mainShip.Update(gameTime);

            hud.UpdateHealth(mainShip.Health);
            hud.UpdateHeat(mainShip.Heat);
            hud.UpdateScore(this.score, this.ComboValue, ComboMultiplier);
            hud.Update();
            if (mainShip.mode=="Playing") ParticleSystem.addThrusterSprite3(mainShip, this.Controls);
           
            handleOnline(gameTime);

            //Update the Camera

            Vector3 CameraPosition = CameraClass.Position;
            Vector3 OldCameraPosition = CameraPosition;

            CameraPosition.X = 0;
            CameraPosition.Y = CameraYdistance;
            CameraPosition.Z = CameraXZdistance;
            if (mainShip.IsLooping()) CameraPosition.Z = 2 * CameraXZdistance;
            CameraPosition = Vector3.Transform(CameraPosition, CameraMatrix);
            //CameraPosition = Vector3.SmoothStep(OldCameraPosition, CameraPosition, catchupvalue);
            CameraPosition = Vector3.Lerp(OldCameraPosition, CameraPosition, catchupvalue);
            Vector3 UpVector = new Vector3(0, 1, 0);
            UpVector = Vector3.Transform(UpVector,Matrix.CreateFromQuaternion(mainShip.NewRotation));


            CameraClass.Position = CameraPosition;
            CameraClass.Rotation = mainShip.NewRotation;
            CameraClass.CameraPointingAt = mainShip.getPointingAt() + new Vector3(0,CameraYdistance,0);
            //CameraClass.CameraUpVector = UpVector;
            
            CameraClass.Update();

            OldBanking = Banking;
            OldBoosting = Controls.getBoost();
            OldShooting = Controls.getShoot();
            OldMissle = Controls.getMissle();

          //  Playing.NewLevel.StaticObjects[1].setPosition(new Vector3(mainShip.Position.X, Playing.NewLevel.StaticObjects[1].getPosition().Y, mainShip.Position.Z));
        }
        public override void DebugDraw(Matrix ViewMatrix,SpriteFont font, SpriteBatch batch, string technique)
        {
            base.DebugDraw(ViewMatrix, font, batch,technique);

        }
        public override void Draw(Matrix ViewMatrix)
        {
            base.Draw(ViewMatrix);

        }

        public override void DrawText(SpriteFont font, SpriteBatch batch)
        {
            hud.Draw(batch,font);
            mainShip.Draw2D(batch, font);
        }
    }
}
