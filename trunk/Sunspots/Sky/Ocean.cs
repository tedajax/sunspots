using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    public class Ocean
    {
        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector4 TextureCoordinate;
            public Vector4 TexWeights;

            public static int SizeInBytes = (3 + 3 + 4 + 4) * 4;
            public static VertexElement[] VertexElements = new VertexElement[]
             {
                 new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
                 new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
                 new VertexElement( 0, sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
                 new VertexElement( 0, sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
             };
        }

        private VertexBuffer vb;
        private IndexBuffer ib;
        VertexMultitextured[] myVertices;
        private int myHeight = 128;
        private int myWidth = 128;

        private Vector3 myPosition;
        private Vector3 myScale;
        private Quaternion myRotation;

        Effect effect;

        private Vector3 basePosition;

        public Vector3 Position
        {
            get { return basePosition; }
            set { basePosition = value; }
        }

        /// <summary>
        /// Default 128
        /// </summary>
        public int Height
        {
            get { return myHeight; }
            set { myHeight = value; }
        }
        /// <summary>
        /// Default 128
        /// </summary>
        public int Width
        {
            get { return myWidth; }
            set { myWidth = value; }
        }

        private string myEnvironmentAsset;
        private string myBumpMapAsset;
        private TextureCube myEnvironment;
        private Texture2D my2DEnvironment;
        private Texture2D myBumpMap;
        private Vector4 myDeepWater = new Vector4(0, 0, 1, 1);
        private Vector4 myShallowWater = new Vector4(0, 5, 5, 1);
        private Vector4 myReflection = new Vector4(1, 1, 1, 1);

        private float myBumpHeight = 0.10f;
        private float myHDRMult = 3.0f;
        private float myReflectionAmt = 1.0f;
        private float myWaterColorAmount = 1.0f;
        private float myWaveAmplitude = 1.0f;
        private float myWaveFrequency = 0.1f;

        private bool sparkle;

        /// <summary>
        /// Use HALO sparkle effect
        /// </summary>
        public bool Sparkle
        {
            get { return sparkle; }
            set { sparkle = value; }
        }

        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float BumpMapHeight
        {
            get { return myBumpHeight; }
            set { myBumpHeight = value; }
        }
        /// <summary>
        /// Min 0, Max 100
        /// </summary>
        public float HDRMultiplier
        {
            get { return myHDRMult; }
            set { myHDRMult = value; }
        }
        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float ReflectionAmount
        {
            get { return myReflectionAmt; }
            set { myReflectionAmt = value; }
        }
        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float WaterColorAmount
        {
            get { return myWaterColorAmount; }
            set { myWaterColorAmount = value; }
        }
        /// <summary>
        /// Min 0, Max 10.0
        /// </summary>
        public float WaveAmplitude
        {
            get { return myWaveAmplitude; }
            set { myWaveAmplitude = value; }
        }
        /// <summary>
        /// Min 0, Max 1.0
        /// </summary>
        public float WaveFrequency
        {
            get { return myWaveFrequency; }
            set { myWaveFrequency = value; }
        }

        private float tick = 0.0f;
        private float animSpeed = 0.0f;

        private bool alphaCheck;
        private Color myAlphaBlendColor;

        /// <summary>
        /// Set to get transparent water.
        /// </summary>
        public Color AlphaBlendColor
        {
            get { return myAlphaBlendColor; }
            set
            {
                myAlphaBlendColor = value;
                if (value == Color.Black)
                    alphaCheck = false;
                else
                    alphaCheck = true;
            }
        }

        /// <summary>
        /// Min 0, Max 0.1
        /// </summary>
        public float AnimationSpeed
        {
            get { return animSpeed; }
            set { animSpeed = value; }
        }

        /// <summary>
        /// Color of deep water
        /// </summary>
        public Vector4 DeepWaterColor
        {
            get { return myDeepWater; }
            set { myDeepWater = value; }
        }
        /// <summary>
        /// Color of shallow water
        /// </summary>
        public Vector4 ShallowWaterColor
        {
            get { return myShallowWater; }
            set { myShallowWater = value; }
        }
        /// <summary>
        /// Color of reflection
        /// </summary>
        public Vector4 ReflectionColor
        {
            get { return myReflection; }
            set { myReflection = value; }
        }

        public Vector3 Scale
        {
            get { return myScale; }
            set { myScale = value; }
        }

        public Ocean(string environmentMap, string bumpMap)
        {
            myEnvironmentAsset = environmentMap;
            myBumpMapAsset = bumpMap;
            myWidth = 128;
            myHeight = 128;

            myPosition = new Vector3(0, 0, 0);
            myScale = Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);
        }
       
        public void LoadGraphicsContent(GraphicsDevice myDevice, ContentManager myLoader)
        {
            effect = myLoader.Load<Effect>("Content\\Effects\\Ocean");
            // Textures
            try
            {
                myEnvironment = myLoader.Load<TextureCube>(myEnvironmentAsset);
            }
            catch
            {
                my2DEnvironment = myLoader.Load<Texture2D>(myEnvironmentAsset);
            }
            myBumpMap = myLoader.Load<Texture2D>(myBumpMapAsset);

            myPosition = new Vector3(basePosition.X - (myWidth / 2), basePosition.Y, basePosition.Z - (myHeight / 2));

            // Vertices
            myVertices = new VertexMultitextured[myWidth * myHeight];

            for (int x = 0; x < myWidth; x++)
                for (int y = 0; y < myHeight; y++)
                {
                    myVertices[x + y * myWidth].Position = new Vector3(y, 0, x);
                    myVertices[x + y * myWidth].Normal = new Vector3(0, -1, 0);
                    myVertices[x + y * myWidth].TextureCoordinate.X = (float)x / 30.0f;
                    myVertices[x + y * myWidth].TextureCoordinate.Y = (float)y / 30.0f;
                }

           // vb = new VertexBuffer(myDevice, VertexMultitextured.SizeInBytes * myWidth * myHeight, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
            vb = new VertexBuffer(myDevice, VertexMultitextured.SizeInBytes *myWidth *myHeight, BufferUsage.WriteOnly);
            vb.SetData(myVertices);

           short[] terrainIndices = new short[(myWidth - 1) * (myHeight - 1) * 6];
            for (short x = 0; x < myWidth - 1; x++)
            {
                for (short y = 0; y < myHeight - 1; y++)
                {
                    terrainIndices[(x + y * (myWidth - 1)) * 6] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 1] = (short)((x + 1) + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 2] = (short)(x + y * myWidth);

                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 4] = (short)(x + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 5] = (short)(x + (y + 1) * myWidth);
                }
            }

            //ib = new IndexBuffer(myDevice, typeof(short), (myWidth - 1) * (myHeight - 1) * 6, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
            ib = new IndexBuffer(myDevice, typeof(short), (myWidth - 1) * (myHeight - 1) * 6, BufferUsage.WriteOnly);
            ib.SetData(terrainIndices);            
        }

        public void Draw(GraphicsDevice myDevice)
        {
            Matrix World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(myPosition);
            Matrix WVP = World * CameraClass.getLookAt() * CameraClass.getPerspective();
            Matrix WV = World * CameraClass.getLookAt();
            Matrix viewI = Matrix.Invert(CameraClass.getLookAt());

            effect.Parameters["world"].SetValue(World);
            effect.Parameters["wvp"].SetValue(WVP);
            effect.Parameters["worldView"].SetValue(WV);
            effect.Parameters["viewI"].SetValue(viewI);

            effect.Parameters["normalMap"].SetValue(myBumpMap);
            effect.Parameters["sparkle"].SetValue(sparkle);
            if (myEnvironment != null)
                effect.Parameters["cubeMap"].SetValue(myEnvironment);
            else
                effect.Parameters["cubeMap"].SetValue(my2DEnvironment);
            effect.Parameters["deepColor"].SetValue(myDeepWater);
            effect.Parameters["shallowColor"].SetValue(myShallowWater);
            effect.Parameters["reflectionColor"].SetValue(myReflection);
            effect.Parameters["time"].SetValue(tick += animSpeed);

            effect.Parameters["bumpHeight"].SetValue(myBumpHeight);
            effect.Parameters["hdrMultiplier"].SetValue(myHDRMult);
            effect.Parameters["reflectionAmount"].SetValue(myReflectionAmt);
            effect.Parameters["waterAmount"].SetValue(myWaterColorAmount);
            effect.Parameters["waveAmp"].SetValue(myWaveAmplitude);
            effect.Parameters["waveFreq"].SetValue(myWaveFrequency);

            bool alphaTest = myDevice.RenderState.AlphaTestEnable;
            bool alphaBlend = myDevice.RenderState.AlphaBlendEnable;
            CompareFunction alphaFunc = myDevice.RenderState.AlphaFunction;
            Blend sourceBlend = myDevice.RenderState.SourceBlend;
            Blend destinationBlend = myDevice.RenderState.DestinationBlend;
            Color blendFator = myDevice.RenderState.BlendFactor;

            if (alphaCheck)
            {
                if (myDevice.RenderState.AlphaTestEnable != true)
                    myDevice.RenderState.AlphaTestEnable = true;
                if (myDevice.RenderState.AlphaBlendEnable != true)
                    myDevice.RenderState.AlphaBlendEnable = true;
                if (myDevice.RenderState.AlphaFunction != CompareFunction.NotEqual)
                    myDevice.RenderState.AlphaFunction = CompareFunction.NotEqual;

                if (myDevice.RenderState.SourceBlend != Blend.BlendFactor)
                    myDevice.RenderState.SourceBlend = Blend.BlendFactor;
                if (myDevice.RenderState.DestinationBlend != Blend.One)
                    myDevice.RenderState.DestinationBlend = Blend.One;
                if (myDevice.RenderState.BlendFactor != myAlphaBlendColor)
                    myDevice.RenderState.BlendFactor = myAlphaBlendColor;

            }
            effect.Begin(SaveStateMode.SaveState);
            for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
            {
                effect.CurrentTechnique.Passes[p].Begin();

                myDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
                myDevice.Indices = ib;
                myDevice.VertexDeclaration = new VertexDeclaration(myDevice, VertexMultitextured.VertexElements);
                myDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, myWidth * myHeight, 0, (myWidth - 1) * (myHeight - 1) * 2);

                effect.CurrentTechnique.Passes[p].End();
            }
            effect.End();

            if (alphaCheck)
            {
                if (myDevice.RenderState.AlphaTestEnable != alphaTest)
                    myDevice.RenderState.AlphaTestEnable = alphaTest;
                if (myDevice.RenderState.AlphaBlendEnable != alphaBlend)
                    myDevice.RenderState.AlphaBlendEnable = alphaBlend;
                if (myDevice.RenderState.AlphaFunction != alphaFunc)
                    myDevice.RenderState.AlphaFunction = alphaFunc;

                if (myDevice.RenderState.SourceBlend != sourceBlend)
                    myDevice.RenderState.SourceBlend = sourceBlend;
                if (myDevice.RenderState.DestinationBlend != destinationBlend)
                    myDevice.RenderState.DestinationBlend = destinationBlend;
                if (myDevice.RenderState.BlendFactor != blendFator)
                    myDevice.RenderState.BlendFactor = blendFator;
            }
        }
    }
}
