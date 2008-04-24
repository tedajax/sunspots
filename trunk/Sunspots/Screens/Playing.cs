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
using Lidgren.Library.Network.Xna;
using Ziggyware.Xna;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;


#endregion

namespace StarForce_PendingTitle_
{
    class Playing : GameWindow
    {
        Model PlayerModel;
      
        //KeyboardTyping KeyboardTypingHandle;

       /* const float CameraXZdistance = 85f;
        const float CameraYdistance = 13;
        const float catchupvalue = .95f;*/

        CollisionManager CollisionManager;

        Texture2D Dark;
        float DarkOpacity = 255f;

        List<Controller> Controllers;

        SpriteFont gameFont;

        ParticleSystem ParticleSystem;

        SkySphere skySphere;

        Model EnemyModel;
        EnemyManager EnemyManagement;
        
        Random Randomizer;
        BloomComponent bloom;

        Level NewLevel;

        Effect postprocessEffect;

        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;

        public static Model LaserModel;
        public static LaserManager LaserManagement;

        public static Missle PlayerMissile;
        public static Model MissleModel;

        Boundries Bounds = new Boundries();

        ObjectCollisionManager ObjColMngr = new ObjectCollisionManager();
        /// <summary>
        /// a little texture to display how many enemies were killed
        /// </summary>
        Texture2D EnemiesKilled;

        public static Vector3 StartingPosition = new Vector3(5100,358,-7300);

        public static bool MissionComplete;
        TimeSpan VictoryTimer;

        float ShootOldState = 0f;

        ContentManager Content;

        public Playing()
        {
            Mode = "Load";
            CameraClass.setUpCameraClass();
            ParticleSystem = new ParticleSystem();
            Randomizer = new Random((int)DateTime.Now.Ticks);
            MissionComplete = false;
            VictoryTimer = new TimeSpan(0, 0, 0, 3);
            ShootOldState = 0f;
            
        }

        String ContentLoaded = "None";
        bool SentServerLoaded = false;
        public override void LoadContent()
        {
            Content = new ContentManager(WindowManager.Game.Services);
            gameFont = Content.Load<SpriteFont>("Content\\SpriteFont2");
           

            // Starts a new thread to perform a task
           ThreadStart ThreadStarter = delegate
            {
                LoadData();
            };
            Thread myThread = new Thread(ThreadStarter);
            
            myThread.Start();
            //LoadData();
        }

