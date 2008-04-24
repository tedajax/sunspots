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
    class LobbyScreen : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;

        string UserName = NetClientClass.SelectedProfile.getName();
      
        Texture2D Background;

        //The Conversation going on between players
        List<String> Conversation = new List<String>();
       
        String Typing = "";
        //MenuItem SelectedItem = null;

        KeyboardTyping KeyboardHandle;

        KeyboardState Oldstate;

        MenuItem Continue = new MenuItem(new Rectangle(596, 489, 772-596, 548-489), "Continue", new Vector2(596, 489));

        public LobbyScreen()
        {
            Mode = "Load";
            //NetClientClass.Players.Add(UserName);
        }

        public override void LoadContent()
        {
            if (content == null) content = new ContentManager(WindowManager.Game.Services);
            gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
            Background = WindowManager.Content.Load<Texture2D>("Content\\lobby_screen"); 
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gametime)
        {
            if (Mode == "Run") Run(gametime);
            if (Mode == "Load") Load(gametime);
            if (Mode == "Die") Die(gametime);

            if ( NetClientClass.MessagesRecieved.Count > 0)
            {
               
                NetMessage msg =  NetClientClass.MessagesRecieved.Dequeue();

              
                    Conversation.Add(msg.ReadString());
                
                //no need to check for 2 because that's gameplay stuff
                
            }
        }

        public void Load(GameTime gametime)
        {
            KeyboardHandle = new KeyboardTyping(Keyboard.GetState());
            Oldstate = Keyboard.GetState();
            Mode = "Run";

            NetMessage joinMsg = new NetMessage();
            joinMsg.Write((byte)1);
            joinMsg.Write("/joined " + UserName);
            NetClientClass.sendMessage(joinMsg);
        }

        public void Run(GameTime gametime)
        {
            KeyboardState newstate = Keyboard.GetState();
            MouseState mousestate = Mouse.GetState();
            Vector2 mouseXY = new Vector2(mousestate.X, mousestate.Y);
            if (NetClientClass.Client == null || NetClientClass.Client.Status == NetConnectionStatus.Disconnected)
            {
               // WindowManager.AddScreen(new TitleScreen(true));
                WindowManager.removeScreen(this);
            }
            NetClientClass.Update();
            
            String returnstring = KeyboardHandle.update(gametime, Keyboard.GetState());
            Typing = returnstring;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && Oldstate.IsKeyUp(Keys.Enter) )
            {
                if (!Typing.Substring(0, 1).Equals("/"))
                {
                    NetMessage newmessage = new NetMessage();
                    newmessage.Write((byte)1);
                    newmessage.Write(UserName + ": " + Typing);
                    Conversation.Add(UserName + ": " + Typing);
                    NetClientClass.sendMessage(newmessage);
                    KeyboardHandle.clearCurrentString();
                }
                else
                {
                    if (Typing.Substring(1, 2).ToLower().Equals("me"))
                    {
                        string sendstring = " " + UserName + Typing.Substring(3);
                        NetMessage newmessage = new NetMessage();
                        newmessage.Write((byte)1);
                        newmessage.Write(sendstring);
                        Conversation.Add(sendstring);
                        NetClientClass.sendMessage(newmessage);
                        KeyboardHandle.clearCurrentString();
                    }
                    else
                    {
                        NetMessage newmessage = new NetMessage();
                        newmessage.Write((byte)1);
                        newmessage.Write(UserName + ": " + Typing);
                        Conversation.Add(UserName + ": " + Typing);
                        NetClientClass.sendMessage(newmessage);
                        KeyboardHandle.clearCurrentString();
                    }
                }
            }

            while (Conversation.Count > 17)
            {
                Conversation.RemoveAt(0);
            }

            
            if (Continue.isMouseOver(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                WindowManager.AddScreen(new Playing());
                WindowManager.removeScreen(this);
            }

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


        void Game1_Disposed(object sender, EventArgs e)
        {
            NetMessage leaveMsg = new NetMessage();
            leaveMsg.Write("/left " + UserName);
            NetClientClass.sendMessage(leaveMsg);
        }


        public override void Draw(GameTime gameTime)
        {
            // WindowManager.GraphicsDevice.Clear(Color.Black);


            //DrawModel(TestModel);

            WindowManager.SpriteBatch.Begin();

            WindowManager.SpriteBatch.Draw(Background, Vector2.Zero, Color.White);
            int convx = 27;
            int convy = 15;

            int playx = 570;
            int playy = 15;
            foreach (String s in Conversation)
            {
                WindowManager.SpriteBatch.DrawString(gameFont, s, new Vector2(convx, convy), Color.White);
                convy += 25;
            }
            WindowManager.SpriteBatch.DrawString(gameFont, NetClientClass.SelectedProfile.getName(), new Vector2(playx, playy), Color.White);
            playy += 25;
            foreach (OnlinePlayer o in NetClientClass.Players)
            {
                WindowManager.SpriteBatch.DrawString(gameFont, o.getName(), new Vector2(playx, playy), Color.White);
                playy += 25;
            }
            WindowManager.SpriteBatch.DrawString(gameFont, Typing, new Vector2(convx, 500), Color.White);
            WindowManager.SpriteBatch.End();


        }

        public void setMode(String Mode) { this.Mode = Mode; }


    }
}
