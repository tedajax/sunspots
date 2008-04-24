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
#endregion

namespace StarForce_PendingTitle_
{
    public class StupidPlayer
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Rotation;

        public Model PlayerModel;

        public StupidPlayer(Model pmodel)
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Rotation = Vector3.Zero;

            PlayerModel = pmodel;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 VelocityAdd = new Vector3();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                VelocityAdd.X = (float)Math.Sin(MathHelper.ToRadians(Rotation.Y + 180)) / 10f;
                VelocityAdd.Z = (float)Math.Cos(MathHelper.ToRadians(Rotation.Y + 180)) / 10f;
                Velocity = VelocityAdd*8f;
            }

            

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Rotation.Y -= 2f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Rotation.Y += 2f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Velocity *= .95f;
            }
            Position += Velocity;
        }

        public void Draw()
        {
            DisplayModel();
        }

        public void DisplayModel()
        {
            Matrix[] transforms = new Matrix[PlayerModel.Bones.Count];
            PlayerModel.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in PlayerModel.Meshes)
            {
                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y))
                                   * Matrix.CreateTranslation(Position)
                                   * Matrix.CreateScale(1.0f);

                    effect.View = CameraClass.getLookAt();
                    effect.Projection = CameraClass.getPerspective();
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
    }
}
