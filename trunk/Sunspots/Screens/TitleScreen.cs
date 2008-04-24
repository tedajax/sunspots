#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace StarForce_PendingTitle_
{
    class TitleScreen : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;

        //Title Screen
        Texture2D Title;
        //Selection Overlay
        Texture2D Select;
        //List of MenuItems
        List<MenuItem> menuItemList = new List<MenuItem>();

        MenuItem SelectedItem = null;

        //The 'fullscreen' black image that makes the background fade out
        Texture2D DarkOverlay;
        float DarkOpacity = 0f;
        float MaxDarkOpacity = 200f;

        public TitleScreen(Boolean name)
        {
            Mode = "Load";
        }


        //Load images
        public override void LoadContent()
        {   
            if (content == null) content = new ContentManager(WindowManager.Game.Services);

            gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
            Title = WindowManager.Content.Load<Texture2D>("Content\\title");           
            Select = WindowManager.Content.Load<Texture2D>("Content\\select");
            DarkOverlay = WindowManager.Content.Load<Texture2D>("Content\\Dark");  
        }

        public override void UnloadContent()
        {
            content.Unload();
        }
        public override void Update(GameTime gametime)
        {
            if (Mode == "Run") Run(gametime);
            if (Mode == "Pause") Pause(gametime);
            if (Mode == "UnPause") UnPause(gametime);
            if (Mode == "Load") Load(gametime);
            if (Mode == "Die") Die(gametime);
        }

        public void Load(GameTime gametime)
        {
            //Add menu items, The rectangle explains where the cursor has to be to select that item
            //the string is a name of the item,
            //The Vector2 is an offset coordinate for the highlighting image used to show which item is selected
            menuItemList.Add(new MenuItem(new Rectangle(0, 171, 324, 30), "Single Player", new Vector2(0, 174)));
            menuItemList.Add(new MenuItem(new Rectangle(0, 220, 298, 29), "Host", new Vector2(-33, 220)));
            menuItemList.Add(new MenuItem(new Rectangle(0, 265, 265, 30), "Join", new Vector2(-62, 265)));
            menuItemList.Add(new MenuItem(new Rectangle(0, 310, 240, 30), "Options", new Vector2(-87, 310)));
            Mode = "Run";
        }

        /// <summary>
        /// If the Mouse is over any of the menu items, return the menu item
        /// </summary>
        /// <param name="mouseXY">Mouse X and Y coordinates ina  Vector2</param>
        /// <returns>A menu item, if the mouse is over one</returns>
        private MenuItem checkMenuItems(Vector2 mouseXY)
        {
            for (int i =0; i<menuItemList.Count;i++)
            {
                if (menuItemList[i].isMouseOver(mouseXY))
                {
                    return menuItemList[i];
                }
            }
            return null;
        }
                  

        public void Run(GameTime gametime)
        {

            //Get the mouse state (position, buttons clicked) and check for menu items 'under' the cursor
            MouseState mousestate = Mouse.GetState();
            Vector2 mouseXY = new Vector2(mousestate.X, mousestate.Y);
            SelectedItem = checkMenuItems(mouseXY);
            
            //If there is a selected item
            if (SelectedItem != null)
            {
                //If it's join and the left mouse button is pressed
                if (SelectedItem.ToString().Equals("Join") && mousestate.LeftButton == ButtonState.Pressed)
                {
                    Mode = "Pause"; //Pause this screen
                    WindowManager.AddScreen(new Connect()); //Create a IP address input box
                }

                //Remove this screen and Start the lobby screen
                if (SelectedItem.ToString() == "Host" && mousestate.LeftButton == ButtonState.Pressed)
                {
                    WindowManager.AddScreen(new LobbyScreen());
                    WindowManager.removeScreen(this);
                }

                if (SelectedItem.ToString().Equals("Single Player") && mousestate.LeftButton == ButtonState.Pressed)
                {
                    WindowManager.AddScreen(new Playing());
                    WindowManager.removeScreen(this);
                }
                if (SelectedItem.ToString().Equals("Options") && mousestate.LeftButton == ButtonState.Pressed)
                {
                    WindowManager.AddScreen(new Options());
                    this.Mode = "Pause";
                }
            }
        }

        //Kill this screen, game probably ends when that happens
        public void Die(GameTime gametime)
        {
            UnloadContent();
            WindowManager.removeScreen(this);
        }
                

        public override void Draw(GameTime gameTime)
        {
            //Start drawing
            WindowManager.SpriteBatch.Begin();

            //Draw the Background image
            WindowManager.SpriteBatch.Draw(Title, Vector2.Zero, Color.White);

            //Draw the different item strings
            WindowManager.SpriteBatch.DrawString(gameFont, "Single Player", new Vector2(25, 178), Color.White);
            WindowManager.SpriteBatch.DrawString(gameFont, "Host Server (Not Working)", new Vector2(25, 225), Color.White);
            WindowManager.SpriteBatch.DrawString(gameFont, "Connect to Server", new Vector2(25, 270), Color.White);
            WindowManager.SpriteBatch.DrawString(gameFont, "Options", new Vector2(25, 315), Color.White);

            //If there is a selected item, draw the highlight bar over it
            if (SelectedItem != null)
            {
                WindowManager.SpriteBatch.Draw(Select, SelectedItem.getOverlayPosition(),Color.White);
            }

            //Draw the dark overlay with the opacity defined by "DarkOpacity"
            WindowManager.SpriteBatch.Draw(DarkOverlay, Vector2.Zero, new Color(255, 255, 255, (byte)DarkOpacity));

            //stop drawing
            WindowManager.SpriteBatch.End();

            
        }

        //For setting the mode
        public void setMode(String Mode) { this.Mode = Mode; }

        //Pauses, increases DarkOpacity to a point, nothing else happens until unpause is called
        public void Pause(GameTime gameTime)
        {
            if (DarkOpacity < MaxDarkOpacity)
                DarkOpacity += 5f;
        }

        //Fade the title back in, then set it to run again
        public void UnPause(GameTime gameTime)
        {
            if (DarkOpacity > 0)
                DarkOpacity -= 5f;
            else
            {
                DarkOpacity = 0f;
                Mode = "Run";
            }
        }

        //Game window override for unpausing from screens made from this one (and all of the screens made from them)
        public override void UnPauseWindow()
        {
            Mode = "UnPause";
        }

        //For WindowManager findwindow()
        public override string ToString()
        {
            return "TitleScreen";
        }
    }
}
