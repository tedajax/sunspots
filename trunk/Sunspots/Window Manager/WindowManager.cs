#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Ziggyware.Xna;

#endregion

namespace StarForce_PendingTitle_
{
    
    public class WindowManager : DrawableGameComponent
    {

        public static SignedInGamer CurrentGamer;

        List<GameWindow> windows = new List<GameWindow>();

        List<GameWindow> windowstoUpdate = new List<GameWindow>();

        ContentManager content;

        static PointSpriteManager psparticles;
        
        public Vector3 cameraPosition = new Vector3(0.0f, 120.0f, 5f);
        float aspectRatio = Game1.iWidth / Game1.iHeight;

        //GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        SpriteFont gameFont;

        public static IGraphicsDeviceService graphicsDeviceService;

        public KeyboardState OldState;
        public KeyboardState NewState;

        MyControls controls;

        public static ParticleSystem3D ExplosionParticles;
        public static ParticleSystem3D ExplosionSmokeParticles;
        public static ParticleSystem3D SmokeTrailParticles;
        public static ParticleSystem3D SmokeParticles;
        public static ParticleSystem3D FireParticles;
             
        public static BoundingFrustum ScreenFrustum;

        #region exposestuff

        public MyControls Controls
        {
            get { return this.controls; }
            internal set { this.controls =value; }
        }

        new public Game Game
        {
            get { return base.Game; }
        }

        public Game getGame() { return base.Game; }

        public BoundingFrustum ScreenViewFrustum
        {
            get { if (ScreenFrustum != null) return ScreenFrustum; else return null; }
        }

        public GraphicsDevice GD
        {
            get { return GD; }
            
        }

        public ContentManager Content
        {
            get { return content; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        
        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        public static PointSpriteManager PSParticles
        {
            get { return psparticles; }
        }

#endregion

        public WindowManager(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);

            graphicsDeviceService = (IGraphicsDeviceService)game.Services.GetService(
                                                        typeof(IGraphicsDeviceService));

            if (graphicsDeviceService == null)
                throw new InvalidOperationException("No graphics device service.");

            OldState = Keyboard.GetState();

          //  WindowManagerTextBoxes = new TextboxManager();

            ExplosionParticles = new ExplosionParticleSystem(this.Game, this.Content);
            ExplosionSmokeParticles = new ExplosionSmokeParticleSystem(this.Game, this.Content);
            SmokeTrailParticles = new ProjectileTrailParticleSystem(this.Game, this.Content);
            SmokeParticles = new SmokePlumeParticleSystem(this.Game, this.Content);
            FireParticles = new FireParticleSystem(this.Game, this.Content);

            this.controls = MyControls.Load("Controls.xml");

            this.Game.Components.Add(ExplosionParticles);
            this.Game.Components.Add(ExplosionSmokeParticles);
            this.Game.Components.Add(SmokeTrailParticles);
            this.Game.Components.Add(SmokeParticles);
            this.Game.Components.Add(FireParticles);
        }

        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");

            psparticles = new PointSpriteManager(Game);

            foreach (GameWindow window in windows)
            {
                window.LoadContent();
            }
            
        }

        protected override void UnloadContent()
        {
            Content.Unload();

            foreach (GameWindow window in windows)
            {
                window.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Check The Keyboard First
            NewState = Keyboard.GetState();

                //Check to see who is the CurrentGamer
              
                //Figure out what windows to update

                windowstoUpdate.Clear();

                foreach (GameWindow window in windows)
                {
                    windowstoUpdate.Add(window);
                }

                //Update Each Window
                while (windowstoUpdate.Count > 0)
                {
                    GameWindow window = windowstoUpdate[windowstoUpdate.Count - 1];
                    windowstoUpdate.RemoveAt(windowstoUpdate.Count - 1);

                    window.Update(gameTime);
                }

               
            
            //Save the Keyboard State
            OldState = NewState;

           
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //
            //spriteBatch.Begin();
            foreach(GameWindow window in windows)
            {
                window.Draw(gameTime);
            }
           
            //spriteBatch.End();
            
        }


        public void AddScreen(GameWindow window)
        {
            window.WindowManager = this;

            if ((graphicsDeviceService != null) && (graphicsDeviceService.GraphicsDevice != null))
                window.LoadContent();


            windows.Add(window);
            window.Init();
        }

        /// <summary>
        /// adds a screen to be handled by Window Manager
        /// </summary>
        /// <param name="window">the game screen</param>
        /// <param name="Position">The Position in Window Mangers list to put it AT</param>
        public void AddScreen(GameWindow window, int Position)
        {
            window.WindowManager = this;

            if ((graphicsDeviceService != null) && (graphicsDeviceService.GraphicsDevice != null))
                window.LoadContent();

            windows.Insert(Position, window);
            window.Init();



        }

        public int FindLastScreenPosition() { return windows.Count - 1; }

        public void removeScreen(GameWindow window)
        {
            windows.Remove(window);
            window = null;
            GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// Finds if a Specific Window Exists
        /// </summary>
        /// <param name="Lookup">Name of Window to Look up (ToString)</param>
        /// <returns></returns>
        public Boolean windowExist(String Lookup)
        {
            Boolean exist = false;
            List<GameWindow> L = new List<GameWindow>();
            foreach(GameWindow w in windows)
            {
                L.Add(w);
            }
            while (exist == false && L.Count > 0)
            {
                if (L[L.Count - 1].ToString() == Lookup) exist = true;
                L.RemoveAt(L.Count - 1);
            }

            return exist;
        }

        /// <summary>
        /// Finds a Specific Window then Returns the Window
        /// </summary>
        /// <param name="Lookup">The Name of the Window to look up</param>
        /// <returns></returns>
        public GameWindow findWindow(String Lookup)
        {
            GameWindow exist = null;
            List<GameWindow> L = new List<GameWindow>();
            foreach (GameWindow w in windows)
            {
                L.Add(w);
            }
            while (exist == null && L.Count > 0)
            {
                if (L[L.Count - 1].ToString() == Lookup) exist = L[L.Count - 1];
                L.RemoveAt(L.Count - 1);
            }

            return exist;
        }

        public GameWindow findWindow(int Position)
        {
            return windows[Position];
        }

        public static Vector3 V3FromV2(Vector2 input)
        {
            return new Vector3(input.X, 0f, input.Y);
        }        
    }
}
