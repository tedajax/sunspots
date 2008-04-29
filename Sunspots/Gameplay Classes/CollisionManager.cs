using System;
using System.Collections.Generic;
using System.Text;
using Ziggyware.Xna;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    public class CollisionManager
    {

        private MainShip player;

        private static List<CollisionObject> Environment;

        private static Quad[,] heightmap=null;

        private static float scale=1f;

        private static Vector3 heightmapoffset = new Vector3();

        //private bool ready = false;

        public CollisionManager(MainShip mainship)
        {
            player = mainship;
            Environment = new List<CollisionObject>();
            //ready = true;
        }
        public CollisionManager()
        {
            Environment = new List<CollisionObject>();
            //ready = false;
        }

        public void setPlayer(MainShip player)
        {
            this.player = player;
            //if (Environment != null) this.ready = true;
        }

        public static void addCollisionObject(CollisionObject newObj)
        {
            Environment.Add(newObj);
        }

        public static void addHeightMap(Quad[,] map, float Scale, Vector3 offsetvalue)
        {
            heightmap = map;
            scale = Scale;
            heightmapoffset = offsetvalue;
        }

        public bool CheckVertexWithTerrain2(Vector3 VertexToCheck, OBB O)
        {
            Vector3 V = VertexToCheck;

            Vector3 transform1 = Vector3.Transform(V, O.WorldTransform);
            Vector3 Deconversion = transform1;
            transform1 -= heightmapoffset*20f;
            transform1 = Vector3.Transform(transform1, Matrix.Invert(Matrix.CreateScale(20)));
            
            float realx = (transform1.X);
            float realz = (transform1.Z);
            float realy = transform1.Y;
            //realx += 250;
            //realz += 250;

          //  Deconversion = Vector3.Transform(Deconversion, Matrix.Invert(Matrix.CreateTranslation(250, 0, -250) * Matrix.CreateScale(20f)));

            //  int deconvertedx = (int)Deconversion.X;
            //  int deconvertedz = (int)Deconversion.Z;



            realx = realx / 7.93649f;
            realz = realz / 7.93649f;
            realz *= -1;
            if ((int)realx > 62) realx = 62;
            if ((int)realz > 62) realz = 62;



            if (realx >= 0 && realz >= 0 && (int)realx <= 62 && (int)realz <= 62)
            {
                Vector3[] Verticies = heightmap[(int)realx, (int)realz].GetVertices();
                float XNormalized = (transform1.X % 8);
                XNormalized = (transform1.X % 8) / 8;
                float ZNormalized = ((-1*transform1.Z) % 8) / 8;

                float topHeight = MathHelper.Lerp(Verticies[0].Y, Verticies[2].Y, XNormalized);

                float bottomHeight = MathHelper.Lerp(Verticies[3].Y, Verticies[1].Y, XNormalized);

                float h1 = MathHelper.Lerp(topHeight, bottomHeight, ZNormalized);

                if (realy <= h1)
                {

                    return true;
                }
            }
            return false;
        }

        public bool CheckVertexWithTerrain(Vector3 VertexToCheck, OBB O)
        {
                Vector3 V = VertexToCheck;
                Vector3 transform1 = Vector3.Transform(V, O.WorldTransform);
                Vector3 Deconversion = transform1;
                transform1 = Vector3.Transform(transform1, Matrix.Invert(Matrix.CreateScale(20)));
                int realx = (int)(transform1.X );
                int realz = (int)(transform1.Z);
                float realy = transform1.Y;
                //realx += 250;
                //realz += 250;
               
                Deconversion = Vector3.Transform(Deconversion, Matrix.Invert(Matrix.CreateTranslation(250, 0, -250) * Matrix.CreateScale(20f)));

              //  int deconvertedx = (int)Deconversion.X;
              //  int deconvertedz = (int)Deconversion.Z;

                

                realx = realx / 16;
                realz = realz / 16;
                realz *= -1;
                if (realx > 30) realx = 30;
                if (realz > 30) realz = 30;



                if (realx >= 0 && realz >= 0 && realx < 250 && realz < 250)
                {
                    Vector3[] Verticies = heightmap[realx, realz].GetVertices();
                    //Find the three smallest vectors
                    float[] distances = new float[4];
                    for (int j = 0; j < 4; j++)
                    {
                        distances[j] = Vector3.Distance(new Vector3(Verticies[j].X, 0, Verticies[j].Z), new Vector3(Deconversion.X, 0, Deconversion.Z));
                    }
                    float largestdistance = 0;
                    int selected = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        if (distances[j] > largestdistance)
                        {
                            largestdistance = distances[j];
                            selected = j;
                        }
                    }
                    List<Vector3> SelectedVerts = new List<Vector3>();
                    for (int j = 0; j < 4; j++)
                    {
                        if (j != selected)
                        {
                            SelectedVerts.Add(Verticies[j]);
                        }
                    }

                    //Use 0,2,3 to make a plane
                    Vector3 StartVector = SelectedVerts[0];
                    Vector3 Vector1 = SelectedVerts[1] - StartVector;
                    Vector3 Vector2 = SelectedVerts[2] - StartVector;
                    //Cross the Vectors to get the vector for the plane
                    Vector3 PlaneVector = Vector3.Cross(Vector1, Vector2);
                    //Equation For Plane :
                    //   <Plane Vector> * <x-a[0], y-b[0], z-z[0]> = 0
                    // Solved Equation for plane y= -a(x-a[0])-c(z-c[0])+b*b[0]
                    //                                --------------------------
                    //                                        b
                    float h1 = (-PlaneVector.X * (Deconversion.X - StartVector.X)) - (PlaneVector.Z * (Deconversion.Z - StartVector.Z)) + (PlaneVector.Y * StartVector.Y);
                    h1 /= PlaneVector.Y;



                 

                    if (Deconversion.Y <= h1)
                    {

                       return true;
                    }
                }
            return false;
        }

        public void CheckBoundingBoxWithTerrain(CollisionData CollisionData)
        {
            OBB[] CollisionObjects = CollisionData.getCollisionBoxes();
            for (int c =0; c< CollisionObjects.Length; c++)
            {
                //Find the LocalBoundingBox
                OBB BoundingBox = CollisionObjects[c];
                //find the local bounding box so we can construct the verticies
                BoundingBox B = BoundingBox.LocalBoundingBox;


                Vector3[] Position = new Vector3[8];
                Position[0] = new Vector3(B.Min.X, B.Min.Y, B.Min.Z);
                Position[1] = new Vector3(B.Max.X, B.Min.Y, B.Min.Z);
                Position[2] = new Vector3(B.Max.X, B.Min.Y, B.Max.Z);
                Position[3] = new Vector3(B.Min.X, B.Min.Y, B.Max.Z);

                Position[4] = new Vector3(B.Min.X, B.Max.Y, B.Min.Z);
                Position[5] = new Vector3(B.Max.X, B.Max.Y, B.Min.Z);
                Position[6] = new Vector3(B.Max.X, B.Max.Y, B.Max.Z);
                Position[7] = new Vector3(B.Min.X, B.Max.Y, B.Max.Z);



                for (int i = 0; i < Position.Length; i++)
                {

                    if (CheckVertexWithTerrain2(Position[i],BoundingBox))
                    {
                        //We found some collision. Update 1) Set vertex inside the OBB 2)Tell CollisionData that collision occured
                        BoundingBox.CollisionVerts[i] = true;
                        CollisionData.foundCollision(c);
                    }
                }
            }

           

        }

        public void UpdateCollision()
        {

            CollisionData PlayerObj = player.getCollisionData();
    
            CheckBoundingBoxWithTerrain(PlayerObj);
        }
    }
}
