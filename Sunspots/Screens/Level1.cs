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
    class Level1 : GameWindow
    {
        LocalPlayer LocalPlayer;
        SpriteFont gameFont;
        Texture2D Dark;
        ParticleSystem ParticleSystem;
        SkySphere skySphere;
        Random Randomizer;
        BloomComponent bloom;
        Level LevelPieces;

        Effect postprocessEffect;
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;

        PointSpriteParticles PointSpriteParticles;
        PointSpriteParticles JetParts;

        public static Vector3 StartingPosition = new Vector3(5100, 358, -7300);
        public static bool MissionComplete;
        TimeSpan VictoryTimer;
        ContentManager Content;

        String ContentLoaded = "None";
        float ContentPercent = 0f;
        bool SentServerLoaded = false;

        Model PlayerModel;

        ObjectCollisionManager ObjColMngr = new ObjectCollisionManager();

        public Level1(Model PlayerModel)
        {
            Mode = "Load";
            CameraClass.setUpCameraClass();
            ParticleSystem = new ParticleSystem();
            Randomizer = new Random((int)DateTime.Now.Ticks);
            MissionComplete = false;
            VictoryTimer = new TimeSpan(0, 0, 0, 3);
            this.PlayerModel = PlayerModel;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Content = new ContentManager(WindowManager.Game.Services);
            gameFont = Content.Load<SpriteFont>("Content\\SpriteFont2");
            Dark = Content.Load<Texture2D>("Content\\Dark");

            // Starts a new thread to perform a task
            ThreadStart ThreadStarter = delegate
             {
                 LoadData();
             };
            Thread myThread = new Thread(ThreadStarter);

            myThread.Start();
        }

        void LoadData()
        {
            ContentLoaded = "Loading Level File";
            LevelData LevelData = LevelData.Load("TestLevel.XML");

            ContentLoaded = "Loading Bloom";
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
            skySphere.LoadGraphicsContent(Content);

            ContentLoaded = "Setting up Debug Manager";
            DebugManager.setUp(WindowManager.GraphicsDevice);

            ContentLoaded = "Setting up Player";
            MainShip MyPlayer;

            Queue<Vector3> WaypointQueue = new Queue<Vector3>();
            foreach (LevelData.WaypointData W in LevelData.Waypoints.WaypointData)
            {
                WaypointQueue.Enqueue(W.Position * 20f);
            }
            //StartingPosition = WaypointQueue.Dequeue();
            MyPlayer = new RailShip(PlayerModel, WaypointQueue);
            ContentLoaded = "Setting up HUD";
            Hud hud = new Hud();
            hud.LoadContent(Content);

            ContentLoaded = "Setting up Level";
            LevelPieces = new Level(Content, WindowManager.GraphicsDevice, cartoonEffect, LevelData);

            ContentLoaded = "Setting up Controllers";
            LocalPlayer = new LocalPlayer(MyPlayer, WindowManager.Controls, hud);

            CameraClass.Position = StartingPosition;
            
            ContentLoaded = "Setting up Particle System";
            ParticleSystem.LoadGraphicsContent(WindowManager.GraphicsDevice, Content);
            PointSpriteParticles = new PointSpriteParticles(Game1.Graphics,
                                                            "Content\\Particle",
                                                            "Content\\Effects\\Particle",
                                                            200
                                                           );
            PointSpriteParticles.Initialize();
            PointSpriteParticles.myPosition = Vector3.Zero;
            PointSpriteParticles.myScale = Vector3.One * 0.15f;
            PointSpriteParticles.RandomColor = false;
            PointSpriteParticles.particleColor = Color.Salmon;
            PointSpriteParticles.SetSystemToStream();
            PointSpriteParticles.RefreshParticles();

            JetParts = new PointSpriteParticles(Game1.Graphics,
                                                "Content\\Particle",
                                                "Content\\Effects\\Particle",
                                                50
                                               );
            JetParts.Initialize();
            JetParts.myPosition = Vector3.Zero;
            JetParts.myScale = Vector3.One * 0.15f;
            JetParts.RandomColor = false;
            JetParts.particleColor = Color.CornflowerBlue;
            JetParts.SetSystemToJet();
            JetParts.RefreshParticles();

            

            ContentLoaded = "Setting up Projectiles";
            
            Model LaserModel = Content.Load<Model>("Content\\Laser\\laser");
            LaserManager LaserManagement = new LaserManager(LaserModel);
            ChangeEffectUsedByModel(LaserModel, cartoonEffect);
            Model MissleModel = Content.Load<Model>("Content\\Missle\\Missle");
            ChangeEffectUsedByModel(MissleModel, cartoonEffect);
            //Model PlayerMissile = new Missle(MissleModel);

            ContentLoaded = "Generating Management Systems";

            //ObjColMngr.EnemyMngr = EnemyManagement;
            ObjColMngr.LaserMngr = LaserManagement;
          

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
            //if (Mode == "Die") Die(gameTime);
        }

        private void Run(GameTime gameTime)
        {
            //CameraClass.Update();
            skySphere.Update(gameTime);
            LocalPlayer.update(gameTime, WindowManager.NewState, WindowManager.OldState);
            ParticleSystem.Update(gameTime);
            PointSpriteUpdate(gameTime);
            ObjColMngr.LaserMngr.Update(gameTime);
        }

        private void PointSpriteUpdate(GameTime gameTime)
        {
            if (LocalPlayer.MainShip.isBoosting())
                PointSpriteParticles.Show = true;
            else
                PointSpriteParticles.Show = false;
            Vector3 pos = LocalPlayer.MainShip.Position;
            Matrix rot = LocalPlayer.MainShip.getNonMovementPositionRotationMatrix();//Matrix.CreateFromQuaternion(Controllers[0].MainShip.NewRotation);
            Vector3 pspos = Vector3.Transform(Vector3.Forward * 15, rot);
            PointSpriteParticles.myPosition = pspos;//Controllers[0].MainShip.Position;
            PointSpriteParticles.myRotation = Quaternion.CreateFromRotationMatrix(rot);//Controllers[0].MainShip.NewRotation;
            PointSpriteParticles.Update(gameTime);

            pspos = Vector3.Transform(Vector3.Backward * 10, rot);
            JetParts.myPosition = pspos;
            JetParts.myRotation = Quaternion.CreateFromRotationMatrix(rot);
            JetParts.Update(gameTime);
        }

        public void GameplayDraw(GameTime gameTime)
        {
            WindowManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            WindowManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            WindowManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            RenderState renderState = WindowManager.GraphicsDevice.RenderState;

            renderState.AlphaBlendEnable = true;
            renderState.AlphaTestEnable = false;
            renderState.DepthBufferEnable = true;

            GraphicsDevice device = WindowManager.GraphicsDevice;

            device.SetRenderTarget(0, normalDepthRenderTarget);
                skySphere.Draw(gameTime, CameraClass.getLookAt());
                LevelPieces.drawObjects(device, CameraClass.getLookAt(), "NormalDepth");
                LevelPieces.drawWater(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "NormalDepth");
                LocalPlayer.DebugDraw(CameraClass.getLookAt(), gameFont, WindowManager.SpriteBatch, "NormalDepth");
                ParticleSystem.Draw(gameTime, WindowManager.GraphicsDevice, CameraClass.getLookAt());
                PointSpriteParticles.Draw(gameTime);
                JetParts.Draw(gameTime);
                ObjColMngr.LaserMngr.Draw("NormalDepth");
                WindowManager.ExplosionSmokeParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.ExplosionParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.SmokeParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.SmokeTrailParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
                WindowManager.FireParticles.SetCamera(CameraClass.getLookAt(), CameraClass.getPerspective());
            device.SetRenderTarget(0, sceneRenderTarget);
                device.Clear(Color.Black);
                skySphere.Draw(gameTime, CameraClass.getLookAt());
                LevelPieces.drawObjects(device, CameraClass.getLookAt(), "Toon");
                LevelPieces.drawWater(WindowManager.GraphicsDevice, CameraClass.getLookAt(), "Toon");
                LocalPlayer.DebugDraw(CameraClass.getLookAt(), gameFont, WindowManager.SpriteBatch, "Toon");
                ObjColMngr.LaserMngr.Draw("Toon");
                WindowManager.ExplosionSmokeParticles.Draw(gameTime);
                WindowManager.ExplosionParticles.Draw(gameTime);
                WindowManager.SmokeParticles.Draw(gameTime);
                WindowManager.SmokeTrailParticles.Draw(gameTime);
                WindowManager.FireParticles.Draw(gameTime);
                ParticleSystem.Draw(gameTime, WindowManager.GraphicsDevice, CameraClass.getLookAt());
                PointSpriteParticles.Draw(gameTime);
                JetParts.Draw(gameTime);
            device.SetRenderTarget(0, null);
            device.Clear(Color.Black);
            ApplyPostprocess();
            bloom.calleddrawalready = false;
            bloom.Draw(gameTime);
        }

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

        public override void Draw(GameTime gameTime)
        {
            if (Mode == "Run")
            {
                GameplayDraw(gameTime);
            }
            if (Mode == "Load")
            {
                WindowManager.GraphicsDevice.Clear(Color.Black);
                WindowManager.SpriteBatch.Begin();
                Vector2 Size = gameFont.MeasureString(ContentLoaded);
                WindowManager.SpriteBatch.DrawString(gameFont, ContentLoaded, new Vector2(400 - Size.X / 2, 300 - Size.Y / 2), Color.White);
                WindowManager.SpriteBatch.End();

            }
        }

        public void Load(GameTime gameTime)
        {
            
            if (ContentLoaded == "Done")
            {
                Mode = "Run";
                WindowManager.findWindow(WindowManager.FindLastScreenPosition()).mode = "Hide";
            }
            
        }
        
    }
}
