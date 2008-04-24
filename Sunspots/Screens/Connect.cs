#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Library.Network;
#endregion


namespace StarForce_PendingTitle_
{
    class Connect : GameWindow
    {
        //This is the image for IP input
        Texture2D ConnectImage;
        Vector2 ImagePosition;

        //This is the image for displaying a failed connection
        Texture2D FailImage;
        Vector2 FailPosition;

        String textToDisplay = "";

        SpriteFont gamefont;

        KeyboardTyping keytyping;
        
        //Confirms if the player has pressed enter after a failed IP entry attempt
        bool failpress = false;
        //A temp Client to check if a connection exists. this gets moved into netclient if succesfull
        private NetClient tempClient;
        //a temp config, see above
        private NetAppConfiguration tempConfig;
        

        //How long to wait to find a connection
        private TimeSpan waitTime = new TimeSpan(0, 0, 10);


        public Connect()
        {
            //Initialize the positions of the images and set the window mode to load
            ImagePosition = new Vector2(-400f, 175f);
            FailPosition = new Vector2(-100f, 275);
            //sets the netclient and config to null
            tempClient = null;
            tempConfig = null;

            Mode = "Load";
        }

        public override void LoadContent()
        {
            //Load the images
            ConnectImage = WindowManager.Content.Load<Texture2D>("Content\\enterip");
            FailImage = WindowManager.Content.Load<Texture2D>("Content\\fail");
            gamefont = WindowManager.Content.Load<SpriteFont>("Content\\SpriteFont1");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public void Load(GameTime gameTime)
        {
            //Smoothly move the input box to the center
            // < 199.5 is used because SmoothStep would take too long to get to 200
            //When the input box is centered, Mode is set to run
            if (ImagePosition.X < 199.5)
                ImagePosition.X = MathHelper.SmoothStep(ImagePosition.X, 200, .35f);
            else
                Mode = "Run";

            keytyping = new KeyboardTyping(Keyboard.GetState());
        }

        public void Run(GameTime gameTime)
        {            
            //If enter is pressed and the connection doesn't work, sets mode to fail
            textToDisplay = keytyping.update(gameTime, Keyboard.GetState());
            
            
            if (WindowManager.NewState.IsKeyDown(Keys.Escape))
            {
                Mode = "Die";
            }
            if (WindowManager.NewState.IsKeyDown(Keys.Enter))
            {
                //Create objects required to contact the server
                NetLog Log;
                tempConfig = new NetAppConfiguration("SimpleChat");

                //Commented out because we're not supporting encryption right now
                /*Config.EnableEncryption(
"AQAB1nF0bzkPd+oG2lXk1lraVovWJHTt9+fuZKUZ4VoH6dmeO8GnuDDjWAk+KjeHh" +
"u1Malaf1DJpdzehTTKXvh9JV47+GUSgyGLjeuASBQCcqi3RVTe0S0u6KhOkdclDjN" +
"gr07oFlyR96UdTLhUtYUnp/E1xbzDgU//A9fdjAP/oBeU=", null);*/

                //For logging information
                Log = new NetLog();
                Log.IgnoreTypes = NetLogEntryTypes.None;

                //Setup the client
                tempClient = new NetClient(tempConfig, Log);
                //tell the client to connect
                tempClient.Connect(textToDisplay, 14242);
                keytyping.clearCurrentString();
                tempClient.StatusChanged += new EventHandler<NetStatusEventArgs>(tempClient_StatusChanged);
            }
            if (tempClient != null)
            {
                ImagePosition.X = MathHelper.Lerp(-400, ImagePosition.X, .90f);

                tempClient.Heartbeat();
                waitTime -= gameTime.ElapsedGameTime;
                if (waitTime.TotalSeconds <= 0 && waitTime.TotalMilliseconds <=0)
                {
                    //Haha you failed
                    Mode = "Fail";
                }
            }
        }

        void tempClient_StatusChanged(object sender, NetStatusEventArgs e)
        {
            if (tempClient.Status == NetConnectionStatus.Connected)
            {
                //If connection is successful connect, destroy this screen, and the title screen
                //and move into the lobby
                NetClientClass.Client = tempClient;
                NetClientClass.Config = tempConfig;
                //lets send the server a message requesting an ID and the playerlist
                NetMessage newmessage = new NetMessage();
                newmessage.Write((byte)5);
                newmessage.Write(NetClientClass.SelectedProfile.getName());
                NetClientClass.sendMessage(newmessage);
                WindowManager.AddScreen(new LobbyScreen());
                WindowManager.removeScreen(this);
                WindowManager.removeScreen(WindowManager.findWindow("TitleScreen"));
            }

        }

        public void Die(GameTime gameTime)
        {
            //Checking if both the fail and the input box images are off the screen (same principal as above)
            if (ImagePosition.X <= -399.5 && FailPosition.X <= -99.5)
            {
                //Find the titlescreen
                //GameWindow Title = (TitleScreen)WindowManager.findWindow("TitleScreen");

                //Hopefully this will never be null, if it is, well, only time will tell (probably will crash)
                //if (Title != null)
                    //Sets the titlescreens mode to unpause
                //    Title.UnPauseWindow();
                //Kill this screen within the windowmanager
                WindowManager.removeScreen(this);
            }
            
            //As long as these are still on screen the screen wont die and the images will move towards the side
            ImagePosition.X = MathHelper.SmoothStep(-400, ImagePosition.X, .35f);
            FailPosition.X = MathHelper.SmoothStep(-100, ImagePosition.X, .35f);
            
        }

        public void Fail(GameTime gameTime)
        {
            //This function is called if mode is set to Fail
            //Usually if an invalid IP is entered, or no connection is established

            //Until The player presses enter to change failpress to true, this will move towards the center
            //It's a notification that the IP was bad
            if (!failpress)
                FailPosition.X = MathHelper.SmoothStep(FailPosition.X, 350f, .35f);
            //Once it's pressed, kill this screen
            else
                Mode = "Die";

            //If enter is pressed, this will start all the killing
            if (WindowManager.NewState.IsKeyDown(Keys.Enter))
                failpress = true;
        }

        public override void Update(GameTime gameTime)
        {
            //OK if you need me to tell you this, you shouldn't be programming
            if (Mode == "Run") Run(gameTime);
            if (Mode == "Load") Load(gameTime);
            if (Mode == "Die") Die(gameTime);
            if (Mode == "Fail") Fail(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the images
            WindowManager.SpriteBatch.Begin();
            WindowManager.SpriteBatch.Draw(ConnectImage, ImagePosition, Color.White);
            WindowManager.SpriteBatch.Draw(FailImage, FailPosition, Color.White);
            WindowManager.SpriteBatch.DrawString(gamefont, textToDisplay, new Vector2(350, 375), Color.White);
            WindowManager.SpriteBatch.End();
        }

        //For window manager findwindow() purposes
        public override string ToString()
        {
            return "Connect";
        }
    }
}
