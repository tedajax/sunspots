using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    public abstract class Controller
    {
        protected MainShip mainShip;

        public virtual void setUp(MainShip ship) { this.mainShip = ship; }

        public virtual void update(GameTime gameTime, KeyboardState newstate, KeyboardState oldstate) { }

        public virtual void Draw(Matrix ViewMatrix) { mainShip.DisplayModelDebug(ViewMatrix); }

        public virtual void DebugDraw(Matrix ViewMatrix, SpriteFont font, SpriteBatch batch, string Technique)
        {
            mainShip.DisplayModelDebug(ViewMatrix,Technique, batch, font);
        }

        public virtual void DebugDraw(Matrix ViewMatrix, SpriteFont font, SpriteBatch batch, Effect ce)
        {
            mainShip.DisplayModelDebug(ViewMatrix, batch, font, ce);
        }

        public virtual void DrawText(SpriteFont font, SpriteBatch batch)
        {
          
        }

        public MainShip MainShip
        {
            get { return this.mainShip; }
            internal set { this.mainShip = value; }
        }

    }
}
