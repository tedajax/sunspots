using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    class TextboxManager
    {
        public List<TextBox> textboxes;

        public TextboxManager()
        {
            textboxes = new List<TextBox>();
        }
        public void AddTextBox(TextBox newbox)
        {
            textboxes.Add(newbox);
        }
        public void Update(TimeSpan ElapsedGametime, KeyboardState keystate)
        {
            for (int i = 0; i < textboxes.Count; i++)
            {
                TextBox currentbox = textboxes[i];
                if (currentbox.Mode == "Dead")
                {
                    textboxes.Remove(currentbox);
                    currentbox = null;
                }
                else
                {
                    currentbox.Run(ElapsedGametime, keystate);
                }
            }
        }

        public void Draw(SpriteBatch batch, SpriteFont font)
        {
            foreach (TextBox t in textboxes)
            {
                t.Draw(batch, font);
            }
        }
    }
}
