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
using System.IO;
using Soopah.Xna.Input;
#endregion

namespace StarForce_PendingTitle_
{
    class Options : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;

       

        //List of MenuItems
        List<MenuItem> menuItemList = new List<MenuItem>();
        //List of Buttons
        List<MenuItem> buttonList = new List<MenuItem>();

        MenuItem SelectedItem = null;
        MenuItem SelectedButton = null;
        MenuItem ClickedButton = null;

        //The 'fullscreen' black image that makes the background fade out
        Texture2D DarkOverlay;
        float DarkOpacity = 0f;
        float MaxDarkOpacity = 200f;
                
        //The User Set Controls
        MyControls Controls;

        public Options()
        {
            Mode = "Load";
        }

        //Load images
        public override void LoadContent()
        {   
            if (content == null) content = new ContentManager(WindowManager.Game.Services);

            gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
            DarkOverlay = WindowManager.Content.Load<Texture2D>("Content\\Dark");

            Controls = WindowManager.Controls;
            if (Controls.ControllerType == "Keyboard") SetUpKeyboardControls();
            if (Controls.ControllerType == "Gamepad") SetUpGamePadControls();
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

        public void SetUpGamePadControls()
        {
            buttonList = new List<MenuItem>();
            int ypos = 100;
            int xpos = 100;
            String DataString = "Boost : " + Controls.MyMovement.Boost.ButtonNumber.ToString();
            Vector2 mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Shoot : " + Controls.MyMovement.Shoot.ButtonNumber.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Bank Left : " + Controls.MyMovement.BankLeft.ButtonNumber.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Bank Right : " + Controls.MyMovement.BankRight.ButtonNumber.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

              
        }

        public void SetUpKeyboardControls()
        {
            buttonList = new List<MenuItem>();
            int ypos = 100;
            int xpos = 100;
            String DataString = "Up : "+Controls.MyMovement.Up.ToString();
            Vector2 mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y+20;
            buttonList.Add(new MenuItem(new Rectangle(xpos,ypos,(int) mesurement.X,(int)mesurement.Y),DataString, new Vector2(xpos,ypos)));
            
            DataString = "Down : "+Controls.MyMovement.Down.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos,ypos,(int) mesurement.X,(int)mesurement.Y),DataString, new Vector2(xpos,ypos)));

            DataString = "Left : " + Controls.MyMovement.Left.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Right : " + Controls.MyMovement.Right.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Shoot : " + Controls.MyMovement.Shoot.Key.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Boost : " + Controls.MyMovement.Boost.Key.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Bank Left : " + Controls.MyMovement.BankLeft.Key.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));

            DataString = "Bank Right : " + Controls.MyMovement.BankRight.Key.ToString();
            mesurement = gameFont.MeasureString(DataString);
            ypos += (int)mesurement.Y + 20;
            buttonList.Add(new MenuItem(new Rectangle(xpos, ypos, (int)mesurement.X, (int)mesurement.Y), DataString, new Vector2(xpos, ypos)));
    
        }

        public void HandleGamepadControls()
        {
            List<ButtonState> ButtonStates = DirectInputGamepad.Gamepads[Controls.MyMovement.Gamepad].Buttons.List;
            int Pressed = -1;
            for (int i = 0; i < ButtonStates.Count; i++)
            {
                if (ButtonStates[i] == ButtonState.Pressed)
                {
                    Pressed = i;
                }
            }
            if (Pressed != -1)
            {
                //they pressed a button
                if (ClickedButton.ToString().Contains("Boost :"))
                {
                    Controls.MyMovement.Boost.ButtonNumber = Pressed;
                }
                if (ClickedButton.ToString().Contains("Shoot :"))
                {
                    Controls.MyMovement.Shoot.ButtonNumber = Pressed;
                }
                if (ClickedButton.ToString().Contains("Bank Left :"))
                {
                    Controls.MyMovement.BankLeft.ButtonNumber = Pressed;
                }
                if (ClickedButton.ToString().Contains("Bank Right :"))
                {
                    Controls.MyMovement.BankRight.ButtonNumber = Pressed;
                }
                ClickedButton = null;
                SetUpGamePadControls();
            }       

        }

        public void HandleKeyboardControls()
        {
            if (NewState.GetPressedKeys().Length > 0)
            {
                //A Key was pressed
                if (ClickedButton.ToString().Contains("Up :"))
                {
                    Controls.MyMovement.Up = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Contains("Down :"))
                {
                    Controls.MyMovement.Down = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Substring(0,6).Equals("Left :"))
                {
                    Controls.MyMovement.Left = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Substring(0,7).Equals("Right :"))
                {
                    Controls.MyMovement.Right = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Contains("Shoot :"))
                {
                    Controls.MyMovement.Shoot.Key = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Contains("Boost :"))
                {
                    Controls.MyMovement.Boost.Key = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Contains("Bank Left :"))
                {
                    Controls.MyMovement.BankLeft.Key = NewState.GetPressedKeys()[0];
                }
                if (ClickedButton.ToString().Contains("Bank Right :"))
                {
                    Controls.MyMovement.BankRight.Key = NewState.GetPressedKeys()[0];
                }
                ClickedButton = null;
                SetUpKeyboardControls();
            }


        }

        public void Load(GameTime gametime)
        {
            //Add menu items, The rectangle explains where the cursor has to be to select that item
            //the string is a name of the item,
            //The Vector2 is an offset coordinate for the highlighting image used to show which item is selected
            int Yposition = 50;
            int Xposition = 200;
            Vector2 mesurement = gameFont.MeasureString("Keyboard");
            menuItemList.Add(new MenuItem(new Rectangle(Xposition, Yposition, (int)mesurement.X, (int)mesurement.Y), "Keyboard", new Vector2(Xposition, Yposition)));
            //Yposition += mesurement.Y;
            Xposition += (int)mesurement.X+50;
            mesurement = gameFont.MeasureString("Controller");
            menuItemList.Add(new MenuItem(new Rectangle(Xposition, Yposition, (int)mesurement.X, (int)mesurement.Y), "Gamepad", new Vector2(Xposition, Yposition)));
            //Yposition += mesurement.Y;
            Xposition += (int)mesurement.X+50;
            mesurement = gameFont.MeasureString("Xbox Controller");
            menuItemList.Add(new MenuItem(new Rectangle(Xposition, Yposition, (int)mesurement.X, (int)mesurement.Y), "XBox Controller", new Vector2(Xposition, Yposition)));
          
           // menuItemList.Add(new MenuItem(new Rectangle(0, 310, 240, 30), "Options", new Vector2(-87, 310)));
            Mode = "Run";
        }

        /// <summary>
        /// If the Mouse is over any of the menu items, return the menu item
        /// </summary>
        /// <param name="mouseXY">Mouse X and Y coordinates ina  Vector2</param>
        /// <returns>A menu item, if the mouse is over one</returns>
        private MenuItem checkMenuItems(Vector2 mouseXY, List<MenuItem> ItemsToCheck)
        {
            for (int i =0; i<ItemsToCheck.Count;i++)
            {
                if (ItemsToCheck[i].isMouseOver(mouseXY))
                {
                    return ItemsToCheck[i];
                }
            }
            return null;
        }
                  

        public void Run(GameTime gametime)
        {

            //Get the mouse state (position, buttons clicked) and check for menu items 'under' the cursor
            MouseState mousestate = Mouse.GetState();
            KeyboardState keystate = WindowManager.NewState;
            Vector2 mouseXY = new Vector2(mousestate.X, mousestate.Y);
            SelectedItem = checkMenuItems(mouseXY,menuItemList);
            SelectedButton = checkMenuItems(mouseXY,buttonList);

            if (keystate.IsKeyDown(Keys.Back))
            {
                Controls.Save("Controls.xml");
                //((TitleScreen)(WindowManager.findWindow("TitleScreen"))).UnPauseWindow();
                WindowManager.AddScreen(new MainMenu());
                WindowManager.removeScreen(this);
            }
            
            //If there is a selected item
            if (SelectedItem != null)
            {
                if (mousestate.LeftButton == ButtonState.Pressed)
                {
                    Controls.ControllerType = SelectedItem.ToString();
                    if (SelectedItem.ToString() == "Keyboard") SetUpKeyboardControls();
                    if (SelectedItem.ToString() == "Gamepad") SetUpGamePadControls();
                    //if (SelectedItem.ToString() == "Xbox Controller") Controls.ControllerType = "360Pad";
                }
            }
            if (SelectedButton != null)
            {
                if (mousestate.LeftButton == ButtonState.Pressed)
                {
                    ClickedButton = SelectedButton;
                }
            }
            if (ClickedButton != null)
            {
                if (Controls.ControllerType == "Keyboard")
                {
                    HandleKeyboardControls();
                }
                if (Controls.ControllerType == "Gamepad")
                {
                    HandleGamepadControls();
                }   
            }
            if (Controls.ControllerType == "XBox Controller")
            {
                Controls.MyMovement.InvertYAxis = -1;
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

            foreach (MenuItem M in menuItemList)
            {
                Color DrawColor = Color.White;
                
                if (M == SelectedItem) DrawColor = Color.Red;
                if (M.ToString() == Controls.ControllerType) DrawColor = Color.Green;
                WindowManager.SpriteBatch.DrawString(gameFont, M.ToString(), M.getOverlayPosition(), DrawColor);
            }

            foreach (MenuItem M in buttonList)
            {
                Color DrawColor = Color.White;

                if (M == SelectedButton) DrawColor = Color.Red;
                if (M == ClickedButton) DrawColor = Color.Green;
                //if (M.ToString() == Controls.ControllerType) DrawColor = Color.Green;
                WindowManager.SpriteBatch.DrawString(gameFont, M.ToString(), M.getOverlayPosition(), DrawColor);
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
            return "Options";
        }
    }
}
