using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace StarForce_PendingTitle_
{
    /// <summary>
    /// this screen forces the player to load or create a profile
    /// </summary>
    class BackGroundScreen : GameWindow
    {
   
        SpriteFont gameFont;
        ContentManager content;


      
        Texture2D Logo;
        Vector2 LogoPosition;
        byte LogoOpacity;
        Texture2D Bars;
        Vector2 Bar1Position;
        Vector2 Bar2Position;
        
        /// <summary>
        /// Starts a new background screen that shows the red bars at the top. 
        /// </summary>
        public BackGroundScreen()
        {
            Mode = "Setup";

            Bar1Position = new Vector2(0,-100);
            Bar2Position = new Vector2(0,900);
       
        }


        public override void LoadContent()
        {
            
            
                if (content == null) content = new ContentManager(WindowManager.Game.Services);

                gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");

                Bars = WindowManager.Content.Load<Texture2D>("Content\\MenuContent\\Bars");

                Logo = WindowManager.Content.Load<Texture2D>("Content\\MenuContent\\NewLogo");

                LogoPosition = new Vector2(400 - Logo.Width / 2, 300 - Logo.Height / 2);
                LogoOpacity = 0;
            
        }

        public override void UnloadContent()
        {
            
                content.Unload();
            

        }
        public override void Update(GameTime gametime)
        {
            if (Mode == "Setup") SetUp(gametime);
            if (Mode == "Run") Run(gametime);
            if (Mode == "Hide") Hide(gametime);
            if (Mode == "Die") Die(gametime);

          
        }

        public void Hide(GameTime gameTime)
        {
            Bar1Position = Vector2.Lerp(Bar1Position, new Vector2(0, -Bars.Height), .8f);
            Bar2Position = Vector2.Lerp(Bar2Position, new Vector2(0, 600), .8f);
            LogoPosition = Vector2.Lerp(LogoPosition, new Vector2(0, -Bars.Height), .8f);
        }

        public void Run(GameTime gameTime)
        {
            
                if (Bar2Position.Y - (600 - Bars.Height) > 3 || Math.Abs(Bar1Position.Y) > 3)
                {
                    Bar1Position = Vector2.Lerp(Bar1Position, new Vector2(0, 0), .4f);
                    Bar2Position = Vector2.Lerp(Bar2Position, new Vector2(0, 531), .4f);
                    LogoPosition = Vector2.Lerp(LogoPosition, new Vector2(0, 0), .4f);
                }
                else
                {
                    LogoPosition = Vector2.Lerp(LogoPosition, new Vector2(0, 0), .4f);

                }
                

            

        }


        public void SetUp(GameTime gameTime)
        {
            if (LogoOpacity < 255)
            {
                if (WindowManager.Controls.getShoot() == 1f)
                {
                    LogoOpacity = 255;
                }
                LogoOpacity = (byte)Math.Min((int)LogoOpacity+4, 255);
                
            }
            else
            {
                if (Bar2Position.Y - (600 - Bars.Height)>3 || Math.Abs(Bar1Position.Y) > 3)
                {
                    Bar1Position = Vector2.Lerp(Bar1Position, new Vector2(0, 0), .4f);
                    Bar2Position = Vector2.Lerp(Bar2Position, new Vector2(0, 531), .4f);
                    LogoPosition = Vector2.Lerp(LogoPosition, new Vector2(0, 0), .4f);
                }
                else
                {
                    LogoPosition = Vector2.Lerp(LogoPosition, new Vector2(0, 0), .4f);
                    
                }
                if (Vector2.Distance(LogoPosition, Vector2.Zero) < 1)
                {
                    Mode = "Main";
                    WindowManager.AddScreen(new ProfileMenu());
                }

            }

        }


        public void Die(GameTime gametime)
        {
            int i = 0;

            if (i == 0)
            {
                UnloadContent();
                WindowManager.removeScreen(this);
            }

        }


        public override void Draw(GameTime gameTime)
        {
            // WindowManager.GraphicsDevice.Clear(Color.Black);


            //DrawModel(TestModel);



            WindowManager.SpriteBatch.Begin();
                WindowManager.SpriteBatch.Draw(Bars, Bar1Position, Color.White);
                WindowManager.SpriteBatch.Draw(Bars, Bar2Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);
                WindowManager.SpriteBatch.Draw(Logo, LogoPosition, new Color(255,255,255,LogoOpacity));
                    
            WindowManager.SpriteBatch.End();


        }

        public void setMode(String Mode) { this.Mode = Mode; }


    }
}

    

