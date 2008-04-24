using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace StarForce_PendingTitle_
{
    public class ParticleSystem
    {
        [Serializable]
        public struct SpriteVertex
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Velocity;
            public Vector3 Acceleration;
            public bool Draw;
            public SpriteVertex(Vector3 position, Color color)
            {
                Position = position;
                Color = color;
                Velocity = Vector3.Zero;
                Acceleration = Vector3.Zero;
                Draw = true;
            }
            public static readonly VertexElement[] VertexElements =
               new VertexElement[] {
                   
                    new VertexElement(0, 0, VertexElementFormat.Vector3,VertexElementMethod.Default, VertexElementUsage.Position, 0),
                    new VertexElement(0, sizeof(float)*3, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
                    new VertexElement(0, sizeof(float)*7, VertexElementFormat.Vector3,VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(0, sizeof(float)*10, VertexElementFormat.Vector3,VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)

                };
            public static int SizeInBytes
            {
                get
                {
                    return (3 + 4 + 3 + 3) * sizeof(float);
                }
            }
            public static bool operator !=(SpriteVertex left, SpriteVertex right)
            {
                return left.GetHashCode() != right.GetHashCode();
            }
            public static bool operator ==(SpriteVertex left, SpriteVertex right)
            {
                return left.GetHashCode() == right.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                 return this == (SpriteVertex)obj;
            }
            public override int GetHashCode()
            {
                return Position.GetHashCode() | Color.GetHashCode() | Velocity.GetHashCode() | Acceleration.GetHashCode();
            }
            public override string ToString()
            {
                return string.Format("{0},{1},{2}", Position.X, Position.Y, Position.Z);
            }
        }

        static Effect m_effect;
        public const int particlesize = 80;
        public static int size = 80;

        Texture2D Sprite1;
        Texture2D Flames;
        Texture2D Reticule;
        Texture2D TargetReticule;
        Texture2D LockedReticule;
        Texture2D ReticuleUseTexture;
        Texture2D SunTexture;
        static String SpriteToUse;
        
        //SpriteVertex[] m_sprites;
        static List<SpriteVertex> Sprites = new List<SpriteVertex>();
        VertexDeclaration m_vDec;
        static Random m_rand;
        //Viewport viewport;

        public ParticleSystem()
        {
            //viewport = new Viewport();
        }

        public static bool Targeted = false;

        public void LoadGraphicsContent2(GraphicsDevice graphics, ContentManager content)
        {
            m_vDec = new VertexDeclaration(graphics, SpriteVertex.VertexElements);
            m_effect = content.Load<Effect>("Content\\Effects\\PointSprite");
            SunTexture = content.Load<Texture2D>("Content\\sun");
            createParticle(new Vector3(), new Vector3());
        }

        public void LoadGraphicsContent(GraphicsDevice graphics, ContentManager content)
        {
            m_vDec = new VertexDeclaration(graphics, SpriteVertex.VertexElements);
            m_effect = content.Load<Effect>("Content\\Effects\\PointSprite");
            //m_effect.Parameters["SpriteTexture"].SetValue(content.Load<Texture2D>("Content\\omg"));
            Sprite1 = content.Load<Texture2D>("Content\\start");
            //Flames = content.Load<Texture2D>("Content\\clearflames");
            Reticule = content.Load<Texture2D>("Content\\reticule");
            TargetReticule = content.Load<Texture2D>("Content\\reticuletarg");
            LockedReticule = content.Load<Texture2D>("Content\\reticulelocked");
           
            
            //m_sprites = new SpriteVertex[1200];
            
            m_rand = new Random();

            createParticle(new Vector3(), new Vector3());
            createParticle(new Vector3(), new Vector3());
            createParticle(new Vector3(), new Vector3());           

        }

        public static void createParticle(Vector3 Position, Vector3 Velocity)
        {
            SpriteVertex newsprite = new SpriteVertex();
            newsprite.Position = Position;
            newsprite.Velocity = Velocity;
            newsprite.Acceleration = new Vector3();
            newsprite.Color = Color.Blue;


            Sprites.Add(newsprite);

        }

      

        public static void addThrusterSprite(Matrix PositionMatrix, Quaternion Rotation)
        {
            if (m_rand.NextDouble() > .2)
            {
                SpriteVertex newsprite = new SpriteVertex();
                newsprite.Position = new Vector3(m_rand.Next(-3000, 3000) / 1000f, m_rand.Next(-2100, 0) / 1000f, -4.8f);
                newsprite.Position = Vector3.Transform(newsprite.Position, PositionMatrix);
                newsprite.Velocity = new Vector3();
                newsprite.Velocity = Vector3.Transform(newsprite.Velocity, Matrix.CreateFromQuaternion(Rotation));
                newsprite.Acceleration = new Vector3();
                newsprite.Color = Color.Blue;

                Sprites.Add(newsprite);
            }

        }

        public static void addThrusterSprite2(Matrix PositionMatrix, Quaternion Rotation)
        {
            for (int i=-3000; i<5000; i+=3000)
            {
                SpriteVertex newsprite = new SpriteVertex();
                newsprite.Position = new Vector3(i / 1000f, m_rand.Next(-2100, 0) / 1000f, -8.8f);
                newsprite.Position = Vector3.Transform(newsprite.Position, PositionMatrix);
                newsprite.Velocity = new Vector3();
                newsprite.Velocity = Vector3.Transform(newsprite.Velocity, Matrix.CreateFromQuaternion(Rotation));
                newsprite.Acceleration = new Vector3();
                newsprite.Color = Color.Blue;

                Sprites.Add(newsprite);
            }

        }

        public static void addThrusterSprite3(MainShip ship, MyControls controls)
        {
            while (Sprites.Count < 2)
            {
                createParticle(new Vector3(), new Vector3());
            }

            SpriteVertex newsprite = Sprites[0];
                newsprite.Position = new Vector3((float)200 / 1000f,(1500/2) / 1000f, 8.8f);
                newsprite.Position = Vector3.Transform(newsprite.Position, ship.getNonMovementPositionRotationMatrix() );
                //newsprite.Velocity = new Vector3();
                //newsprite.Velocity = Vector3.Transform(newsprite.Velocity, Matrix.CreateFromQuaternion(ship.NewRotation)*ship.getDrawYawPitchRoll());
                newsprite.Acceleration = new Vector3();
                newsprite.Color = Color.Blue;
                size = particlesize;
                if (ship.isBoosting()) size += 1 * particlesize/3;
                Sprites[0] = newsprite;
                //Sprites.Add(newsprite);



                 newsprite = Sprites[1];
                newsprite.Position = new Vector3 (0,0,-350);
                newsprite.Position = Vector3.Transform(newsprite.Position, ship.getNonMovementPositionRotationMatrix());
                //newsprite.Velocity = new Vector3();
                //newsprite.Velocity = Vector3.Transform(newsprite.Velocity, Matrix.CreateFromQuaternion(ship.NewRotation)*ship.getDrawYawPitchRoll());
                newsprite.Acceleration = new Vector3();
                newsprite.Color = Color.Blue;
                Sprites[1] = newsprite;

                if (controls.getMissle() == 1)
                {
                    if (Targeted)
                    {
                        SpriteToUse = "Targetted";
                    }
                    else
                    {
                        SpriteToUse = "Targetting";
                    }
                }
                else
                {
                    SpriteToUse = "Normal";
                }
                   

              
                
        }

        public void Update(GameTime gametime)
        {
            this.ReticuleUseTexture = Reticule;
            if (SpriteToUse == "Targetted")
            {
                this.ReticuleUseTexture = LockedReticule;
            }
            if (SpriteToUse == "Targetting")
            {
                this.ReticuleUseTexture = this.TargetReticule;
            }
           for (int i=0;i<Sprites.Count;i++)
            {
                SpriteVertex m_sprites = Sprites[i];
                m_sprites.Position += m_sprites.Velocity;
                   bool isless = (m_sprites.Color.A <= 5);
                 m_sprites.Color = new Color(m_sprites.Color.R,m_sprites.Color.G,m_sprites.Color.B,(byte)(m_sprites.Color.A-5));
                 if (isless && m_sprites.Color.A > 5)
                 {
                     m_sprites.Color = new Color(m_sprites.Color.R, m_sprites.Color.G, m_sprites.Color.B, 0);
                     Sprites.Remove(m_sprites);
                 }
                 else
                 {
                     Sprites[i] = m_sprites;
                 }
               
            }
        }

        public void Draw2(GameTime gameTime, GraphicsDevice graphics)
        {

            graphics.RenderState.PointSpriteEnable = true;
            graphics.RenderState.PointSize = (float)size;
            graphics.RenderState.AlphaBlendEnable = true;
            graphics.RenderState.AlphaBlendOperation = BlendFunction.Add;
            graphics.RenderState.SourceBlend = Blend.SourceAlpha;
           // graphics.RenderState.DestinationBlend = Blend.One;
            graphics.RenderState.SeparateAlphaBlendEnabled = false;
            graphics.RenderState.AlphaTestEnable = true;
            graphics.RenderState.AlphaFunction = CompareFunction.Greater;
            graphics.RenderState.ReferenceAlpha = 0;
            graphics.RenderState.DepthBufferWriteEnable = true;
            graphics.VertexDeclaration = m_vDec;
            graphics.RenderState.DepthBufferEnable = true;
            m_effect.Parameters["WorldViewProj"].SetValue(Matrix.Identity * CameraClass.getLookAt() * CameraClass.getPerspective());
            m_effect.Parameters["SpriteTexture"].SetValue(SunTexture);
            m_effect.Parameters["Rotation"].SetValue(Matrix.Identity);
            m_effect.Begin();
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                if (Sprites.Count > 0)
                {
                    SpriteVertex[] m_sprites = Sprites.ToArray();
                    graphics.DrawUserPrimitives<SpriteVertex>(PrimitiveType.PointList, m_sprites, 0, 1);
                }
                pass.End();
            }
            m_effect.End();
            graphics.RenderState.AlphaBlendEnable = false;
            graphics.RenderState.SeparateAlphaBlendEnabled = false;
            graphics.RenderState.AlphaTestEnable = false;
            graphics.RenderState.DepthBufferEnable = true;
            graphics.RenderState.PointSpriteEnable = false;
            graphics.RenderState.DepthBufferWriteEnable = true;
            graphics.RenderState.SourceBlend = Blend.SourceAlpha;
            graphics.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphics, Matrix ViewMatrix)
        {
            graphics.RenderState.PointSpriteEnable = true;
            graphics.RenderState.PointSize = (float)size;
            graphics.RenderState.AlphaBlendEnable = true;
            graphics.RenderState.AlphaBlendOperation = BlendFunction.Add;
            graphics.RenderState.SourceBlend = Blend.SourceAlpha;
            //graphics.RenderState.DestinationBlend = Blend.One;
            graphics.RenderState.SeparateAlphaBlendEnabled = false;
            graphics.RenderState.AlphaTestEnable = true;
            graphics.RenderState.AlphaFunction = CompareFunction.Greater;
            graphics.RenderState.ReferenceAlpha = 0;
            graphics.RenderState.DepthBufferWriteEnable = true;
            graphics.VertexDeclaration = m_vDec;
            graphics.RenderState.DepthBufferEnable = true;
            m_effect.Parameters["WorldViewProj"].SetValue(Matrix.Identity* ViewMatrix * CameraClass.getPerspective());
            m_effect.Parameters["SpriteTexture"].SetValue(Sprite1);
            m_effect.Parameters["Rotation"].SetValue(Matrix.Identity);
            m_effect.Begin();
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                if (Sprites.Count > 0)
                {
                    SpriteVertex[] m_sprites = Sprites.ToArray();
                    graphics.DrawUserPrimitives<SpriteVertex>(PrimitiveType.PointList, m_sprites, 0, 1);
                }
                pass.End();
            }
            m_effect.End();
            graphics.RenderState.PointSize = (float)particlesize;
            m_effect.Parameters["WorldViewProj"].SetValue(Matrix.Identity* ViewMatrix * CameraClass.getPerspective());
            m_effect.Parameters["SpriteTexture"].SetValue(ReticuleUseTexture);
            m_effect.Begin();
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                if (Sprites.Count > 1)
                {
                    SpriteVertex[] m_sprites = Sprites.ToArray();
                    graphics.DrawUserPrimitives<SpriteVertex>(PrimitiveType.PointList, m_sprites, 1, 1);
                }
                pass.End();
            }
            m_effect.End();

          


            graphics.RenderState.AlphaBlendEnable = false;
            graphics.RenderState.SeparateAlphaBlendEnabled = false;
            graphics.RenderState.AlphaTestEnable = false;
            graphics.RenderState.DepthBufferEnable = true ;
            graphics.RenderState.PointSpriteEnable = false;
            graphics.RenderState.DepthBufferWriteEnable = true;
           graphics.RenderState.SourceBlend = Blend.SourceAlpha;
            graphics.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

        }
    }
}
