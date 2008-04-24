using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace StarForce_PendingTitle_
{
    public class SkySphere
    {
        private Model skyboxMesh;
        public Vector3 myPosition;
        public Quaternion myRotation;
        public Vector3 myScale;

        public float tod = 12; // Mid day
        Effect shader;

        public Vector3 SunPos = new Vector3(0, 1, 0);

        string modelAsset;
        string shaderAsset;
        string textureAsset;

        TextureCube environ;
        Texture2D[] cloudSystem;
        string[] cloudSystemAsset;

        ContentManager content;

        public Color MorningTint = Color.Gold;
        public Color EveningTint = Color.Red;

        private float cloudAnim = .5f;
        /// <summary>
        /// value should be from 0 - 1. Recomend .0001 - .00005
        /// </summary>
        public float cloudAnimSpeed = .00001f;

        public bool ReaTime = false;
        public bool StopTime = false;

        public SkySphere(ContentManager Content, string modelAsset, string shaderAsset, string textureAsset, string[] cloudSystems)
          
        {

            this.content = Content;
            this.modelAsset = modelAsset;
            this.shaderAsset = shaderAsset;
            this.textureAsset = textureAsset;

            myPosition = new Vector3(0, 0, 0);
            myRotation = new Quaternion(0, 0, 0, 1);
            myScale = new Vector3(13000, 13000, 13000);
           // myScale = new Vector3(5, 5, 5);
            cloudSystemAsset = cloudSystems;
        }

        public string GetTime()
        {
            
            string retVal = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            if (!ReaTime)
            {
                retVal = tod.ToString();

                string hr = ((int)tod).ToString();
                string tmn = "0";
                string umn = "0";
                string tsc = "0";
                string usc = "0";

                if (retVal.IndexOf(".") != -1)
                {
                    tmn = tod.ToString().Substring(tod.ToString().IndexOf(".") + 1, 1);
                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 1)
                        umn = tod.ToString().Substring(tod.ToString().IndexOf(".") + 2, 1);

                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 2)
                        tsc = tod.ToString().Substring(tod.ToString().IndexOf(".") + 3, 1);
                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 3)
                        usc = tod.ToString().Substring(tod.ToString().IndexOf(".") + 4, 1);
                }

                int imn = (int)((Convert.ToDecimal(tmn) / 10) * 60) + (int)((Convert.ToDecimal(umn) / 100) * 60);
                int isc = (int)((Convert.ToDecimal(tsc) / 10) * 60) + (int)((Convert.ToDecimal(usc) / 100) * 60);

                retVal = string.Format("{0:00}:{1:00}:{2:00}", hr, imn, isc);
            }

            return retVal;
        }

        public void LoadGraphicsContent(ContentManager Content)
        {
          
                skyboxMesh = content.Load<Model>(modelAsset);
                shader = content.Load<Effect>(shaderAsset);
                environ = content.Load<TextureCube>(textureAsset);

                cloudSystem = new Texture2D[cloudSystemAsset.Length];
                for (int c = 0; c < cloudSystem.Length; c++)
                    cloudSystem[c] = content.Load<Texture2D>(cloudSystemAsset[c]);
           
            
        }

        public void Update(GameTime gameTime)
        {
            //long div = 10000;
            long div = 10000;

            if (!StopTime)
            {
                if (!ReaTime)
                    tod += ((float)gameTime.ElapsedGameTime.Milliseconds / div);
                else
                    tod = ((float)DateTime.Now.Hour) + ((float)DateTime.Now.Minute) / 60 + (((float)DateTime.Now.Second) / 60) / 60;
            }

            if (tod >= 24)
                tod = 0;

            // Calculate the position of the sun based on the time of day.
            float x = 0;
            float y = 0;
            float z = 0;

            if (tod <= 12)
            {
                y = tod / 12;
                x = 12 - tod;
            }
            else
            {
                y = (24 - tod) / 12;
                x = 12 - tod;
            }

            x /= 10;
            SunPos = new Vector3(-x, y, z);

            cloudAnim += cloudAnimSpeed;
            if (cloudAnim > 1)
                cloudAnim = 0;

           
        }

        public void Draw(GameTime gameTime, Matrix ViewMatrix)
        {

            // DisplayModel();

            Matrix World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation)*
                            Matrix.CreateTranslation(CameraClass.Position);



            shader.Parameters["World"].SetValue(World);
            shader.Parameters["View"].SetValue(ViewMatrix);
            shader.Parameters["Projection"].SetValue(CameraClass.getPerspective());
            shader.Parameters["surfaceTexture"].SetValue(environ);

            shader.Parameters["EyePosition"].SetValue(CameraClass.Position);

            shader.Parameters["NightColor"].SetValue(Color.Navy.ToVector4());
            shader.Parameters["SkyColor"].SetValue(Color.LightSkyBlue.ToVector4());
            shader.Parameters["MorningTint"].SetValue(MorningTint.ToVector4());
            shader.Parameters["EveningTint"].SetValue(EveningTint.ToVector4());
            shader.Parameters["timeOfDay"].SetValue(tod);
            shader.Parameters["cloudTimer"].SetValue(cloudAnim);
            shader.Parameters["cloudIntensity"].SetValue(1.0f);

            // Pass the cloud system you want to use.
            shader.Parameters["cloud"].SetValue(cloudSystem[3]);

            for (int pass = 0; pass < shader.CurrentTechnique.Passes.Count; pass++)
            {
                for (int msh = 0; msh < skyboxMesh.Meshes.Count; msh++)
                {
                    ModelMesh mesh = skyboxMesh.Meshes[msh];
                    for (int prt = 0; prt < mesh.MeshParts.Count; prt++)
                        mesh.MeshParts[prt].Effect = shader;
                    mesh.Draw();
                }
            }

          
        }
        public void DisplayModel()
        {
            Matrix[] transforms = new Matrix[skyboxMesh.Bones.Count];
            skyboxMesh.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in skyboxMesh.Meshes)
            {

                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    
                      
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateScale(this.myScale);
                                   //* Matrix.CreateTranslation(CameraClass.Position);


                    effect.View = CameraClass.getLookAt();
                    effect.Projection = CameraClass.getPerspective();

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
    }
}
