#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Library.Network;
using Ziggyware.Xna;
#endregion
//Lolololol
namespace StarForce_PendingTitle_
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Changing Stuff This is fun
        /// </summary>
        static GraphicsDeviceManager graphics;
        ContentManager content;

        WindowManager windowManager;

        public static Random Randomizer = new Random();

        public static int iWidth = 800;
        public static int iHeight = 600;
        public static Vector2 CenterScreen = new Vector2(iWidth / 2, iHeight / 2);
        public static bool fullscreen =false;
        public static int FPS = 60;
        public static float FieldOfView = 45f;
        public static float DefaultFieldOfView = FieldOfView;
        
        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public Game1()
        {
           
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / 30);
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferredBackBufferWidth = iWidth;
            graphics.PreferredBackBufferHeight = iHeight;
            graphics.IsFullScreen = fullscreen;

            content = new ContentManager(Services);
            this.IsMouseVisible = true;
            //Content.RootDirectory = "Content";

            Music.SetMusicFilename("Content\\Sound\\test_sunspots_music");
            Music.ResetEngine();
            //Music.PlaySong("simpleandclean");

            SoundFX.SetSoundFilename("Content\\Sound\\test_sunspots_music");
            SoundFX.ResetEngine();

            //graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
        }

        void Game1_Exiting(object sender, EventArgs e)
        {
            if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
            {
                NetMessage newmessage = new NetMessage();
                newmessage.Write((byte)1);
                newmessage.Write("/left "+NetClientClass.SelectedProfile.getName());
                NetClientClass.sendMessage(newmessage);
               
                NetClientClass.Client.FlushMessages();
                NetClientClass.Client.Heartbeat();

                NetClientClass.Client.Disconnect(NetClientClass.SelectedProfile.getName());
            }
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

            windowManager = new WindowManager(this);
            
            windowManager.AddScreen(new BackGroundScreen());

            this.Exiting += new EventHandler(Game1_Exiting);

            Components.Add(windowManager);
          
            base.Initialize();

        }

       
        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadContent()
        {

        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadContent()
        {
            content.Unload();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            SoundFX.UpdatePlayedSounds();            
           
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            base.Draw(gameTime);
        }

        /// <summary>
        /// Return an angle value between -Pi and Pi
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < 0)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.TwoPi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        /// <summary>
        /// Return an angle value between 0 and 2Pi
        /// </summary>
        public static float WrapAngle360(float radians)
        {
            while (radians < 0)
                radians += MathHelper.TwoPi;

            while (radians > MathHelper.TwoPi)
                radians -= MathHelper.TwoPi;

            return radians;
        }
    }
}
