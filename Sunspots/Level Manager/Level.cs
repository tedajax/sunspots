using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;


namespace StarForce_PendingTitle_
{
    class Level
    {
       

        public Obj3d[] StaticObjects;


        private VertexPositionNormalTexture[] waterVertices;

        Ocean ocean;

        Effect WaterEffect;

        float time=0f;
        float animspeed = 1f;

        //private Effect WaterEffect;

        public Level(ContentManager Content, GraphicsDevice graphicsDevice, Effect newEffect, LevelData LevelData)
        {
           // float scaling = 5f;

            WaterEffect = Content.Load<Effect>("Content\\Effects\\Pocean");
            WaterEffect.Parameters["texNormal"].SetValue(Content.Load<Texture2D>("Content\\Water\\NormalMap"));
            WaterEffect.Parameters["texSpecularMask"].SetValue(Content.Load<Texture2D>("Content\\Water\\SpecularMask"));
            WaterEffect.Parameters["texAlpha"].SetValue(Content.Load<Texture2D>("Content\\Water\\AlphaMap"));
            SetUpWaterVertices();
       
            StaticObjects = new Obj3d[LevelData.LevelObjects.Count];
            int count = 0;
            foreach (LevelData.Generic3DObject G in LevelData.LevelObjects)
            {
                Model NewModel = Content.Load<Model>("Content\\"+G.ContentName);
                Playing.ChangeEffectUsedByModel(NewModel, newEffect);
                StaticObjects[count] = new Obj3d(NewModel);
                StaticObjects[count].setPosition(G.Position*20f);
                StaticObjects[count].setRotation(G.Rotation);
                StaticObjects[count].setScale(20f);
                count++;
            }

            
            Model Model = Content.Load<Model>("Content\\LevelObjects\\demolevel2");
           
            Dictionary<string, object> tagData = Model.Tag as Dictionary<string, object>;
            List<Vector3> Verticies = tagData["Verticies"] as List<Vector3>;
            ConvertVerticies(Verticies);

      
          
            
           // StaticObjects[0] = new Obj3d(Model, Vector3.Zero,Vector3.Zero, CollisionBoxes.ToArray());
            //StaticObjects[0].setScale(scaling);
    

            
            
        }

        void ConvertVerticies(List<Vector3> vertices)
        {

            int MaxX=-1000;
            int MaxZ=-1000;
            int MinX = 1000;
            int MinZ = 1000;

            List<float> DifferencesFound = new List<float>();
            /* Note -217, 250 does match up with the real value for -217,250 in the verticies values.
             * that means that the first value its storing is -250,250 which should be offsetted accordingly when checking 
             * collision*/
           
           /* foreach (Vector3 V in vertices)
            {
                if (V.Z > MaxZ) MaxZ = (int)V.Z;
                if (V.X > MaxX) MaxX = (int)V.X;
                if (V.X < MinX) MinX = (int)V.X;
                if (V.Z < MinZ) MinZ = (int)V.Z;
              
            }
            for (int i = 1; i < vertices.Count; i++)
            {
                float difference = (vertices[i].Z - vertices[i - 1].Z);
                bool alreadyinlist = false;
                foreach (int D in DifferencesFound)
                {
                    if (difference == D) alreadyinlist = true;
                }
                if (!alreadyinlist) DifferencesFound.Add(difference);
                difference = (vertices[i].X - vertices[i - 1].X);
                 alreadyinlist = false;
                foreach (int D in DifferencesFound)
                {
                    if (difference == D) alreadyinlist = true;
                }
                if (!alreadyinlist) DifferencesFound.Add(difference);
            }*/

            float TopLeftCorner = vertices[0].Z;
            List<Vector3> VertList = new List<Vector3>();
            int VertGrabbed = 0;

            Quad[,] heightData = new Quad[75, 75];
            int xpos = 0;
            int zpos = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                
                VertList.Add(vertices[i]);
                VertGrabbed++;
                if (VertGrabbed == 6)
                {
                    VertGrabbed = 0;
                    Quad newQuad = new Quad(VertList);
                    if (VertList[0].Z != TopLeftCorner)
                    {
                        zpos++;
                        xpos = 0;

                    }
                    heightData[xpos, zpos] = newQuad;
                    
                        xpos++;
                        if (xpos == 63) { int jooga = 0; }
               
                    
                    TopLeftCorner = VertList[0].Z;
                    VertList = new List<Vector3>();

                }
            }



            CollisionManager.addHeightMap(heightData, 20f, new Vector3(135,0,2));
    
        }

