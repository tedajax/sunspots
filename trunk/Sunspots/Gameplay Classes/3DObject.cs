using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    /// <summary>
    /// this is a class to handle displaying a simple 3d object.
    /// </summary>
    public class Obj3d : CollisionObject
    {
        Model Object;
        Vector3 Position;
        Vector3 Rotation;
        //Optional setting about lighting, default is on
        bool applylighting;

        Vector3 Scale = new Vector3(1, 1, 1);

        /// <summary>
        /// set up the 3d object
        /// </summary>
        /// <param name="model">the 3d model for this object</param>
        public Obj3d(Model model)
        {
            setUp(model, Vector3.Zero, Vector3.Zero, null);
        }

        /// <summary>
        /// setup the 3d object
        /// </summary>
        /// <param name="model">the 3d model for this object</param>
        /// <param name="position">the position of this object</param>
        /// <param name="rotation">the rotation of this object VECTOR3(X,Y,Z)</param>
        public Obj3d(Model model, Vector3 position, Vector3 rotation)
        {
            setUp(model, position, rotation, null);
        }

        /// <summary>
        /// setup the 3d object
        /// </summary>
        /// <param name="model">the 3d model for this object</param>
        /// <param name="position">the position of this object</param>
        /// <param name="rotation">the rotation of this object VECTOR3(X,Y,Z)</param>
        /// <param name="BoundingBoxes">a list of bounding boxes to use for this object</param>
        public Obj3d(Model model, Vector3 position, Vector3 rotation, OBB[] BoundingBoxes)
        {
            setUp(model, position, rotation, BoundingBoxes);
        }
        /// <summary>
        /// setup the 3d object
        /// </summary>
        /// <param name="model">the 3d model for this object</param>
        /// <param name="position">the position of this object</param>
        /// <param name="rotation">the rotation of this object VECTOR3(X,Y,Z)</param>
        /// <param name="BoundingBoxes">A single bounding box to use for this object</param>
        public Obj3d(Model model, Vector3 position, Vector3 rotation, OBB boundingbox)
        {
            OBB[] boxes = new OBB[1];
            boxes[0] = boundingbox;
            setUp(model, position, rotation, boxes);
        }




        private void setUp(Model model, Vector3 position, Vector3 rotation, OBB[] BoundingBoxes)
        {
            Object = model;
            this.Position = position;
            this.Rotation = rotation;
            this.applylighting = true;
            base.Init(BoundingBoxes);
            CollisionManager.addCollisionObject(this);
        }

        public Model GetModel() { return Object; }

        /// <summary>
        /// displays the model onto the screen (call from draw function)
        /// </summary>
        public void DisplayModel()
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop

            foreach (ModelMesh mesh in Object.Meshes)
            {
                
                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    
                    if (applylighting)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateScale(Scale)
                                   * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                                   * Matrix.CreateTranslation(Position);
                                  

                    effect.View = CameraClass.getLookAt();
                    effect.Projection = CameraClass.getPerspective();

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
        public void DisplayModelWorldMatrix(Matrix WorldMatrix, string technique)
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);

            //Draw the model, a model can have multiple meshes, so loop

            foreach (ModelMesh mesh in Object.Meshes)
            {

                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (Effect effect in mesh.Effects)
                {

                    /*( if (applylighting)
                     {
                         effect.EnableDefaultLighting();
                         effect.PreferPerPixelLighting = true;
                     }*/
                    effect.CurrentTechnique = effect.Techniques[technique];
                    Matrix localWorld = transforms[mesh.ParentBone.Index]
                                   * WorldMatrix;

                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(CameraClass.getLookAt());
                    effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                    /*
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                    effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
        
        public void DisplayModel(Matrix ViewMatrix, string technique, Matrix RotationMatrix)
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);

            //Draw the model, a model can have multiple meshes, so loop

            foreach (ModelMesh mesh in Object.Meshes)
            {

                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (Effect effect in mesh.Effects)
                {

                    /*( if (applylighting)
                     {
                         effect.EnableDefaultLighting();
                         effect.PreferPerPixelLighting = true;
                     }*/
                    effect.CurrentTechnique = effect.Techniques[technique];
                    Matrix localWorld = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateScale(Scale)
                                   * RotationMatrix
                                   * Matrix.CreateTranslation(Position);

                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(ViewMatrix);
                    effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                    /*
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                    effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void DisplayModel(Matrix ViewMatrix, string technique)
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);
            
            //Draw the model, a model can have multiple meshes, so loop

            foreach (ModelMesh mesh in Object.Meshes)
            {

                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (Effect effect in mesh.Effects)
                {

                   /*( if (applylighting)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }*/
                    effect.CurrentTechnique = effect.Techniques[technique];
                    Matrix localWorld = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateScale(Scale)
                                   * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                                   * Matrix.CreateTranslation(Position);

                                   effect.Parameters["World"].SetValue(localWorld);
                                   effect.Parameters["View"].SetValue(ViewMatrix);
                                   effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                    /*
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                    effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void DisplayModel(Matrix ViewMatrix, string technique, Vector3 ExtraRotation)
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);

            //Draw the model, a model can have multiple meshes, so loop

            foreach (ModelMesh mesh in Object.Meshes)
            {

                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (Effect effect in mesh.Effects)
                {

                    /*( if (applylighting)
                     {
                         effect.EnableDefaultLighting();
                         effect.PreferPerPixelLighting = true;
                     }*/
                    effect.CurrentTechnique = effect.Techniques[technique];
                    if (ExtraRotation != Vector3.Zero)
                        Rotation += ExtraRotation;
                    Matrix localWorld = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateScale(Scale)
                                   * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                                   * Matrix.CreateTranslation(Position);

                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(ViewMatrix);
                    effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                    /*
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                    effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

       

        public void DisplayModelDebug(Matrix ViewMatrix, string technique)
        {
            Matrix[] transforms = new Matrix[Object.Bones.Count];
            Object.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in Object.Meshes)
            {
                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (Effect effect in mesh.Effects)
                {
                    /*if (applylighting)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }*/

                    effect.CurrentTechnique = effect.Techniques[technique];


                    Matrix localWorld = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateTranslation(Position)
                                   * Matrix.CreateScale(Scale)
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y));
                    

                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(ViewMatrix);
                    effect.Parameters["Projection"].SetValue(CameraClass.getPerspective());
                    /*
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(ViewMatrix));
                    effect.Parameters["WorldVP"].SetValue(localWorld * ViewMatrix * CameraClass.getPerspective());*/
                    
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }


        public void setPosition(Vector3 position) { this.Position = position; }
        public void setRotation(Vector3 rotation) { this.Rotation = rotation; }
        public void setScale(float newscale) { this.Scale = new Vector3(newscale,newscale,newscale); }
        public void setScale(Vector3 newscale) { this.Scale = newscale; }
        public void setLighting(bool newbool) { this.applylighting = newbool; }
        public Vector3 getPosition() { return this.Position; }
        public Vector3 getRotation() { return this.Rotation; }
        public Vector3 getScale() { return Scale; }

    }
}
