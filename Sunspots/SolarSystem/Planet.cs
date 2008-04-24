using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

//delete
using Microsoft.Xna.Framework.Input;

namespace StarForce_PendingTitle_
{
    class Planet
    {
        Obj3d PlanetObj;
        float radius; //Radius from the "sun"
        float revolutionrate; //Rate at which the planet revolves around the sun
        float revolutionpos;
        float rotaterate; //Rate at which the planet rotates
        float rotatepos;
        string name;

        //delete this
        float lightpos;

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float RevolutionRate
        {
            get { return revolutionrate; }
            set { revolutionrate = value; }
        }

        public float RotateRate
        {
            get { return rotaterate; }
            set { rotaterate = value; }
        }

        public float RevolutionPosition
        {
            get { return revolutionpos; }
        }

        public string Name
        {
            get { return name; }
        }

        public Vector3 GetPosition() { return PlanetObj.getPosition(); }

        public void UpdateLightData(float radians)
        {
            foreach (ModelMesh mesh in PlanetObj.GetModel().Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Vector3 light = new Vector3((float)Math.Cos(radians), 0, (float)Math.Sin(radians));
                    part.Effect.Parameters["LightDirection"].SetValue(-light);
                }
            }
        }

        public Planet(string Name, Model PlanetModel, float rad, float rev, float rot)
        {
            revolutionpos = MathHelper.ToRadians(Game1.Randomizer.Next(360));
            Vector3 pos = new Vector3((float)Math.Cos(revolutionpos) * rad, 0, (float)Math.Sin(revolutionpos) * rad);
            rotatepos = MathHelper.ToRadians(Game1.Randomizer.Next(360));
            PlanetObj = new Obj3d(PlanetModel, pos, new Vector3(0, rotatepos, 0));
            radius = rad;
            revolutionrate = rev;
            rotaterate = rot;
            name = Name;
        }

        public void Update(GameTime gameTime)
        {
            revolutionpos += revolutionrate;
            rotatepos += rotaterate;
            PlanetObj.setPosition(new Vector3((float)Math.Cos(revolutionpos) * radius, 0, (float)Math.Sin(revolutionpos) * radius));
            PlanetObj.setRotation(new Vector3(0, rotatepos, 0));

            //delete this
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                lightpos -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                lightpos += 0.01f;
            //delete above
        }

        public void Draw(string technique)
        {
            UpdateLightData(revolutionpos);
            PlanetObj.DisplayModel(CameraClass.getLookAt(), technique, PlanetObj.getRotation());
        }

        public void Draw2D(SpriteBatch batch, SpriteFont font)
        {
            batch.Begin();
            batch.DrawString(font, lightpos.ToString(), Vector2.Zero, Color.White);
            batch.DrawString(font, Game1.WrapAngle(revolutionpos).ToString(), new Vector2(0, 25), Color.White);
            batch.End();
        }
    }
}
