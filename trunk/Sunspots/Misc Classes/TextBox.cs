using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarForce_PendingTitle_
{
    class TextBox
    {
        private List<String> TextToDisplay;
        private Vector2 Position;
        private byte Opacity;
        public String Mode;
        private String CloseMethod;
        private bool Center;
        private float FadeSpeed;
        public String Name;
        private Color TextColor;


        public TextBox(string Name, string texttoDisplay, Vector2 Position, byte StartingOpacity, float FadeSpeed, bool Center)
        {
            Initialize(texttoDisplay, Position, StartingOpacity, Color.White);
            CloseMethod = "AnyKey";
            this.Center = Center;
            this.FadeSpeed = FadeSpeed/16;
            this.Name = Name;
        }

        public TextBox(string Name, string texttoDisplay, Vector2 Position, byte StartingOpacity, float FadeSpeed, bool Center, String CloseMethod)
        {
            Initialize(texttoDisplay, Position, StartingOpacity, Color.White);
            this.Center = Center;
            this.FadeSpeed = FadeSpeed / 16;
            this.Name = Name;
            this.CloseMethod = CloseMethod;
        }

        public void setCloseMethod(String CloseMeth)
        {
            CloseMethod = CloseMeth;
        }

        public void Initialize(string textToDisplay, Vector2 Position, byte StartingOpacity, Color StartingColor)
        {
            TextToDisplay = new List<string>();
            String TempString = "";
            for (int i = 0; i < textToDisplay.Length; i++)
            {
                if (textToDisplay[i] == '\n')
                {
                    this.TextToDisplay.Add(TempString);
                    TempString = "";
                }
                else
                    TempString += textToDisplay[i];
       
            }
            this.TextToDisplay.Add(TempString);
            
            this.Position = Position;
            this.Opacity = 255;
            Opacity = StartingOpacity;
            Mode = "Load";
            this.TextColor = StartingColor;
        }

        public void setColor(Color newColor) { TextColor = newColor; }


        public void Run(TimeSpan ElapsedGametime, KeyboardState keystate)
        {
            if (Mode == "Load")
            {
                int tempopacity = Opacity;
                tempopacity += (byte)(ElapsedGametime.Milliseconds * FadeSpeed);
                if (tempopacity >= 255)
                {
                    tempopacity = 255;
                    Mode = "Run";
                }
                Opacity = (byte)tempopacity;
            }
            if (Mode == "Run")
            {
                if (CloseMethod == "AnyKey")
                {
                    if (keystate.GetPressedKeys().Length > 0)
                    {
                        Mode = "Die";
                    }
                }
            }

            if (Mode == "Die")
            {
                if (Opacity > 0)
                {
                    int tempopacity = Opacity;
                    tempopacity -= (byte)(ElapsedGametime.Milliseconds * FadeSpeed);
                    if (tempopacity <= 0)
                    {
                        tempopacity = 0;
                        Mode = "Dead";
                    }
                    Opacity = (byte)tempopacity;
                }

            }

        }

        public void Draw(SpriteBatch drawbatch, SpriteFont drawfont)
        {
            for (int i = 0; i < TextToDisplay.Count; i++)
            {
                Vector2 AdjustmentVector = new Vector2();
                if (Center)
                {
                    AdjustmentVector = drawfont.MeasureString(TextToDisplay[i]) / 2;
                    AdjustmentVector.Y += 10;
                }
                drawbatch.DrawString(drawfont, TextToDisplay[i], new Vector2(Position.X - AdjustmentVector.X,Position.Y - ((TextToDisplay.Count-i)*AdjustmentVector.Y)), new Color(TextColor.R,TextColor.G,TextColor.B, Opacity));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
            



    }
}
