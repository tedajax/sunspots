
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Library.Network;
using System.Collections;

namespace sOMETHINGELSE
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NewWindow : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static NetClient Client;
        public static NetLog Log;
        public static NetAppConfiguration Config;

        public bool uppercase;

        public static string SendString = "";

        public static List<String> thingstoDisplay = new List<String>();
        SpriteFont newfont;

        Texture2D TitleScreen;

        public static string UserName = "Tester";

        KeyboardState oldstate;
        public NewWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            oldstate = Keyboard.GetState();
            // Create a configuration for the client
            // 'SimpleChat' is the server application identifier we want to connect to
            Config = new NetAppConfiguration("SimpleChat");

            // enable encryption; this key was generated using the 'GenerateEncryptionKeys' application
            Config.EnableEncryption(
 "AQAB1nF0bzkPd+oG2lXk1lraVovWJHTt9+fuZKUZ4VoH6dmeO8GnuDDjWAk+KjeHh" +
 "u1Malaf1DJpdzehTTKXvh9JV47+GUSgyGLjeuASBQCcqi3RVTe0S0u6KhOkdclDjN" +
 "gr07oFlyR96UdTLhUtYUnp/E1xbzDgU//A9fdjAP/oBeU=", null);

            Log = new NetLog();
            Log.IgnoreTypes = NetLogEntryTypes.None; //  Verbose;

            Log.OutputFileName = "clientlog.html";

            // uncomment this if you want to start multiple instances of this process on the same computer
            //Log.OutputFileName = "clientlog" + System.Diagnostics.Process.GetCurrentProcess().Id + ".html";

            //Log.LogEvent += new EventHandler<NetLogEventArgs>(Log_LogEvent);

            Client = new NetClient(Config, Log);
           // Client.StatusChanged += new EventHandler<NetStatusEventArgs>(Client_StatusChanged);

            Log.Info("Enter ip or 'localhost' to connect...");


            Client.Connect("10.40.4.255", 14242);
            this.Exiting += new EventHandler(Game1_Exiting);

            
        }

        void Game1_Exiting(object sender, EventArgs e)
        {
            Client.Shutdown("Application exiting");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            newfont = Content.Load<SpriteFont>("SpriteFont1");
            TitleScreen = Content.Load<Texture2D>("title");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // call heartbeat as often as possible; this sends queued packets,
            // keepalives and acknowledges etc.
            Client.Heartbeat();

            NetMessage msg;
            // read a packet if available
            while ((msg = Client.ReadMessage()) != null)
                HandleMessage(msg);

            uppercase = false;
            KeyboardState newstate = Keyboard.GetState();
            if (newstate.GetPressedKeys().Length > 0)
            {
                if (newstate.IsKeyDown(Keys.LeftShift))
                {
                    uppercase = true;
                }
                if (newstate.IsKeyDown(Keys.Enter) && oldstate.IsKeyUp(Keys.Enter))
                {
                    //SEND
                    NetMessage sendmsg = new NetMessage();
                    sendmsg.Write(UserName + ": " + SendString);

                    // send use the ReliableUnordered channel; ie. it WILL arrive, but not necessarily in order
                    Client.SendMessage(sendmsg, NetChannel.ReliableUnordered);
                    SendString = "";
                }
                else if (newstate.IsKeyDown(Keys.OemPeriod) && oldstate.IsKeyUp(Keys.Enter))
                {
                    SendString += ".";
                }
                else if (newstate.IsKeyDown(Keys.OemQuestion) && oldstate.IsKeyUp(Keys.Enter))
                {
                    SendString += "?";
                }
                else if (newstate.IsKeyDown(Keys.Back) && oldstate.IsKeyUp(Keys.Enter))
                {
                    if (SendString.Length > 0)
                        SendString = SendString.Substring(0, SendString.Length - 1);
                }

                else if (newstate.IsKeyDown(Keys.Space) && oldstate.IsKeyUp(Keys.Space))
                {
                    SendString += " ";
                }
                else
                {
                    foreach (Keys pressed in newstate.GetPressedKeys())
                    {
                        String newerstring = pressed.ToString().ToLower();
                        if (uppercase) newerstring = newerstring.ToUpper();
                        if (oldstate.IsKeyUp(pressed) && isCharacter(pressed.ToString()))
                            SendString += newerstring;
                    }
                }
            }

            oldstate = newstate;
            while (thingstoDisplay.Count > 10)
            {
                thingstoDisplay.RemoveAt(0);
            }
            //Thread.Sleep(1); // don't hog the cpu
            

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private static bool isCharacter(String newstring)
        {
            string c = "abcdefghijklmnopqrstuvwxyz";
            if (c.Contains(newstring.ToLower())) return true;

            return false;
        }

        private static void HandleMessage(NetMessage msg)
        {
            // we received a chat message!
            Log.Debug("msg: " + msg);
            try
            {
                thingstoDisplay.Add(msg.ReadString());  
                
            }
            catch { } // ignore disposal problems
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Drawyer(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            int x = 150;
            int y = 150;
            spriteBatch.Begin();
            spriteBatch.Draw(TitleScreen, Vector2.Zero, Color.White);
            for (int i = 0; i < thingstoDisplay.Count; i++)
            {
                spriteBatch.DrawString(newfont, thingstoDisplay[i], new Vector2(x, y), Color.White);
                y += 25;
            }
            spriteBatch.DrawString(newfont, SendString, new Vector2(x, 500), Color.White);
            spriteBatch.End();
            //thingstoDisplay.Clear();
            base.Draw(gameTime);
        }
    }
}
