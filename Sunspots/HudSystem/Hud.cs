using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace StarForce_PendingTitle_
{
    public class Hud
    {
        float Health;
        float HealthToReach;

        float Heat;
        /// <summary>
        /// HealthBox is the box that sorrounds the bar
        /// </summary>
        Texture2D HealthBox;
        /// <summary>
        /// health bar is the bar that sits inside health, it gets bigger and smaller
        /// </summary>
        Texture2D HealthBar;

        Texture2D Heatbox;

        Texture2D HeatBar;
        /// <summary>
        /// the actual score in the game at this moment
        /// </summary>
        double RealScore;
        /// <summary>
        /// the score that you are displaying to the user
        /// this score lerps to the correct value
        /// </summary>
        double displayscore;

        /// <summary>
        /// This is the Combo Score in actuality
        /// </summary>
        double RealComboScore;
        /// <summary>
        /// the score that you are displaying to the user
        /// </summary>
        double displayComboScore;
        /// <summary>
        /// this is the value combo is multiplied by
        /// </summary>
        double ComboMultiplier;
        /// <summary>
        /// the opacity at which we should draw the combo values
        /// allows for fading in and fading out
        /// </summary>
        float ComboOpacity;

        ///How fast should the healthbar catch up with the actual value
        const float HealthStep = .1f;

        Vector2 HealthPosition = new Vector2(0, 0);
        Vector2 HeatPosition = new Vector2(800 - 227, 0);
        Vector2 ScorePosition = new Vector2(400, 0);
        const float ComboOpacityIncrease = 20f;
        const float ComboOpacityDecrease = 20f;

        const double ScoreIncValue =  5f;


        public bool hasComboCaughtUp()
        {
            return RealScore == displayscore;
        }

        public Hud()
        {
            Health = 100;
            HealthToReach = 100;
            RealScore = 0f;
            displayscore = 0f;
            RealComboScore = 0f;
            displayComboScore = 0f;
            ComboOpacity = 0f;
            Heat = 0f;

        }

        public void LoadContent(ContentManager Content)
        {
           HealthBox =  Content.Load<Texture2D>("Content\\Hud\\health");
           HealthBar =  Content.Load<Texture2D>("Content\\Hud\\healthbar");
           Heatbox = Content.Load<Texture2D>("Content\\Hud\\heat");
           HeatBar = Content.Load<Texture2D>("Content\\Hud\\heatbar");
        }

        public void UpdateHealth(float CurVal)
        {
            HealthToReach = CurVal;
        }
        public void UpdateHeat(float CurVal)
        {
            Heat = CurVal;
        }
        public void UpdateScore(double Score, double ComboScore, double ComboMult)
        {
            RealScore = Score;
            RealComboScore = ComboScore;
            this.ComboMultiplier = ComboMult;
        }

        public void Update()
        {
            Health = MathHelper.Lerp(Health, HealthToReach, HealthStep);
            displayscore += ScoreIncValue;
            displayscore = Math.Min(displayscore, RealScore);
            if (RealComboScore == 0)
            {
                displayComboScore = 0;
                ComboOpacity -= ComboOpacityDecrease;
                ComboOpacity = MathHelper.Max(0, ComboOpacity);
            }
            else
            {
                displayComboScore += ScoreIncValue;
                displayComboScore = Math.Min(displayComboScore, RealComboScore);
                ComboOpacity += ComboOpacityIncrease;
                ComboOpacity = MathHelper.Min(255, ComboOpacity);
            }
            //if (Playing.MissionComplete)
            //{
             //   ScorePosition.Y = MathHelper.Lerp(ScorePosition.Y, 320, .1f);
            //}

        }

        public void Draw(SpriteBatch batch, SpriteFont font)
        {

            batch.Draw(HealthBox, HealthPosition, Color.White);
            batch.Draw(HealthBar, HealthPosition + new Vector2(5, 20), new Rectangle(0, 0, (int)(Health / 100 * HealthBar.Width), HealthBar.Height), Color.White);
            batch.Draw(Heatbox, HeatPosition, Color.White);
            batch.Draw(HeatBar, HeatPosition + new Vector2(55,4), new Rectangle(0, 0, (int)(Heat / 100 * HeatBar.Width), HeatBar.Height), Color.White);
            int displayval = (int)displayscore;
            String TextToDraw = "Score : " + displayval;
            Vector2 Size = font.MeasureString(TextToDraw);
            batch.DrawString(font, TextToDraw, ScorePosition - new Vector2(Size.X / 2, 0), Color.Black);
            TextToDraw = "Combo : " + (int)this.displayComboScore + "X" + this.ComboMultiplier;
            Size = font.MeasureString(TextToDraw);
            batch.DrawString(font, TextToDraw, ScorePosition + new Vector2(-Size.X / 2, Size.Y), new Color(0,0,0,(byte)ComboOpacity));
        }

    }
}
