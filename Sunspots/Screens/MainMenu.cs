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
    class MainMenu : GameWindow
    {
   
        SpriteFont gameFont;
        ContentManager content;
        
        Texture2D MenuImage;
        Vector2 MenuImagePosition;

        Texture2D MenuBackGround;
        Vector2 MenuBGPosition;

        List<SpriteClass> MenuItems;
        SpriteClass SelectedClass;

        Vector2 OldMovement = new Vector2();
        
        /// <summary>
        /// Starts the new main menu..
        /// </summary>
        public MainMenu()
        {
            Mode = "Setup";

            MenuImagePosition = new Vector2( 0,0);
            MenuBGPosition = new Vector2(850, 78);
            MenuItems = new List<SpriteClass>();
        }
        //14,93

        public override void LoadContent()
        {
            ContentManager content = WindowManager.Content;
      
            gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
            
            MenuImage = content.Load<Texture2D>("Content\\MenuContent\\ModeSelect");

            MenuBackGround = content.Load<Texture2D>("Content\\MenuContent\\MainMenuBG");

            MenuItems.Add(new SpriteClass(content.Load<Texture2D>("Content\\MenuContent\\solo"), new Vector2(-200,93), Color.White, SpriteEffects.None));
            MenuItems.Add(new SpriteClass(content.Load<Texture2D>("Content\\MenuContent\\multi"), new Vector2(-200, 264), Color.White, SpriteEffects.None));
            MenuItems.Add(new SpriteClass(content.Load<Texture2D>("Content\\MenuContent\\options"), new Vector2(-200, 420), Color.White, SpriteEffects.None));
            SelectedClass = MenuItems[0];
        }

        public override void UnloadContent()
        {
            
                

        }

        //156,264
        //404,420
        public override void Update(GameTime gametime)
        {
            if (Mode == "Setup") SetUp(gametime);
            if (Mode == "Hide") Hide(gametime);
            if (Mode == "Die") Die(gametime);
        }

        TimeSpan ElapsedTimeSpan = new TimeSpan(0, 0, 3);
        GameWindow ScreentoAdd;
        int PositionToAdd;
        public void Hide(GameTime gameTime)
        {
            MenuImagePosition = Vector2.Lerp(MenuImagePosition, new Vector2(800, 0), .3f);
            MenuBGPosition = Vector2.Lerp(MenuBGPosition, new Vector2(800, 78), .5f);
            MenuItems[0].position = Vector2.Lerp(MenuItems[0].position, new Vector2(-MenuItems[0].sprite.Width, 93), .3f);
            MenuItems[1].position = Vector2.Lerp(MenuItems[1].position, new Vector2(-MenuItems[1].sprite.Width, 264), .3f);
            MenuItems[2].position = Vector2.Lerp(MenuItems[2].position, new Vector2(-MenuItems[2].sprite.Width, 420), .3f);

            ElapsedTimeSpan -= gameTime.ElapsedGameTime;

            if (Math.Abs(MenuItems[2].position.X - -MenuItems[2].sprite.Width) <=5)
            {
                WindowManager.AddScreen(ScreentoAdd, PositionToAdd);
                WindowManager.removeScreen(this);
            }
        }



        public void SetUp(GameTime gameTime)
        {
            MenuImagePosition = Vector2.Lerp(MenuImagePosition, new Vector2(800 - MenuImage.Width,0), .2f);
            MenuBGPosition = Vector2.Lerp(MenuBGPosition, new Vector2(0, 78), .25f);
            MenuItems[0].position = Vector2.Lerp(MenuItems[0].position, new Vector2(14, 93), .15f);
            MenuItems[1].position = Vector2.Lerp(MenuItems[1].position, new Vector2(156, 264), .15f);
            MenuItems[2].position = Vector2.Lerp(MenuItems[2].position, new Vector2(404, 420), .15f);

            Vector2 Movement = WindowManager.Controls.GetMovement();
            float Button = WindowManager.Controls.getShoot();
            float Back = WindowManager.Controls.getMissle();

            int IndexOf = MenuItems.IndexOf(SelectedClass);
            
            if (Math.Abs(Movement.Y) >.5 && Math.Abs(OldMovement.Y) < .5)
            {
                int addvalue = 1;
                if (Movement.Y < 0) addvalue = -1;

                IndexOf += addvalue;
            }
            if (IndexOf < 0) IndexOf = MenuItems.Count - 1;
            if (IndexOf == MenuItems.Count) IndexOf = 0;

            if (Button == 1f)
            {
                if (IndexOf == 0)
                {
                    ScreentoAdd = new SystemSelect();
                    PositionToAdd = WindowManager.FindLastScreenPosition() - 1;
                }
                if (IndexOf == 1)
                {
                    ScreentoAdd = new LobbyScreen();
                    PositionToAdd = WindowManager.FindLastScreenPosition();
                }
                if (IndexOf == 2)
                {
                    ScreentoAdd = new Options();
                    PositionToAdd = WindowManager.FindLastScreenPosition();
                }
                Mode = "Hide";
            }
            if (Back > 0)
            {
                ScreentoAdd = new ProfileMenu();
                PositionToAdd = WindowManager.FindLastScreenPosition();
                Mode = "Hide";
            }


            SelectedClass = MenuItems[IndexOf];
            OldMovement = Movement;
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
            WindowManager.SpriteBatch.Draw(MenuImage, MenuImagePosition, Color.White);
            WindowManager.SpriteBatch.Draw(MenuBackGround, MenuBGPosition, Color.White);
            foreach (SpriteClass c in MenuItems)
            {
                if (c == SelectedClass)
                {
                    c.color = Color.White;
                }
                else
                {
                    c.color = new Color(80,80,80);
                }
                c.Draw(WindowManager.SpriteBatch);

            }
                    
            WindowManager.SpriteBatch.End();


        }

        public void setMode(String Mode) { this.Mode = Mode; }


    }
}

    