        void LoadData()
        {
            ContentLoaded = "Loading Level File";
            LevelData LevelData = LevelData.Load("TestLevel.XML");

            ContentLoaded = "Loading Bloom";
            //Load Bloom
            bloom = new BloomComponent(WindowManager.Game);
            WindowManager.Game.Components.Add(bloom);
            bloom.Settings = BloomSettings.PresetSettings[6];


            ContentLoaded = "Loading Cartoon Effect";

            //set up the other effects
            postprocessEffect = Content.Load<Effect>("Content\\Effects\\PostprocessEffect");
            Effect cartoonEffect = Content.Load<Effect>("Content\\Effects\\CartoonEffect");


            ContentLoaded = "Setting up Render Targets";

            // Create two custom rendertargets.
            PresentationParameters pp = WindowManager.GraphicsDevice.PresentationParameters;

            sceneRenderTarget = new RenderTarget2D(WindowManager.GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);

            normalDepthRenderTarget = new RenderTarget2D(WindowManager.GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);


            ContentLoaded = "Setting up Sky Sphere";

            string[] clouds = { "Content//Sky//clouds00", "Content//Sky//clouds0", "Content//Sky//clouds1", "Content//Sky//clouds2" };
            skySphere = new SkySphere(Content, "Content//Sky//InvSphere", "Content//Sky//dynamicSkybox", "Content//Sky//cubeMap", clouds);
            skySphere.StopTime = true;
            skySphere.tod = 14;

            //setup the basic effect for debugging
            skySphere.LoadGraphicsContent(Content);

            ContentLoaded = "Setting up Debug Manager";
            
            DebugManager.setUp(WindowManager.GraphicsDevice);

            ContentLoaded = "Setting up Player";

            MainShip MyPlayer;
            PlayerModel = Content.Load <Model>("Content\\unwrappedmodel");
            ChangeEffectUsedByModel(PlayerModel, cartoonEffect);
            Dark = Content.Load<Texture2D>("Content\\Dark");
           
            EnemyModel = Content.Load<Model>("Content\\enemyship");

            ChangeEffectUsedByModel(EnemyModel, cartoonEffect);

            Queue<Vector3> WaypointQueue = new Queue<Vector3>();
            foreach (LevelData.WaypointData W in LevelData.Waypoints.WaypointData)
            {
                WaypointQueue.Enqueue(W.Position*20f);
            }
            StartingPosition = WaypointQueue.Dequeue();

            MyPlayer = new RailShip(PlayerModel, WaypointQueue);

            ContentLoaded = "Setting up HUD";

            Hud hud = new Hud();
            hud.LoadContent(Content);

            ContentLoaded = "Setting up Collision Manager";

            CollisionManager = new CollisionManager(MyPlayer);


            ContentLoaded = "Setting up Level";

            NewLevel = new Level(Content, WindowManager.GraphicsDevice, cartoonEffect,LevelData);
           
            ContentLoaded = "Setting up Controllers";

            Controllers = new List<Controller>();
            Controllers.Add(new LocalPlayer(MyPlayer, WindowManager.Controls,hud));

            CameraClass.Position = StartingPosition;
            //MyPlayer.Position = new Vector3(4400, 200, -3633);
            //MyPlayer.rotationValues = new Vector3(0, 135, 0);
            ObjColMngr.AddLocalPlayer((LocalPlayer)Controllers[0]);
            
            ContentLoaded = "Setting up Online Players";

            foreach (OnlinePlayer o in NetClientClass.Players)
            {
                MainShip Onlineship = new AllRangeShip(PlayerModel);
                Controllers.Add(new OnlineController(o, Onlineship));
            }

            ContentLoaded = "Setting up Particle System";

            ParticleSystem.LoadGraphicsContent(WindowManager.GraphicsDevice, Content);

            ContentLoaded = "Setting up Enemies";
            EnemiesKilled = Content.Load<Texture2D>("Content\\Hud\\enemieskilled");
            EnemyManagement = new EnemyManager();
            Vector3[] EnemyPositions = { new Vector3(5400, 200, -6000), new Vector3(7100, 500, -7100), new Vector3(5400, 500, -6500), new Vector3(5100, 500, -6100), new Vector3(4600, 800, -7000), new Vector3(7000, 300, -6300), new Vector3(6100, 400, -6000), StartingPosition + new Vector3(300, 0, -1000), StartingPosition + new Vector3(-300, 100, -500), StartingPosition + new Vector3(300, 100, -500), StartingPosition + new Vector3(100, 100, -600) };
            for (int x = 0; x < EnemyPositions.Length; x++)
            {
                EnemyManagement.AddEnemy(new Defensive(EnemyModel, EnemyPositions[x], Vector3.Zero, EnemyManager.EnemyId));
                //EnemyManagement.GetEnemyList()[x].ENEMY_SPEED = (float)Randomizer.NextDouble() * 10f;
            }
            LaserManagement = new LaserManager();

            ContentLoaded = "Setting up Projectiles";
            
            LaserModel = Content.Load<Model>("Content\\Laser\\laser");
            ChangeEffectUsedByModel(LaserModel, cartoonEffect);
            MissleModel = Content.Load<Model>("Content\\Missle\\Missle");
            ChangeEffectUsedByModel(MissleModel, cartoonEffect);
            PlayerMissile = new Missle(MissleModel);

            ContentLoaded = "Generating Management Systems";

            ObjColMngr.EnemyMngr = EnemyManagement;
            ObjColMngr.LaserMngr = LaserManagement;
            //ObjColMngr.AddBoundaryBoxes(Bounds);

            ContentLoaded = "Done";
        }