        public byte[] CreateSkyTexture()
        {
               Random rand = new Random(500);
            float[] map32 = new float[32*32];
            float[] map256 = new float[256*256];
            //SetNoise(ref map32);
            //
            for (int x = 0; x<5000;x++)
            {
                double ang = MathHelper.ToRadians(x);
                int rad = rand.Next(30);
                //Box((int)(128 + (Math.Sin(ang) * rad)),(int)(128 + (Math.Cos(ang) * rad)),(int)( rand.Next(3) + 128 + (Math.Sin(ang) * rad)),(int)( rand.Next(3) + 128+ (Math.Cos(ang) * rad)), ref map256);
                Box(100,100,200,200, ref map256);
            }
        
            //OverlapOctaves(ref map32, ref map256);

            //ExpFilter(ref map256);

            byte[] data = new byte[256*256 * 4];
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    data[4 * (x + (256 * y)) + 0] = (byte)map256[(x + (256 * y))]; /* blue */
                    data[4 * (x + (256 * y)) + 1] = (byte)map256[(x + (256 * y))]; /* green */
                    data[4 * (x + (256 * y)) + 2] = (byte)map256[(x + (256 * y))]; /* red */
                    if (map256[(x + (256 * y))] > 100)
                    {
                        data[4 * (x + (256 * y)) + 3] = (byte)255; /* alpha */
                    }
                    else
                    {
                        data[4 * (x + (256 * y)) + 3] = (byte)255;
                    }
                   
                  
                }
            }
            return data;
            /*
        
            int startx = 128 - 32;
            int starty = 128 - 32;
            byte[] data2 = new byte[64 * 64 * 4];
            int counter = 0;
            for (int y = starty; y < starty + 64; y++)
            {
                for (int x = startx; x < startx + 64; x++)
                {
                    data2[4 * (counter) + 0] = data[4 * (x + 256 * y) + 0];
                    data2[4 * (counter) + 1] = data[4 * (x + 256 * y) + 1];
                    data2[4 * (counter) + 2] = data[4 * (x + 256 * y) + 2];
                    data2[4 * (counter) + 3] = data[4 * (x + 256 * y) + 3];
                    counter++;
                }
            }
            return data2;*/
           

        }


        void ExpFilter(ref float[] map)
        {
            float cover = 20.0f;
            float sharpness = 0.95f;

            for (int x = 0; x < 256 * 256; x++)
            {
                float c = map[x] - (255.0f - cover);
                if (c < 0) c = 0;
                map[x] = 255.0f - ((float)(Math.Pow(sharpness, c)) * 255.0f);
            }
        }

        void OverlapOctaves(ref float[] map32,ref float[] map256)
        {
            for (int x = 0; x < 256 * 256; x++)
            {
                map256[x] = 0;
            }
            for (int octave=0; octave<4; octave++)
                for (int x=0; x<256; x++)
                    for (int y = 0; y < 256; y++)
                    {
                        float scale = 1 / (float) Math.Pow(2, 3 - octave);
                        float noise = Interpolate(x * scale, y * scale,ref map32);
                        map256[(y * 256) + x] += noise / (float)Math.Pow(2, octave);
                    }

        }

        float Interpolate(float x, float y, ref float[] map)
        {
            int Xint = (int)x;
            int Yint = (int)y;

            float Xfrac = x - Xint;
            float Yfrac = y - Yint;

            int X0 = Xint % 32;
            int Y0 = Yint % 32;
            int X1 = (Xint + 1) % 32;
            int Y1 = (Yint + 1) % 32;

            float bot = map[X0 * 32 + Y0] + Xfrac * (map[X1 * 32 + Y0] - map[X0 * 32 + Y0]);
            float top = map[X0 * 32 + Y1] + Xfrac * (map[X1 * 32 + Y1] - map[X0 * 32 + Y1]);

            return (bot + Yfrac * (top - bot));

        }

        void SetNoise(ref float[] map)
        {
            float[,] temp = new float[34, 34];
            //Generate Noise
            Random rand = new Random();
            int random = rand.Next() % 5000;
            for (int y=1; y<33; y++) 
              for (int x=1; x<33; x++)
              {
                temp[x,y] = 128.0f + Noise(x,  y,  random)*128.0f;
              }
            //Fix Borders problem by mirroring
              for (int x = 1; x < 33; x++)
              {
                  temp[0,x] = temp[32,x];
                  temp[33,x] = temp[1,x];
                  temp[x,0] = temp[x,32];
                  temp[x,33] = temp[x,1];
              }
              temp[0,0] = temp[32,32];
              temp[33,33] = temp[1,1];
              temp[0,33] = temp[32,1];
              temp[33,0] = temp[1,32];
            //smooth
              for (int y = 1; y < 33; y++)
                  for (int x = 1; x < 33; x++)
                  {
                      float center = temp[x,y] / 4.0f;
                      float sides = (temp[x + 1,y] + temp[x - 1,y] + temp[x,y + 1] + temp[x,y - 1]) / 8.0f;
                      float corners = (temp[x + 1,y + 1] + temp[x + 1,y - 1] + temp[x - 1,y + 1] + temp[x - 1,y - 1]) / 16.0f;

                      map[((x - 1) * 32) + (y - 1)] = center + sides + corners;
                  }

        }

        float Noise(int x, int y, int random)
        {
            int n = x + y * 57 + random * 131;
            n = (n << 13) ^ n;
            return (1.0f - ((n * (n * n * 15731 + 789221) +
                    1376312589) & 0x7fffffff) * 0.000000000931322574615478515625f);
        }

        public void Box(int x1, int y1, int x2, int y2,ref float[] map256)
        {
            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    map256[(x + (256 * y))] = 255;
                }
            }

        }

        public Obj3d[] getStaticObjects() { return StaticObjects; }

     

        public void drawObjects(GraphicsDevice device,Matrix ViewMatrix, string technique)
        {
            foreach (Obj3d o in StaticObjects)
            {
                //we need to switch this to use ViewMatrix
                o.DisplayModel(ViewMatrix, technique, Vector3.Zero);
            }
           // ocean.Draw(device);
          //  Terrain.DrawTerrain(device, ViewMatrix);
           
        }

        int Width = 500*2;
        int Height = 500*2;
        int NumberOfSquares = 1;
        private void SetUpWaterVertices()
        {

            
            
            float waterHeight = 0f;
            waterVertices = new VertexPositionNormalTexture[(NumberOfSquares*NumberOfSquares*2*3)];
            float DistanceBetweenX = Width/NumberOfSquares;
            float DistanceBetweenZ = Height/NumberOfSquares;
            int Counter = 0;
            for (int Horizontal = 0; Horizontal < NumberOfSquares; Horizontal++)
            {
                for (int Vertical = 0; Vertical < NumberOfSquares; Vertical++)
                {
                    float StartXPOS = Horizontal * DistanceBetweenX;
                    float StartZPOS = Vertical * DistanceBetweenZ;

                    float XPOS = StartXPOS;
                    float ZPOS = StartZPOS;
                    Vector3 Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS / Width, ZPOS /Height));
                    Counter++;

                    XPOS = StartXPOS;
                    ZPOS = StartZPOS + DistanceBetweenZ;
                    Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS / Width, ZPOS /Height));
                    Counter++;

                    XPOS = StartXPOS + DistanceBetweenX;
                    ZPOS = StartZPOS + DistanceBetweenZ;
                    Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS / Width, ZPOS / Height));
                    Counter++;

                    XPOS = StartXPOS;
                    ZPOS = StartZPOS;
                    Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS / Width, ZPOS / Height));
                    Counter++;

                    XPOS = StartXPOS + DistanceBetweenX;
                    ZPOS = StartZPOS + DistanceBetweenZ;
                    Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS /Width, ZPOS / Height));
                    Counter++;


                    XPOS = StartXPOS + DistanceBetweenX;
                    ZPOS = StartZPOS;
                    Position = new Vector3(XPOS, waterHeight, ZPOS);
                    waterVertices[Counter] = new VertexPositionNormalTexture(Position, Vector3.Normalize(Position), new Vector2(XPOS / Width, ZPOS / Height));
                    Counter++;
                }
            }

                   

                    

                   // waterVertices[2] = new VertexPositionNormalTexture(new Vector3(WIDTH, waterHeight, HEIGHT), Vector3.Normalize(new Vector3(WIDTH, waterHeight, HEIGHT)), new Vector2(1, 0));
                   // waterVertices[1] = new VertexPositionNormalTexture(new Vector3(0, waterHeight, HEIGHT), Vector3.Normalize(new Vector3(0, waterHeight, HEIGHT)), new Vector2(0, 0));

                   // waterVertices[3] = new VertexPositionNormalTexture(new Vector3(0, waterHeight, 0), Vector3.Normalize(new Vector3(0, waterHeight, 0)), new Vector2(0, 1));
                   // waterVertices[5] = new VertexPositionNormalTexture(new Vector3(WIDTH, waterHeight, 0), Vector3.Normalize(new Vector3(WIDTH, waterHeight, 0)), new Vector2(1, 1));
                    //waterVertices[4] = new VertexPositionNormalTexture(new Vector3(WIDTH, waterHeight, HEIGHT), Vector3.Normalize( new Vector3(WIDTH, waterHeight, HEIGHT)), new Vector2(1, 0));
        }

        public void drawWater(GraphicsDevice device, Matrix ViewMatrix, string Technique)
        {
            device.RenderState.CullMode = CullMode.None;
            WaterEffect.CurrentTechnique = WaterEffect.Techniques["TSM2"];
            Matrix localWorld = Matrix.Identity * Matrix.CreateRotationZ(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(500,-5,-500)*Matrix.CreateScale(20f);
            WaterEffect.Parameters["matWorld"].SetValue(localWorld);
            WaterEffect.Parameters["matWorldViewProj"].SetValue(localWorld*ViewMatrix*CameraClass.getPerspective());
            WaterEffect.Parameters["matWorldIT"].SetValue(Matrix.Invert(Matrix.Transpose(localWorld)));
            WaterEffect.Parameters["viewPosition"].SetValue(CameraClass.Position);
            time += animspeed;
            WaterEffect.Parameters["time"].SetValue(time);
            WaterEffect.Begin();
            foreach (EffectPass pass in WaterEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements);
                device.DrawUserPrimitives(PrimitiveType.TriangleList, waterVertices, 0, NumberOfSquares*NumberOfSquares* 2);
                pass.End();
            }
            WaterEffect.End();
            device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }
            

    }
}
