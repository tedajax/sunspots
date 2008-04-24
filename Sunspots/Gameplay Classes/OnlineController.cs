using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Library.Network.Xna;
using Lidgren.Library.Network;

namespace StarForce_PendingTitle_
{
    class OnlineController : Controller
    {
        OnlinePlayer MyPlayer;
        private const float lerpspeed = .25f;

        Vector3 readposition = new Vector3();
        Vector3 readrotation = new Vector3();
        public OnlineController(OnlinePlayer player, MainShip ship)
        {
            this.MyPlayer = player;
            this.setUp(ship);
        }

        public override void update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState newstate, Microsoft.Xna.Framework.Input.KeyboardState oldstate)
        {
            //read unread messages and update positional data
            NetMessage latestmessage = null;
            latestmessage= MyPlayer.getLatestMessage();
            if (latestmessage != null)
            {
                readposition = XnaSerialization.ReadVector3(latestmessage);
                readrotation = XnaSerialization.ReadVector3(latestmessage);
            }
                
            Vector3 oldposition = this.mainShip.Position;
            Vector3 oldrotation = this.mainShip.rotationValues;
            //position the player based on this message
           
            //lerp to the new positions
            Vector3 newposition = new Vector3();
            Vector3 newrotation = new Vector3();
            newposition.X = MathHelper.Lerp(readposition.X, oldposition.X, lerpspeed);
            newposition.Y = MathHelper.Lerp(readposition.Y, oldposition.Y, lerpspeed);
            newposition.Z = MathHelper.Lerp(readposition.Z, oldposition.Z, lerpspeed);
            //lerp to the new rotation
            
            newrotation.X = MathHelper.Lerp(readrotation.X, oldrotation.X, lerpspeed);
            newrotation.Y = MathHelper.Lerp(readrotation.Y, oldrotation.Y, lerpspeed);
            newrotation.Z = MathHelper.Lerp(readrotation.Z, oldrotation.Z, lerpspeed);

            mainShip.rotationValues = newrotation;

            this.mainShip.Position = newposition;

            mainShip.Update(gameTime);
        }
        public override void Draw(Matrix ViewMatrix)
        {
            mainShip.DisplayModelDebug(ViewMatrix);
        }

      


    }
}