        /// <summary>
        /// Alters a model so it will draw using a custom effect, while preserving
        /// whatever textures were set on it as part of the original effects.
        /// </summary>
        public static void ChangeEffectUsedByModel(Model model, Effect replacementEffect)
        {
            
            // Table mapping the original effects to our replacement versions.
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                // Scan over all the effects currently on the mesh.
                foreach (BasicEffect oldEffect in mesh.Effects)
                {
                    // If we haven't already seen this effect...
                    if (!effectMapping.ContainsKey(oldEffect))
                    {
                        // Make a clone of our replacement effect. We can't just use
                        // it directly, because the same effect might need to be
                        // applied several times to different parts of the llmodel using
                        // a different texture each time, so we need a fresh copy each
                        // time we want to set a different texture into it.
                        Effect newEffect = replacementEffect.Clone(
                                                    replacementEffect.GraphicsDevice);

                        // Copy across the texture from the original effect.
                        newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                        newEffect.Parameters["TextureEnabled"].SetValue(
                                                            oldEffect.TextureEnabled);

                        effectMapping.Add(oldEffect, newEffect);
                    }
                }

                // Now that we've found all the effects in use on this mesh,
                // update it to use our new replacement versions.
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectMapping[meshPart.Effect];
                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (Mode == "Load") Load(gameTime);
            if (Mode == "Run") Run(gameTime);
            if (Mode == "Die") Die(gameTime);
        }

     
        public void Run(GameTime gameTime)
        {
            skySphere.Update(gameTime);
            foreach (Controller c in Controllers)
            {
                c.update(gameTime, WindowManager.NewState, WindowManager.OldState);
            }
         
            NetClientClass.Update();
            //CollisionManager.UpdateCollision();
            ObjColMngr.CheckObjectCollisions();
            ParticleSystem.Update(gameTime);
            EnemyManagement.Update(gameTime);
            LaserManagement.Update(gameTime);

            if (EnemyManagement.GetEnemyCount() == 0)
            {
                MissionComplete = true;
            }
            if (MissionComplete == true)
            {
                float shootState = WindowManager.Controls.getShoot();
                VictoryTimer -= gameTime.ElapsedGameTime;
                if (((LocalPlayer)Controllers[0]).HUD.hasComboCaughtUp() && VictoryTimer.TotalSeconds <= 0)
                {
                   
                    if (shootState == 0 && ShootOldState > 0)
                    {
                        this.Mode = "Die";
                    }
                    
                }
                ShootOldState = shootState;
            }
            
            //PlayerMissile.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Mode == "Run") GameplayDraw(gameTime);
            if (Mode == "Load")
            {
                WindowManager.GraphicsDevice.Clear(Color.Black);
                WindowManager.SpriteBatch.Begin();
                Vector2 Size = gameFont.MeasureString(ContentLoaded);
                WindowManager.SpriteBatch.DrawString(gameFont, ContentLoaded, new Vector2(400-Size.X/2, 300 - Size.Y/2), Color.White);
                if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
                {
                    WindowManager.SpriteBatch.DrawString(gameFont, "Waiting for other players", new Vector2(0, 40), Color.White);
                }

                // WindowManager.SpriteBatch.DrawString(gameFont, MyPlayer.Position.ToString(), new Vector2(0, 35), Color.White);
                 WindowManager.SpriteBatch.End();

            }
        }

