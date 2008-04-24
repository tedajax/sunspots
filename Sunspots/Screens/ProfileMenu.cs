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
    class ProfileMenu : GameWindow
    {
        SpriteFont gameFont;
        
        Texture2D ProfileSelect;
        Vector2 ProfileSelectPosition;

        Texture2D ProfileStats;
        Vector2 ProfilesStatsPosition;

        Texture2D ProfileRight;
        Vector2 ProfileRightPosition;

        Texture2D ProfileLeft;
        Vector2 ProfileLeftPosition;

        Texture2D ProfileMenuImg;
        Vector2 ProfileMenuPosition;

        Texture2D ProfileBackground;
        Vector2 ProfileBackgroundPosition;

        Vector2 ProfileNamePosition;
        bool DrawNameInFront = false;

        List<Profile> ProfileList = new List<Profile>();
        int CurrentlySelected = 0;

        bool Left;
        bool Right;

        public ProfileMenu()
        {
            Mode = "Setup";

            ProfileMenuPosition = new Vector2(0, 0);
            ProfileBackgroundPosition = new Vector2(850, 78);

            Left = false;
            Right = false;
        }

        public override void LoadContent()
        {
            ContentManager content = WindowManager.Content;

            gameFont = content.Load<SpriteFont>("Content\\SpriteFont2");

            ProfileMenuImg = content.Load<Texture2D>("Content\\MenuContent\\profileselect");
            ProfileSelect = content.Load<Texture2D>("Content\\MenuContent\\profilename");
            ProfileStats = content.Load<Texture2D>("Content\\MenuContent\\profilestats");
            ProfileRight = content.Load<Texture2D>("Content\\MenuContent\\profileright");
            ProfileLeft = content.Load<Texture2D>("Content\\MenuContent\\profileleft");
            ProfileBackground = content.Load<Texture2D>("Content\\MenuContent\\MainMenuBG");

            if (!Directory.Exists("Profiles")) Directory.CreateDirectory("Profiles");
            string[] fileEntries = Directory.GetFiles("Profiles");
            foreach (string s in fileEntries)
            {
                if (s.Contains("PROF"))
                {
                    //profiles/
                    ProfileList.Add(new Profile(s.Substring(9, s.Length - 14)));
                }
            }
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gametime)
        {
            if (Mode == "Setup") SetUp(gametime);
            if (Mode == "Hide") Hide(gametime);
            if (Mode == "Die") Die(gametime);
        }

        private void SetUp(GameTime gameTime)
        {
            ProfileMenuPosition = Vector2.Lerp(ProfileMenuPosition, new Vector2(Game1.iWidth - ProfileMenuImg.Width, 0), .2f);
            ProfileBackgroundPosition = Vector2.Lerp(ProfileBackgroundPosition, new Vector2(0, 78), .25f);
            ProfileLeftPosition = Vector2.Lerp(ProfileLeftPosition, new Vector2(13, 95), .2f);
            ProfileRightPosition = Vector2.Lerp(ProfileRightPosition, new Vector2(331, 93), .2f);
            ProfileSelectPosition = Vector2.Lerp(ProfileSelectPosition, new Vector2(71, 88), .2f);
            ProfilesStatsPosition = Vector2.Lerp(ProfilesStatsPosition, new Vector2(16, 166), .2f);

            ProfileNamePosition = Vector2.Lerp(ProfileNamePosition, new Vector2(197, 123), .2f);

            Vector2 Movement =  WindowManager.Controls.GetMovement();
            float Forward = WindowManager.Controls.getShoot();
            float Back = WindowManager.Controls.getBoost();

            if (Movement.X < -.5f && !Left)
            {
                CurrentlySelected--;
                Left = true;
                Right = false;
                
                ProfileNamePosition = new Vector2(217, 123);
               
            }
            else if (Movement.X > .5f && !Right)
            {
                CurrentlySelected++;
                Left = false;
                Right = true;
                ProfileNamePosition = new Vector2(177, 123);
            }
            
            if (Movement.X > -.5f && Movement.X < .5f)
            {
                Left = false;
                Right = false;
            }

            if (CurrentlySelected < 0)
                CurrentlySelected = ProfileList.Count - 1;
            if (CurrentlySelected > ProfileList.Count - 1)
                CurrentlySelected = 0;

            if (Forward == 1f)
                Mode = "Hide";
        }

        public void Hide(GameTime gameTime)
        {
            ProfileMenuPosition = Vector2.Lerp(ProfileMenuPosition, new Vector2(Game1.iWidth, 0), .2f);
            ProfileBackgroundPosition = Vector2.Lerp(ProfileBackgroundPosition, new Vector2(Game1.iWidth, 78), .25f);
            ProfileLeftPosition = Vector2.Lerp(ProfileLeftPosition, new Vector2(0 - ProfileLeft.Width, 95), .2f);
            ProfileRightPosition = Vector2.Lerp(ProfileRightPosition, new Vector2(0 - ProfileRight.Width, 93), .2f);
            ProfileSelectPosition = Vector2.Lerp(ProfileSelectPosition, new Vector2(0 - ProfileSelect.Width, 88), .2f);
            ProfilesStatsPosition = Vector2.Lerp(ProfilesStatsPosition, new Vector2(0 - ProfileStats.Width, 166), .2f);

            if (Vector2.Distance(ProfileMenuPosition, new Vector2(Game1.iWidth, 0)) < 1)
                Mode = "Die";
        }

        public override void Draw(GameTime gameTime)
        {
            WindowManager.SpriteBatch.Begin();

            WindowManager.SpriteBatch.Draw(ProfileBackground, ProfileBackgroundPosition, Color.White);
            WindowManager.SpriteBatch.Draw(ProfileMenuImg, ProfileMenuPosition, Color.White);
            WindowManager.SpriteBatch.Draw(ProfileSelect, ProfileSelectPosition, Color.White);
            WindowManager.SpriteBatch.Draw(ProfileStats, ProfilesStatsPosition, Color.White);

            if (Left)
                WindowManager.SpriteBatch.Draw(ProfileLeft, ProfileLeftPosition, Color.White);
            else
                WindowManager.SpriteBatch.Draw(ProfileLeft, ProfileLeftPosition, new Color(128, 128, 128));

            if (Right)
                WindowManager.SpriteBatch.Draw(ProfileRight, ProfileRightPosition, Color.White);
            else
                WindowManager.SpriteBatch.Draw(ProfileRight, ProfileRightPosition, new Color(128, 128, 128));

            WindowManager.SpriteBatch.DrawString(gameFont,
                                                 ProfileList[CurrentlySelected].getName(),
                                                 ProfileNamePosition,
                                                 Color.Black,
                                                 0f,
                                                 gameFont.MeasureString(ProfileList[CurrentlySelected].getName()) / 2,
                                                 Vector2.One,
                                                 SpriteEffects.None,
                                                 1f);

            WindowManager.SpriteBatch.End();
        }

        public void Die(GameTime gameTime)
        {
            UnloadContent();
            NetClientClass.SelectedProfile = ProfileList[CurrentlySelected];
            WindowManager.AddScreen(new MainMenu());
            WindowManager.removeScreen(this);
        }
    }
}
