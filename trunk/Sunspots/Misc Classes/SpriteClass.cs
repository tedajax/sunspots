using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    class SpriteClass
    {
        Texture2D Sprite;
        Vector2 Position;
        SpriteEffects Effects;
        Color Color;

        public Texture2D sprite
        {
            get { return this.Sprite; }
        }

        public Color color
        {
            get { return this.Color; }
            internal set { Color = value; }
        }

        public SpriteEffects effects
        {
            get { return this.Effects; }
            internal set { Effects = value; }
        }

        public Vector2 position
        {
            get { return this.Position; }
            internal set { Position = value; }
        }

        public SpriteClass(Texture2D Sprite)
        {
            this.Sprite = Sprite;
            SetUp();
        }
        public SpriteClass(Texture2D Sprite, Vector2 Position, Color Color, SpriteEffects effects)
        {
            this.Sprite = Sprite;
            SetUp();
            this.Position = Position;
            this.Color = Color;
            this.Effects = effects;
        }

        private void SetUp()
        {
            this.Position = new Vector2();
            this.Effects = SpriteEffects.None;
            this.Color = Color.White;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Sprite, this.Position, null, Color, 0f, Vector2.Zero, 1f, Effects, 0);

        }

        

        
    }
}
