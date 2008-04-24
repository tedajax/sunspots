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
    class LevelSelect : GameWindow
    {
        string SystemName;
        SpriteFont InfoFont;
        bool SelectPressed = false;

        Effect postprocessEffect;
        Effect cartoonEffect;

        BloomComponent Bloom;
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;

        List<Planet> Planets;

        SkySphere skySphere;

        CollisionManager colmanager; //Don't actually use this but obj3d automatically references it...

        ParticleSystem Particles;

        Vector3 CameraRotation = new Vector3();
        float CameraDistance = 100f;

        int OldMouseX;
        int OldMouseY;
        int OldScrollWheel;

        Obj3d Mothership;
        Vector3 MothershipRotation = new Vector3();
        
        public LevelSelect(string sysname)
        {
            SystemName = sysname;

            Mode = "Setup";

            Planets = new List<Planet>();

            colmanager = new CollisionManager();

            CameraClass.setUpCameraClass();
        }

        public override void LoadContent()
        {
            InfoFont = WindowManager.Content.Load<SpriteFont>("Content\\SpriteFont2");

            string[] clouds = { "Content//Sky//clouds00", "Content//Sky//clouds0", "Content//Sky//clouds1", "Content//Sky//clouds2" };
            skySphere = new SkySphere(WindowManager.Content, "Content//Sky//InvSphere", "Content//Sky//dynamicSkybox", "Content//Sky//cubeMap", clouds);

            //setup the basic effect for debugging
            skySphere.LoadGraphicsContent(WindowManager.Content);
            skySphere.tod = 23;
            skySphere.StopTime = true;

            Particles = new ParticleSystem();
            Particles.LoadGraphicsContent2(WindowManager.GraphicsDevice, WindowManager.Content);

            

            // Create two custom rendertargets.
            PresentationParameters pp = WindowManager.GraphicsDevice.PresentationParameters;

            sceneRenderTarget = new RenderTarget2D(WindowManager.GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);

            normalDepthRenderTarget = new RenderTarget2D(WindowManager.GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);
        }

        public override void Update(GameTime gameTime)
        {
            if (Mode.Equals("Setup")) Setup();
            if (Mode.Equals("PlanetSelect")) PlanetSelectRun(gameTime);
            if (Mode.Equals("LevelSelect")) LevelSelectRun(gameTime);
            if (Mode.Equals("Die")) Die();

            skySphere.Update(gameTime);
            CameraClass.Update();
        }

        private void Setup()
        {
            //set up the other effects
            postprocessEffect = WindowManager.Content.Load<Effect>("Content\\Effects\\PostprocessEffect");
            cartoonEffect = WindowManager.Content.Load<Effect>("Content\\Effects\\CartoonEffect");

            Bloom = new BloomComponent(WindowManager.Game);
            WindowManager.Game.Components.Add(Bloom);
            Bloom.Settings = BloomSettings.PresetSettings[6];

            StreamReader sr = new StreamReader("Systems\\" + SystemName + "\\PlanetList.DAT");

            while (!sr.EndOfStream)
            {
                Planet newplanet;
                string nm = sr.ReadLine();
                Model planetmodel = WindowManager.Content.Load<Model>("Content\\Models\\Planets\\" + nm.ToLower());
                if (!ModelChanged(nm))
                    ChangeEffectUsedByModel(planetmodel, cartoonEffect);

                string rad = sr.ReadLine();
                float radius = float.Parse(rad);

                string revrt = sr.ReadLine();
                float revrate = float.Parse(revrt);

                string rotrt = sr.ReadLine();
                float rotrate = float.Parse(rotrt);

                newplanet = new Planet(nm, planetmodel, radius, revrate, rotrate);
                Planets.Add(newplanet);
            }

            sr.Close();

            Model YourMom = WindowManager.Content.Load<Model>("Content\\agagemnemon");
            ChangeEffectUsedByModel(YourMom, cartoonEffect);
            Mothership = new Obj3d(YourMom);
            Mothership.setScale(.07f);

            Mode = "PlanetSelect";
            if (WindowManager.Controls.getShoot() > 0)
            {
                SelectPressed = true;
            }
        }

        private void PlanetSelectRun(GameTime gameTime)
        {
            Vector3 tempvector = new Vector3(0, 0, -CameraDistance);
            tempvector = Vector3.Transform(tempvector, CreateFromVector3(CameraRotation));
            CameraClass.Position = tempvector;
            CameraClass.CameraPointingAt = Vector3.Zero;

            if (WindowManager.Controls.getShoot() > 0 && !SelectPressed)
            {
                WindowManager.AddScreen(new Playing(), WindowManager.FindLastScreenPosition() - 1);
                SelectPressed = true;
                WindowManager.removeScreen(this);
            }

            if (WindowManager.Controls.getShoot() == 0)
                SelectPressed = false;

            CameraRotation.Y += (float)(Mouse.GetState().X - OldMouseX) / 64f;
            CameraRotation.X += (float)(Mouse.GetState().Y - OldMouseY) / 64f;
                        
            CameraDistance += (float)(Mouse.GetState().ScrollWheelValue - OldScrollWheel) / 64f;

            foreach (Planet p in Planets)
            {
                p.Update(gameTime);
            }
            Vector3 MotherShipPosition = new Vector3(0, 0, -5);
            MothershipRotation.Y += 0.05f;
            MotherShipPosition = Vector3.Transform(MotherShipPosition, CreateFromVector3(MothershipRotation));
            Mothership.setPosition(Planets[0].GetPosition() + MotherShipPosition);
            Mothership.setRotation(MothershipRotation + new Vector3(0, MathHelper.PiOver2, 0));

            OldMouseX = Mouse.GetState().X;
            OldMouseY = Mouse.GetState().Y;
            OldScrollWheel = Mouse.GetState().ScrollWheelValue;
        }

        private void LevelSelectRun(GameTime gameTime)
        {
        }

        private void Die()
        {
            WindowManager.Game.Components.Remove(Bloom);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Mode.Equals("Setup"))
            {
                WindowManager.SpriteBatch.Begin();
                DrawCenterString(Game1.CenterScreen, "Loading System Data");
                WindowManager.SpriteBatch.End();
            }
            if (Mode.Equals("PlanetSelect"))
            {
                WindowManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                WindowManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

                RenderState renderState = WindowManager.GraphicsDevice.RenderState;

                renderState.AlphaBlendEnable = true;
                renderState.AlphaTestEnable = false;
                renderState.DepthBufferEnable = true;

                GraphicsDevice device = WindowManager.GraphicsDevice;
                device.SetRenderTarget(0, normalDepthRenderTarget);

                foreach (Planet p in Planets)
                {
                    p.Draw("NormalDepth");
                }
                Mothership.DisplayModel(CameraClass.getLookAt(), "NormalDepth", Vector3.Zero);
                Particles.Draw2(gameTime, WindowManager.GraphicsDevice);
                device.SetRenderTarget(0, sceneRenderTarget);
                device.Clear(Color.Black);

                skySphere.Draw(gameTime, CameraClass.getLookAt());

                foreach (Planet p in Planets)
                {
                    p.Draw("Toon");
                }
                Mothership.DisplayModel(CameraClass.getLookAt(), "Toon", Vector3.Zero);
                Particles.Draw2(gameTime, WindowManager.GraphicsDevice);

                device.SetRenderTarget(0, null);
                device.Clear(Color.Black);
                ApplyPostprocess();
                Bloom.calleddrawalready = false;
                Bloom.Draw(gameTime);

                Planets[0].Draw2D(WindowManager.SpriteBatch, InfoFont);
            }
            if (Mode.Equals("LevelSelect"))
            {
                WindowManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                WindowManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

                RenderState renderState = WindowManager.GraphicsDevice.RenderState;

                renderState.AlphaBlendEnable = true;
                renderState.AlphaTestEnable = false;
                renderState.DepthBufferEnable = true;
            }
        }

        private void DrawCenterString(Vector2 Position, string str)
        {
            WindowManager.SpriteBatch.DrawString(InfoFont, str,
                                                 Position,
                                                 Color.White,
                                                 0f,
                                                 InfoFont.MeasureString(str) / 2,
                                                 1f,
                                                 SpriteEffects.None,
                                                 0);
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

        private bool ModelChanged(string modelname)
        {
            foreach (Planet p in Planets)
            {
                if (p.Name.Equals(modelname))
                    return true;
            }

            return false;
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

        private Matrix CreateFromVector3(Vector3 create)
        {
            return Matrix.CreateFromYawPitchRoll(create.Y, create.X, create.Z);
        }
    }
}
