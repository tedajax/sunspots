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
    class ProfileScreen : GameWindow
    {
   
        SpriteFont gameFont;
        ContentManager content;


      
        Texture2D Background;
        //texture used to select buttons on this screen
        Texture2D SelectButton;
        //texture used to select profiles
        Texture2D profileSelectTex;
        //texture used to show profile is clicked
        Texture2D profileClickedTex;


        KeyboardTyping KeyboardHandle;
        MouseState MouseOldstate;
        KeyboardState Oldstate;
        //List of Profiles Loaded
        List<Profile> ProfileList = new List<Profile>();

        List<MenuItem> MenuItemList = new List<MenuItem>();

        //The Selected Menu Item
        MenuItem Selected = null;
        //The Stuff They Have Typed
        String Typing;

        //Crappy Little Workaround to Select Profiles
        Profile SelectedProfile;
        //Profile they clicked on, mouse not needed to be over
        Profile ClickedProfile;


        public ProfileScreen()
        {
            Mode = "Load";
            
        }


        public override void LoadContent()
        {
            
            
                if (content == null) content = new ContentManager(WindowManager.Game.Services);

                gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");

                Background = WindowManager.Content.Load<Texture2D>("Content\\profile enter");

                SelectButton = WindowManager.Content.Load<Texture2D>("Content\\menuselect");

                profileSelectTex = WindowManager.Content.Load<Texture2D>("Content\\profileselect");

                profileClickedTex = WindowManager.Content.Load<Texture2D>("Content\\profileclicked");
        }

        public override void UnloadContent()
        {
            
                content.Unload();
            

        }
        public override void Update(GameTime gametime)
        {
            if (Mode == "Select") Select(gametime);
            if (Mode == "New") New(gametime);
            if (Mode == "Load") Load(gametime);
            if (Mode == "Die") Die(gametime);

          
        }

        public void Load(GameTime gametime)
        {
            KeyboardHandle = new KeyboardTyping(WindowManager.NewState);
            Oldstate = WindowManager.NewState;
            MouseOldstate = Mouse.GetState();
            if (!Directory.Exists("Profiles")) Directory.CreateDirectory("Profiles");
            string[] fileEntries = Directory.GetFiles("Profiles");
            foreach (string s in fileEntries)
            {
                if (s.Contains("PROF"))
                {
                    //profiles/
                    ProfileList.Add(new Profile(s.Substring(9,s.Length-14)));
                }
            }

            MenuItemList.Add(new MenuItem(new Rectangle(299,547,100,586-547),"Button1", new Vector2(300,546)));
            MenuItemList.Add(new MenuItem(new Rectangle(441,546,100,100),"Button2", new Vector2(442,547)));

            Mode = "Select";

        }


        public MenuItem updateMenuItems(Vector2 MousePositon)
        {

            foreach (MenuItem m in MenuItemList)
            {
                if (m.isMouseOver(MousePositon)) return m;
            }
            return null;
        }


        public Profile selectProfile(Vector2 mouseXY)
        {
            float y = 130f;
            foreach (Profile f in ProfileList)
            {
                if (mouseXY.X > 295 && mouseXY.X < 543 && mouseXY.Y > y-19.5 && mouseXY.Y < y + 19.5)
                {
                    return f;
                }
                y += 50;
            }
            return null;
        }


        public void Select(GameTime gametime)
        {
            KeyboardState newstate = WindowManager.NewState;
            MouseState mousestate = Mouse.GetState();
            Vector2 mouseXY = new Vector2(mousestate.X, mousestate.Y);


            if (mousestate.LeftButton == ButtonState.Pressed)
            {
                SelectedProfile = selectProfile(mouseXY);
                Selected = updateMenuItems(mouseXY);
            }

            if (mousestate.LeftButton == ButtonState.Pressed && MouseOldstate.LeftButton == ButtonState.Released)
            {
                if (Selected != null)
                {
                    if (Selected.ToString() == "Button2")
                    {
                        if (ClickedProfile != null)
                        {
                            File.Delete("Profiles\\" + ClickedProfile.getName() + ".PROF");
                            ProfileList.Remove(ClickedProfile);
                        }
                        else
                        {
                            Mode = "New";
                            KeyboardHandle.clearCurrentString();
                        }
                    }
                    if (Selected.ToString() == "Button1" && ClickedProfile != null)
                    {
                        NetClientClass.SelectedProfile = ClickedProfile;
                        //WindowManager.AddScreen(new TitleScreen(true));
                        //WindowManager.removeScreen(this);
                    }

                }
                else
                {
                    if (SelectedProfile != null)
                    {
                        ClickedProfile = SelectedProfile;
                    }
                    else
                        ClickedProfile = null;
                }
            }


            MouseOldstate = mousestate;
            Oldstate = newstate;
            // WindowManager.OldState = WindowManager.NewState;

        }

        public void New(GameTime gametime)
        {
            KeyboardState newstate = WindowManager.NewState;
            MouseState mousestate = Mouse.GetState();
            Vector2 mouseXY = new Vector2(mousestate.X, mousestate.Y);

            Typing = KeyboardHandle.update(gametime, newstate);

            Selected = updateMenuItems(mouseXY);
            if (mousestate.LeftButton == ButtonState.Pressed && MouseOldstate.LeftButton == ButtonState.Released && Selected != null )
            {
                Mode = "Select";
                if (Selected.ToString() == "Button1")
                {
                    if (!File.Exists(Typing))
                    {
                        StreamWriter stream = new StreamWriter("Profiles\\"+Typing + ".PROF");
                        stream.Write("This is a new profile");
                        stream.Close();
                        ProfileList.Add(new Profile(Typing));
                    }
                }
            }


            MouseOldstate = mousestate;
            Oldstate = newstate;
            // WindowManager.OldState = WindowManager.NewState;

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

            WindowManager.SpriteBatch.Draw(Background, Vector2.Zero, Color.White);
            String Button1 = "Select";
            
            String Button2 = "New";
            if (ClickedProfile != null) Button2 = "Delete";
            String TopBar = "Select A Profile";
            if (Mode == "New")
            {
                Button1 = "Create";
                Button2 = "Cancel";
                TopBar = "Create A Profile";
            }
            int x = 326;
            int y = 130;
            if (Mode == "Select")
            {
                foreach (Profile f in ProfileList)
                {
                    
                    WindowManager.SpriteBatch.DrawString(gameFont, f.getName(), new Vector2(x, y), Color.White);
                    if (SelectedProfile == f) WindowManager.SpriteBatch.Draw(profileSelectTex, new Vector2(295, y-10), Color.White);
                    if (ClickedProfile == f) WindowManager.SpriteBatch.Draw(profileClickedTex, new Vector2(295, y - 10), Color.Green);
                    
                    y += 50;
                }
            }
            else
            {
                WindowManager.SpriteBatch.DrawString(gameFont, "Name : "+Typing, new Vector2(x, y), Color.White);
            }
            
            WindowManager.SpriteBatch.DrawString(gameFont, TopBar, new Vector2(325, 42), Color.White);
            
            WindowManager.SpriteBatch.DrawString(gameFont, Button1, new Vector2(319,555), Color.White);
            WindowManager.SpriteBatch.DrawString(gameFont, Button2, new Vector2(465,555), Color.White);
            if (Selected != null) WindowManager.SpriteBatch.Draw(SelectButton, Selected.getOverlayPosition(), Color.White);


            WindowManager.SpriteBatch.End();


        }

        public void setMode(String Mode) { this.Mode = Mode; }


    }
}

    

