#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Tanks
{
    class MenuItem
    {

        Vector2 TargetPositon;
        Vector2 Position;
        Color color;
        String Name;
        float bounce;
        bool Selected;
        float internalvalue;

        public Boolean loaded;

        public MenuItem()
        {
            TargetPositon = Vector2.Zero;
            color = Color.White;
            Name = " ";
            bounce = 0;
            Selected = false;
            loaded = false;
        }

        public MenuItem(Vector2 Pos, String name)
        {
            TargetPositon = Pos;
            Position.Y = Pos.Y;
            Name = name;
            color = Color.White;
            Selected = false;
            bounce = 0;
            loaded = false;
        }

        public void Update(double totalSeconds)
        {
            if (Math.Abs(TargetPositon.X - Position.X) < 0.5  )
            {
                if (loaded == false) loaded = true;
                if (Selected)
                {

                    bounce = (float)Math.Sin((totalSeconds * 5) + internalvalue);
                    bounce = (float)Math.Abs(bounce) * 0.3f;
                }
                else
                {
                    if (bounce > 0)
                    {
                        bounce = (float)Math.Sin((totalSeconds * 5) + internalvalue);
                        bounce = (float)Math.Abs(bounce) * 0.3f;
                        if (bounce < .05) bounce = 0;
                    }
                }
            }
            else
            {
                Position.X = MathHelper.SmoothStep(Position.X, TargetPositon.X, 0.15f);
                //Position.Y = MathHelper.SmoothStep(Position.Y, TargetPositon.Y, 0.1f);
                if (Selected) internalvalue = (float)(MathHelper.Pi - (totalSeconds * 5));
            }
        }

        public Boolean Die(double totalSeconds)
        {
            if (Position.X <= -50)
            {
                return true;
            }
            else
            {
                Position.X = MathHelper.SmoothStep(Position.X, -100, 0.10f);
                return false;

            }
        }



        public void SetSelected(bool isSelected, double totalSeconds)
        {
            Selected = isSelected;
            if (Selected)
            {

                internalvalue = (float)(MathHelper.Pi - (totalSeconds * 5));
            }
                
            
        }

        public void Draw(SpriteBatch batch, SpriteFont gameFont)
        {
           
                batch.DrawString(gameFont, Name, new Vector2(Position.X, Position.Y - (bounce * 100f)), color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            
        }

        public String getName() { return Name; }

       
        

        
    }
}
