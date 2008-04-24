using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    public class TerrainManager
    {

        public struct VertexMultiTextured
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


        private Effect effect;
        private VertexBuffer terrainVertexBuffer;
        private IndexBuffer terrainIndexBuffer;
        Texture2D heightMap;
        private int WIDTH;
        private int HEIGHT;
        private float[,] heightData;

        
        private Texture2D grassTexture;
        private Texture2D sandTexture;
        private Texture2D rockTexture;
        private Texture2D snowTexture;
        
        
        //Water Stuff
        RenderTarget2D refractionRenderTarg;
        Texture2D refractionMap;

        private RenderTarget2D reflectionRenderTarg;
        private Texture2D reflectionMap;
        private Matrix reflectionViewMatrix;

        private VertexPositionTexture[] waterVertices;

        private Texture2D waterBumpMap;


        public TerrainManager(GraphicsDevice device)
        {
            refractionRenderTarg = new RenderTarget2D(device,
                 800,
                 600,
                 1,
               SurfaceFormat.Color);
            reflectionRenderTarg = new RenderTarget2D(device, 800,600, 1, SurfaceFormat.Color);
        }

        public void LoadGraphicsContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            effect = Content.Load<Effect>("Content\\Effects\\Series4Effects");
            //heightMap = Content.Load<Texture2D>("Content\\demolevel");
            //grassTexture = Content.Load<Texture2D>("Content\\CityLevel\\grass");
            //sandTexture = Content.Load<Texture2D>("Content\\CityLevel\\sand");
            //rockTexture = Content.Load<Texture2D>("Content\\CityLevel\\rock");
            //snowTexture = Content.Load<Texture2D>("Content\\CityLevel\\snow");
            waterBumpMap = Content.Load<Texture2D>("Content\\waterbump");
         
            /*
             LoadHeightData();

            SetUpTerrainterrainVertices(graphicsDevice);
            SetUpTerrainterrainIndices(graphicsDevice);*/
            SetUpWaterVertices();

        }

        public void Draw(GraphicsDevice device)
        {
            //DrawRefractionMap(device);
           // DrawTerrain(device,CameraClass.getLookAt());
            
            
        }

        public void DrawWater(GraphicsDevice device)
        {
            effect.CurrentTechnique = effect.Techniques["Water"];
            Matrix worldMatrix = Matrix.Identity;
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(CameraClass.getLookAt());
            effect.Parameters["xReflectionView"].SetValue(reflectionViewMatrix);
            effect.Parameters["xProjection"].SetValue(CameraClass.getPerspective());
            effect.Parameters["xReflectionMap"].SetValue(reflectionMap);
            effect.Parameters["xRefractionMap"].SetValue(refractionMap);
            effect.Parameters["xWaterBumpMap"].SetValue(waterBumpMap);
            effect.Parameters["xWaveLength"].SetValue(0.1f);
            effect.Parameters["xWaveHeight"].SetValue(0.1f);
            effect.Parameters["xCamPos"].SetValue(CameraClass.Position);


            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
                device.DrawUserPrimitives(PrimitiveType.TriangleList, waterVertices, 0, 2);
                pass.End();
            }
            effect.End();
        }

        public void DrawTerrain(GraphicsDevice device, Matrix ViewMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xSandTexture"].SetValue(sandTexture);
            effect.Parameters["xGrassTexture"].SetValue(grassTexture);
            effect.Parameters["xRockTexture"].SetValue(rockTexture);
            effect.Parameters["xSnowTexture"].SetValue(snowTexture);
            Matrix worldMatrix = Matrix.Identity * Matrix.CreateScale(100f);
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(ViewMatrix);
            effect.Parameters["xProjection"].SetValue(CameraClass.getPerspective());
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -0.5f, -1));
            effect.Parameters["xAmbient"].SetValue(1.0f);
            effect.Parameters["xTexture"].SetValue(grassTexture);

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.Vertices[0].SetSource(terrainVertexBuffer, 0, VertexMultiTextured.SizeInBytes);
                device.Indices = terrainIndexBuffer;
                device.VertexDeclaration = new VertexDeclaration(device, VertexMultiTextured.VertexElements);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, WIDTH * HEIGHT, 0, (WIDTH - 1) * (HEIGHT - 1) * 2);

                pass.End();
            }
            effect.End();
           
        }

        public Matrix DrawReflectionMapBegin(GraphicsDevice device)
        {
            float waterHeight = 5f*20f;
            float reflectionCamYCoord = -1*CameraClass.Position.Y + 2 * waterHeight;
           // reflectionCamYCoord= -CameraClass.Position.Y;
            Vector3 reflectionCamPosition = new Vector3(CameraClass.Position.X, reflectionCamYCoord, CameraClass.Position.Z);

           float reflectionTargetYCoord = -CameraClass.CameraPointingAt.Y + 2 * waterHeight;
           // reflectionTargetYCoord =waterHeight;
            Vector3 reflectionCamTarget = new Vector3(CameraClass.CameraPointingAt.X, reflectionTargetYCoord, CameraClass.CameraPointingAt.Z);
           
            Vector3 forwardVector = reflectionCamTarget - reflectionCamPosition;
            Vector3 sideVector = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateFromQuaternion(CameraClass.Rotation));
            //Vector3 sideVector = new Vector3(1, 0, 0);
            Vector3 reflectionCamUp = Vector3.Cross(sideVector, forwardVector);

            reflectionViewMatrix = Matrix.CreateLookAt(reflectionCamPosition, reflectionCamTarget, reflectionCamUp);

            Vector3 planeNormalDirection = new Vector3(0, 1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, -1*waterHeight+(1*20f));

            Matrix camMatrix = reflectionViewMatrix * CameraClass.getPerspective();
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane refractionClipPlane = new Plane(planeCoefficients);

            device.ClipPlanes[0].Plane = refractionClipPlane;
            device.ClipPlanes[0].IsEnabled = true;

            device.SetRenderTarget(0, reflectionRenderTarg);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //we return the view matrix and let the game draw the scene
            return reflectionViewMatrix;
        }

        public void DrawReflectionMapEnd(GraphicsDevice device)
        {
            //Knowing that the scene was drawn from underwater we now save the texture and return things back to normal
            device.SetRenderTarget(0, null);
            reflectionMap = reflectionRenderTarg.GetTexture();

            device.ClipPlanes[0].IsEnabled = false;

            //reflectionMap.Save("reflectionmap.jpg", ImageFileFormat.Jpg);
        }

        public void DrawRefractionMap(GraphicsDevice device)
        {
            Vector3 planeNormalDirection = new Vector3(0, -1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, 5.0f*20f+(0.5f*20f));

            Matrix camMatrix = CameraClass.getLookAt() * CameraClass.getPerspective();
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane refractionClipPlane = new Plane(planeCoefficients);

            device.ClipPlanes[0].Plane = refractionClipPlane;
            device.ClipPlanes[0].IsEnabled = true;

            device.SetRenderTarget(0, refractionRenderTarg);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            //DrawTerrain(device, CameraClass.getLookAt());
            device.SetRenderTarget(0, null);
            refractionMap = refractionRenderTarg.GetTexture();

            device.ClipPlanes[0].IsEnabled = false;
           

            //refractionMap.Save("refractionmap.jpg", ImageFileFormat.Jpg);
           

        }


        private void SetUpWaterVertices()
        {

            WIDTH = 1024;
            HEIGHT = 1024;
            float waterHeight = 20f;
            waterVertices = new VertexPositionTexture[6];

            waterVertices[0] = new VertexPositionTexture(new Vector3(0, waterHeight, 0), new Vector2(0, 1));
            waterVertices[2] = new VertexPositionTexture(new Vector3(WIDTH,  waterHeight, HEIGHT), new Vector2(1, 0));
            waterVertices[1] = new VertexPositionTexture(new Vector3(0,waterHeight,HEIGHT ), new Vector2(0, 0));

            waterVertices[3] = new VertexPositionTexture(new Vector3(0,waterHeight, 0 ), new Vector2(0, 1));
            waterVertices[5] = new VertexPositionTexture(new Vector3(WIDTH, waterHeight,0), new Vector2(1, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(WIDTH, waterHeight, HEIGHT), new Vector2(1, 0));
        }
        private void SetUpTerrainterrainVertices(GraphicsDevice device)
        {
            VertexMultiTextured[] terrainVertices = new VertexMultiTextured[WIDTH * HEIGHT];
            

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    terrainVertices[x + y * WIDTH].Position = new Vector3(x, heightData[x, y], y);
                    terrainVertices[x + y * WIDTH].Normal = new Vector3(0, 1, 0);
                    terrainVertices[x + y * WIDTH].TextureCoordinate.X = (float)x / 30f;
                    terrainVertices[x + y * WIDTH].TextureCoordinate.Y = (float)y / 30f;

                    terrainVertices[x + y * WIDTH].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 0) / 8.0f, 0, 1);
                    terrainVertices[x + y * WIDTH].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 12) / 6.0f, 0, 1);
                    terrainVertices[x + y * WIDTH].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 20) / 6.0f, 0, 1);
                    terrainVertices[x + y * WIDTH].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 30) / 6.0f, 0, 1);


                    float totalWeight = terrainVertices[x + y * WIDTH].TexWeights.X;
                    totalWeight += terrainVertices[x + y * WIDTH].TexWeights.Y;
                    totalWeight += terrainVertices[x + y * WIDTH].TexWeights.Z;
                    totalWeight += terrainVertices[x + y * WIDTH].TexWeights.W;
                    terrainVertices[x + y * WIDTH].TexWeights.X /= totalWeight;
                    terrainVertices[x + y * WIDTH].TexWeights.Y /= totalWeight;
                    terrainVertices[x + y * WIDTH].TexWeights.Z /= totalWeight;
                    terrainVertices[x + y * WIDTH].TexWeights.W /= totalWeight;
                }

            for (int x = 1; x < WIDTH - 1; x++)
            {
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    Vector3 normX = new Vector3((terrainVertices[x - 1 + y * WIDTH].Position.Z - terrainVertices[x + 1 + y * WIDTH].Position.Z) / 2, 0, 1);
                    Vector3 normY = new Vector3(0, (terrainVertices[x + (y - 1) * WIDTH].Position.Z - terrainVertices[x + (y + 1) * WIDTH].Position.Z) / 2, 1);
                    terrainVertices[x + y * WIDTH].Normal = normX + normY;
                    terrainVertices[x + y * WIDTH].Normal.Normalize();
                }
            }

            terrainVertexBuffer = new VertexBuffer(device, VertexMultiTextured.SizeInBytes * WIDTH * HEIGHT, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(terrainVertices);
        }

        private void SetUpTerrainterrainIndices(GraphicsDevice device)
        {
            int[] terrainIndices = new int[(WIDTH - 1) * (HEIGHT - 1) * 6];
            for (int x = 0; x < WIDTH - 1; x++)
            {
                for (int y = 0; y < HEIGHT - 1; y++)
                {
                    terrainIndices[(x + y * (WIDTH - 1)) * 6] = (x + 1) + (y + 1) * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 1] = (x + 1) + y * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 2] = x + y * WIDTH;

                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 3] = (x + 1) + (y + 1) * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 4] = x + y * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 5] = x + (y + 1) * WIDTH;
                }
            }

            terrainIndexBuffer = new IndexBuffer(device, typeof(int), (WIDTH - 1) * (HEIGHT - 1) * 6, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(terrainIndices);
        }

        private void LoadHeightData()
        {
            float minimumHeight = 255;
            float maximumHeight = 0;

            WIDTH = heightMap.Width;
            HEIGHT = heightMap.Height;
            Color[] heightMapColors = new Color[WIDTH * HEIGHT];
            heightMap.GetData(heightMapColors);

            heightData = new float[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * WIDTH].R;
                  if (heightData[x, y] < minimumHeight) minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight) maximumHeight = heightData[x, y];
                }

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                   heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30;

            //CollisionManager.addHeightMap(heightData, 100f);
        }


    }
}