        public void GameplayDraw(GameTime gameTime)
        {            
            WindowManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            WindowManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            WindowManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            RenderState renderState = WindowManager.GraphicsDevice.RenderState;

            renderState.AlphaBlendEnable = true ;
            renderState.AlphaTestEnable = false;
            renderState.DepthBufferEnable = true;

            GraphicsDevice device = WindowManager.GraphicsDevice;
            //CameraClass.FarDistance = 8100;
            //CameraClass.Update();
            device.SetRenderTarget(0, normalDepthRenderTarget);
            //device.SetRenderTarget(0, null);
                //skySphere.Draw(gameTime, CameraClass.getLookAt());
                NewLevel.drawObjects(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "NormalDepth");
                NewLevel.drawWater(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "NormalDepth");   
                foreach (Controller C in Controllers) C.DebugDraw(CameraClass.getLookAt(), gameFont, WindowManager.SpriteBatch, "NormalDepth");
                
                EnemyManagement.Draw("NormalDepth");
                LaserManagement.Draw("NormalDepth");
                PlayerMissile.Draw("NormalDepth");
                ParticleSystem.Draw(gameTime, WindowManager.GraphicsDevice, CameraClass.getLookAt());
                WindowManager.ExplosionSmokeParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.ExplosionParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.SmokeParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.SmokeTrailParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.FireParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
            device.SetRenderTarget(0, sceneRenderTarget);
      
            device.Clear(Color.Black);
        
            //CameraClass.FarDistance = 10000f;
            //CameraClass.Update();
            skySphere.Draw(gameTime, CameraClass.getLookAt());
            //DebugManager.drawDebugBox(Controllers[0].MainShip.getCollisionData(), Controllers[0].MainShip.GetWorldMatrix(), CameraClass.getLookAt());
           // DebugManager.drawDebugBox(PlayerMissile.getCollisionData(), PlayerMissile.TargetWorld, CameraClass.getLookAt());
            NewLevel.drawObjects(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "Toon");
            NewLevel.drawWater(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "Water");
            foreach (Controller C in Controllers) C.DebugDraw(CameraClass.getLookAt(), gameFont, WindowManager.SpriteBatch, "Toon");
            EnemyManagement.Draw("Toon");
            LaserManagement.Draw("Toon");
            //PlayerMissile.Draw("Toon");
           // DebugManager.drawDebugBox(PlayerMissile.getCollisionData(), PlayerMissile.GetWorldMatrix(), CameraClass.getLookAt());
            //Bounds.DrawDebug();
            WindowManager.ExplosionSmokeParticles.Draw(gameTime);
            WindowManager.ExplosionParticles.Draw(gameTime);
            WindowManager.SmokeParticles.Draw(gameTime);
            WindowManager.SmokeTrailParticles.Draw(gameTime);
            WindowManager.FireParticles.Draw(gameTime);
            ParticleSystem.Draw(gameTime, WindowManager.GraphicsDevice, CameraClass.getLookAt());

     
            device.SetRenderTarget(0, null);
            device.Clear(Color.Black);
            ApplyPostprocess();
            bloom.calleddrawalready = false;
            bloom.Draw(gameTime);

            WindowManager.SpriteBatch.Begin();
                foreach (Controller C in Controllers) C.DrawText(gameFont, WindowManager.SpriteBatch);
                WindowManager.SpriteBatch.Draw(EnemiesKilled, new Vector2(0, 600 - EnemiesKilled.Height), Color.White);
                WindowManager.SpriteBatch.DrawString(gameFont, EnemyManagement.GetEnemyCount().ToString(), new Vector2(0, 600 - EnemiesKilled.Height) + new Vector2(98, 45), Color.Black);
                if (MissionComplete)
                {
                    Vector2 Size = gameFont.MeasureString("Mission Complete!");
                    WindowManager.SpriteBatch.DrawString(gameFont, "Mission Complete!", new Vector2(400 - Size.X / 2, 300 - Size.Y / 2), Color.Black);
                }
            WindowManager.SpriteBatch.End();
            
        }

        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        void ApplyPostprocess()
        {
            EffectParameterCollection parameters = postprocessEffect.Parameters;
            string effectTechniqueName;


            Vector2 resolution = new Vector2(sceneRenderTarget.Width,
                                             sceneRenderTarget.Height);

            Texture2D normalDepthTexture = normalDepthRenderTarget.GetTexture();

            parameters["EdgeWidth"].SetValue(1);
            parameters["EdgeIntensity"].SetValue(1);
            parameters["ScreenResolution"].SetValue(resolution);
            parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

            // Choose which effect technique to use.

            effectTechniqueName = "EdgeDetect";

            // Activate the appropriate effect technique.
            postprocessEffect.CurrentTechnique =
                                    postprocessEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            WindowManager.SpriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            postprocessEffect.Begin();
            postprocessEffect.CurrentTechnique.Passes[0].Begin();

            WindowManager.SpriteBatch.Draw(sceneRenderTarget.GetTexture(), Vector2.Zero, Color.White);

            WindowManager.SpriteBatch.End();

            postprocessEffect.CurrentTechnique.Passes[0].End();
            postprocessEffect.End();
        }


        public void Die(GameTime gametime)
        {
            int i = 0;

            if (i == 0)
            {
                SoundFX.PauseSound("Engine");
                SoundFX.PauseSound("Boost On");
                SoundFX.PauseSound("Boost Loop");
                SoundFX.PauseSound("Boost Off");
                WindowManager.Game.Components.Remove(bloom);
                Content.Dispose();
                //Content.Unload();
                WindowManager.removeScreen(this);
                WindowManager.findWindow(WindowManager.FindLastScreenPosition()).mode = "Run";
                WindowManager.AddScreen(new MainMenu());
                   
            }
        }

        public void Load(GameTime gameTime)
        {
            DarkOpacity = 0f;
            if (NetClientClass.Client != null && NetClientClass.Client.Status == NetConnectionStatus.Connected)
            {
                if (ContentLoaded == "Done" && SentServerLoaded == false)
                {
                    SentServerLoaded = true;
                    NetMessage NewMessage = new NetMessage();
                    NewMessage.Write((byte)6);
                    NewMessage.Write7BitEncodedUInt(1);
                    NetClientClass.sendMessage(NewMessage);
                   
                }

                if ( NetClientClass.AllClientsLoaded == true)
                {
                    Mode = "Run";
                    WindowManager.findWindow(WindowManager.FindLastScreenPosition()).mode = "Hide";
                }
                NetClientClass.Update();
            }
            else
            {
                if (ContentLoaded == "Done")
                {

                    Mode = "Run";
                    WindowManager.findWindow(WindowManager.FindLastScreenPosition()).mode = "Hide";
                }
            }
        }
    }
}

