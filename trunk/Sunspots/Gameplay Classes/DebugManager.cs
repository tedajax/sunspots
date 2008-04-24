using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ziggyware.Xna;
namespace StarForce_PendingTitle_
{
    static class DebugManager
    {

        static short[] indices;

        static GraphicsDevice device;

        static BasicEffect basiceffect;

        static bool setup = false;

        public static void setUp(GraphicsDevice newdevice)
        {
            device = newdevice;
            basiceffect = new BasicEffect(device, null);
            basiceffect.VertexColorEnabled = true;
            

            //basiceffect.LightingEnabled = false;

            setupIndicies();
            setup = true;

        }


        public static void setupIndicies()
        {
            indices = new short[24];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;

            indices[8] = 4;
            indices[9] = 5;
            indices[10] = 5;
            indices[11] = 6;
            indices[12] = 6;
            indices[13] = 7;
            indices[14] = 7;
            indices[15] = 4;

            indices[16] = 0;
            indices[17] = 4;
            indices[18] = 1;
            indices[19] = 5;
            indices[20] = 2;
            indices[21] = 6;
            indices[22] = 3;
            indices[23] = 7;
        }

        public static bool drawDebugBox(CollisionData dataToDraw, Matrix WorldMatrix, Matrix ViewMatrix)
        {
            if (setup == false) return false;

            basiceffect.View = ViewMatrix;
            basiceffect.Projection = CameraClass.getPerspective();
            basiceffect.World = WorldMatrix;

            foreach (OBB collisionbox in dataToDraw.getCollisionBoxes())
            {
                BoundingBox box = collisionbox.LocalBoundingBox;
                VertexPositionColor[] vertices = new VertexPositionColor[8];
                vertices[0].Color = Color.Gray;
                
                vertices[0].Position = new Vector3(box.Min.X, box.Min.Y, box.Min.Z);
                vertices[1].Position = new Vector3(box.Max.X, box.Min.Y, box.Min.Z);
                vertices[2].Position = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
                vertices[3].Position = new Vector3(box.Min.X, box.Min.Y, box.Max.Z);

                vertices[4].Position = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
                vertices[5].Position = new Vector3(box.Max.X, box.Max.Y, box.Min.Z);
                vertices[6].Position = new Vector3(box.Max.X, box.Max.Y, box.Max.Z);
                vertices[7].Position = new Vector3(box.Min.X, box.Max.Y, box.Max.Z);

               for (int i = 0; i < vertices.Length;i++)
                {
                    vertices[i].Color = Color.Green;
                    if (collisionbox.Colors[i] != null)
                    {
                        vertices[i].Color = collisionbox.Colors[i];
                    }
                }

                basiceffect.World = collisionbox.WorldTransform;
                basiceffect.Begin();
                foreach (EffectPass pass in basiceffect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 8, indices, 0, 12);
                    pass.End();

                }
                basiceffect.End();

            }
            return true;

        }



    }
}
