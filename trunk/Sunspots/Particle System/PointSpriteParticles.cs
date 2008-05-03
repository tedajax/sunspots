using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    public enum ParticleSystemType
    {
        Ball,
        Explosion,
        Stream,
        Jet
    }
    public class PointSpriteParticles
    {
        protected struct VertexParticle
        {
            public Vector3 Position;
            public Color Color;
            public Vector4 Data;

            public Vector3 Velocity;

            public VertexParticle(Vector3 position, Color color)
            {
                Position = position;
                Color = color;
                Data = Vector4.Zero;
                Velocity = Vector3.Zero;
            }
            public static readonly VertexElement[] VertexElements = new VertexElement[] {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, sizeof(float)*3, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
                new VertexElement(0, sizeof(float)*7, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),                
            };

            public static int SizeInBytes
            {
                get
                {
                    return sizeof(float) * 13;
                }
            }

            public static bool operator !=(VertexParticle left, VertexParticle right)
            {
                return left.GetHashCode() != right.GetHashCode();
            }
            public static bool operator ==(VertexParticle left, VertexParticle right)
            {
                return left.GetHashCode() == right.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return this == (VertexParticle)obj;
            }
            public override int GetHashCode()
            {
                return Position.GetHashCode() | Color.GetHashCode() | Data.GetHashCode() | Velocity.GetHashCode();
            }
            public override string ToString()
            {
                return Position.ToString();
            }
        }
        VertexDeclaration m_vDec;

        VertexParticle[] m_sprites;        
        string textureAsset;
        Texture2D myTexture;

        public int ParticleCount
        {
            get { return m_sprites.Length; }
            set 
            { 
                partCount = value;
                RefreshParticles();
            }
        }

        public Vector3 myPosition;
        public Vector3 myScale;
        public Quaternion myRotation;

        private float BallRadius;
        private float JetScale;

        public Color particleColor;
        private bool randomColor;
        private bool varyColor;

        private bool show = true;

        public bool Show
        {
            get { return show; }
            set { show = value; }
        }

        public bool RandomColor
        {
            get { return randomColor; }
            set
            {
                randomColor = value;
                RefreshParticles();
            }
        }

        public bool VaryColor
        {
            get { return varyColor; }
            set
            {
                varyColor = value;
                RefreshParticles();
            }
        }

        private Random m_rand;
        float myPointSize = 20f;

        string shaderAsset;
        Effect effect;

        int partCount;
        ParticleSystemType system;

        ContentManager content;
        GraphicsDeviceManager myDeviceManager;
        GraphicsDevice myDevice;
        
        public PointSpriteParticles(GraphicsDeviceManager graphiceDeviceManager, string texture, string shader,int particleCount)
        {
            content = new ContentManager(PointSpriteManager.Game.Services);
            myDeviceManager = graphiceDeviceManager;
            
            myPosition = Vector3.Zero;
            myScale = Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);

            Rotate( Vector3.Left, MathHelper.PiOver2);

            partCount = particleCount;

            system = ParticleSystemType.Ball;
            randomColor = true;
            particleColor = Color.YellowGreen;

            textureAsset = texture;
            shaderAsset = shader;

            BallRadius = 2;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(myRotation));
            myRotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * myRotation);

        }
        public void Translate(Vector3 distance)
        {
            myPosition += Vector3.Transform(distance, Matrix.CreateFromQuaternion(myRotation));
        }

        public void Revolve(Vector3 target, Vector3 axis, float angle)
        {
            Rotate(axis, angle);
            Vector3 revolveAxis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(myRotation));
            Quaternion rotate = Quaternion.CreateFromAxisAngle(revolveAxis, angle);
            myPosition = Vector3.Transform(target - myPosition, Matrix.CreateFromQuaternion(rotate));
        }

        public void Initialize()
        {
            this.LoadContent();
        }

        protected void LoadContent()
        {
            myDevice = myDeviceManager.GraphicsDevice;
            myTexture = content.Load<Texture2D>(textureAsset);
            m_rand = new Random();

            m_vDec = new VertexDeclaration(myDevice, VertexParticle.VertexElements);

            effect = content.Load<Effect>(shaderAsset);
            try
            {
                effect.Parameters["particleTexture"].SetValue(myTexture);
            }
            catch { }
            RefreshParticles();
        }

        public void RefreshParticles()
        {            
            m_sprites = new VertexParticle[partCount];
            for (int i = 0; i < m_sprites.Length; i++)
            {
                m_sprites[i] = new VertexParticle();
                m_sprites[i].Position = new Vector3(0, 0, 0);
                m_sprites[i].Data = new Vector4(m_rand.Next(0, 360), m_rand.Next(0, 360), 0, 0);
                if (randomColor)
                    m_sprites[i].Color = new Color(new Vector4((float)m_rand.NextDouble(), (float)m_rand.NextDouble(), (float)m_rand.NextDouble(), 1f));
                else
                {
                    if (varyColor)
                    {
                        //Find highest color value
                        int r = m_sprites[i].Color.R;
                        int g = m_sprites[i].Color.G;
                        int b = m_sprites[i].Color.B;
                        int a = m_sprites[i].Color.A;

                        int lowval = 2;
                        int highval = 4;

                        if (r > g && r > b)
                        {
                            r = r + m_rand.Next(-highval, highval);
                            g = g + m_rand.Next(-lowval, lowval);
                            b = g + m_rand.Next(-lowval, lowval);
                        }
                        else if (g > r && g > b)
                        {
                            r = r + m_rand.Next(-lowval, lowval);
                            g = g + m_rand.Next(-highval, highval);
                            b = g + m_rand.Next(-lowval, lowval);
                        }
                        else if (b > r && b > g)
                        {
                            r = r + m_rand.Next(-lowval, lowval);
                            g = g + m_rand.Next(-lowval, lowval);
                            b = g + m_rand.Next(-highval, highval);
                        }
                        else
                        {
                            r = r + m_rand.Next(-lowval, lowval);
                            g = g + m_rand.Next(-lowval, lowval);
                            b = g + m_rand.Next(-lowval, lowval);
                        }

                        m_sprites[i].Color = new Color((byte)r, (byte)g, (byte)b, (byte)a);
                    }
                    else
                        m_sprites[i].Color = particleColor;
                }
                 
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (system)
            {
                case ParticleSystemType.Ball:
                    Ball();
                    break;
                case ParticleSystemType.Explosion:
                    Explosion();
                    break;
                case ParticleSystemType.Stream:
                    Stream();
                    break;
                case ParticleSystemType.Jet:
                    Jet();
                    break;
            }
        }

        public virtual void Jet()
        {
            for (int i = 0; i < m_sprites.Length; i++)
            {
                float angle = m_sprites[i].Data.X;
                float radius = m_sprites[i].Data.Z;

                float backdistance = m_sprites[i].Data.W;

                angle += 0.9f * JetScale;
                if (angle > 360)
                    angle = angle - 360;

                radius += 0.2f * JetScale;

                float sinangle = (float)Math.Sin((double)MathHelper.ToRadians(angle));
                float cosangle = (float)Math.Cos((double)MathHelper.ToRadians(angle));

                m_sprites[i].Position = new Vector3(sinangle * radius, cosangle * radius, m_sprites[i].Position.Z);
                
                m_sprites[i].Position.Z += 2f * JetScale;

                if (m_sprites[i].Position.Z > backdistance * JetScale)
                {
                    radius = 0;
                    backdistance = m_rand.Next(20, 40) * JetScale;
                    m_sprites[i].Position = Vector3.Zero;
                }

                m_sprites[i].Data = new Vector4(angle, 0, radius, backdistance);
            }
        }

        public virtual void Stream()
        {
            float lerpspeed = 0.1f;
            float backdistance = 600f;

            for (int i = 0; i < m_sprites.Length; i++)
            {
                if (m_sprites[i].Velocity == Vector3.Zero)
                {
                    if (m_rand.NextDouble() < .2)
                    {
                                                
                        /*
                        while (m_sprites[i].Velocity.X > -15 && m_sprites[i].Velocity.X < 15
                               && m_sprites[i].Velocity.Y > -15 && m_sprites[i].Velocity.Y < 15)
                        {
                            m_sprites[i].Velocity.X = m_rand.Next(-30, 30);
                            m_sprites[i].Velocity.Y = m_rand.Next(-30, 30);
                        }
                        */

                        while (m_sprites[i].Velocity.X == 0)
                            m_sprites[i].Velocity.X = m_rand.Next(-1, 2);

                        m_sprites[i].Velocity.X *= 30;

                        m_sprites[i].Velocity.Z = backdistance;

                        m_sprites[i].Data = new Vector4(0.1f, 0, 0, 0);
                    }
                }
                
                if (m_sprites[i].Data.X > 0)
                    m_sprites[i].Data.X += 0.5f;

                m_sprites[i].Position.X = MathHelper.Lerp(m_sprites[i].Position.X, m_sprites[i].Velocity.X, lerpspeed);
                m_sprites[i].Position.Y = MathHelper.Lerp(m_sprites[i].Position.Y, m_sprites[i].Velocity.Y, lerpspeed);
                m_sprites[i].Position.Z += m_sprites[i].Data.X;
                
                
                if (m_sprites[i].Position.Z >= m_sprites[i].Velocity.Z)
                {
                    m_sprites[i].Position = Vector3.Zero;
                    m_sprites[i].Velocity = Vector3.Zero;
                    m_sprites[i].Data = Vector4.Zero;
                }
            }
        }

        public virtual void Ball()
        {
            float radius = BallRadius;
            
            for (int i = 0; i < m_sprites.Length; i++)
            {
                float angle = m_sprites[i].Data.X;
                float angle2 = m_sprites[i].Data.Y;

                angle += .01f;
                if (angle > 360)
                    angle = 0;
                angle2 += .01f;
                if (angle2 > 360)
                    angle2 = 0;

                float cos = radius * (float)Math.Cos(angle);
                float sin = radius * (float)Math.Sin(angle);
                float cos2 = radius * (float)Math.Cos(angle2);
                float sin2 = (float)Math.Pow(radius, 2) * (float)Math.Sin(angle2);

                m_sprites[i].Position = new Vector3(cos * cos2, sin * cos2, sin2);
                m_sprites[i].Color = new Color(new Vector4(m_sprites[i].Color.ToVector3(), 1f));

                m_sprites[i].Data = new Vector4(angle, angle2, 0, 0);
            }
        }
        public virtual void Explosion()
        {
            for (int i = 0; i < m_sprites.Length; i++)
            {
                float angle = m_sprites[i].Data.X;
                float angle2 = m_sprites[i].Data.Y;
                float radius = m_sprites[i].Data.Z;

                angle += .01f;
                if (angle > 360)
                    angle = 0;
                angle2 += .01f;
                if (angle2 > 360)
                    angle2 = 0;

                radius += .1f;

                float cos = radius * (float)Math.Cos(angle);
                float sin = radius * (float)Math.Sin(angle);
                float cos2 = radius * (float)Math.Cos(angle2);
                float sin2 = (float)Math.Pow(radius, 2) * (float)Math.Sin(angle2);

                m_sprites[i].Position = new Vector3(cos * cos2, sin * cos2, sin2);
                m_sprites[i].Color = new Color(new Vector4(m_sprites[i].Color.ToVector3(),1f - (radius/3f)));

                if (radius > 5)
                    radius = 0;

                m_sprites[i].Data = new Vector4(angle, angle2, radius, 0);                
            }
        }
        public void Draw(GameTime gameTime)
        {
            bool PointSpriteEnable = myDevice.RenderState.PointSpriteEnable;
            float PointSize = myDevice.RenderState.PointSize;
            bool AlphaBlendEnable = myDevice.RenderState.AlphaBlendEnable;
            BlendFunction AlphaBlendOperation = myDevice.RenderState.AlphaBlendOperation;
            Blend SourceBlend = myDevice.RenderState.SourceBlend;
            Blend DestinationBlend = myDevice.RenderState.DestinationBlend;
            bool SeparateAlphaBlendEnabled = myDevice.RenderState.SeparateAlphaBlendEnabled;
            bool AlphaTestEnable = myDevice.RenderState.AlphaTestEnable;
            CompareFunction AlphaFunction = myDevice.RenderState.AlphaFunction;
            int ReferenceAlpha = myDevice.RenderState.ReferenceAlpha;
            bool DepthBufferWriteEnable = myDevice.RenderState.DepthBufferWriteEnable;

            if (myDevice.RenderState.PointSpriteEnable != true)
                myDevice.RenderState.PointSpriteEnable = true;
            
            float TargetPointSize = myPointSize;
            //if (this.system == ParticleSystemType.Jet) TargetPointSize *= JetScale;

            if (myDevice.RenderState.PointSize != TargetPointSize)
            {
                myDevice.RenderState.PointSize = TargetPointSize;
            }
            
          
                
            
            if (myDevice.RenderState.AlphaBlendEnable != true)
                myDevice.RenderState.AlphaBlendEnable = true;
            if (myDevice.RenderState.AlphaBlendOperation != BlendFunction.Add)
                myDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            if (myDevice.RenderState.SourceBlend != Blend.SourceAlpha)
                myDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            if (myDevice.RenderState.DestinationBlend != Blend.One)
                myDevice.RenderState.DestinationBlend = Blend.One;
            if (myDevice.RenderState.SeparateAlphaBlendEnabled != false)
                myDevice.RenderState.SeparateAlphaBlendEnabled = false;
            if (myDevice.RenderState.AlphaTestEnable != true)
                myDevice.RenderState.AlphaTestEnable = true;
            if (myDevice.RenderState.AlphaFunction != CompareFunction.Greater)
                myDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            if (myDevice.RenderState.ReferenceAlpha != 0)
                myDevice.RenderState.ReferenceAlpha = 0;
            if (myDevice.RenderState.DepthBufferWriteEnable != false)
              myDevice.RenderState.DepthBufferWriteEnable = false;

            myDevice.VertexDeclaration = m_vDec;

            Matrix wvp = Matrix.CreateScale(5) * (Matrix.CreateScale(myScale) * Matrix.CreateFromQuaternion(myRotation) * Matrix.CreateTranslation(myPosition)) * CameraClass.getLookAt() * CameraClass.getPerspective();
            effect.Parameters["WorldViewProj"].SetValue(wvp);

            if (show)
            {
                effect.Begin();
                for (int ps = 0; ps < effect.CurrentTechnique.Passes.Count; ps++)
                {
                    effect.CurrentTechnique.Passes[ps].Begin();

                    if (m_sprites.Length >= 15000)
                    {
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 0, m_sprites.Length / 3);
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, m_sprites.Length / 3, m_sprites.Length / 3);
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 2 * m_sprites.Length / 3, m_sprites.Length / 3);
                    }
                    else
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 0, m_sprites.Length);

                    effect.CurrentTechnique.Passes[ps].End();
                }
                effect.End();
            }

            if (myDevice.RenderState.PointSpriteEnable != PointSpriteEnable)
                myDevice.RenderState.PointSpriteEnable = PointSpriteEnable;
            if (myDevice.RenderState.PointSize != PointSize)
                myDevice.RenderState.PointSize = PointSize;
            if (myDevice.RenderState.AlphaBlendEnable != AlphaBlendEnable)
                myDevice.RenderState.AlphaBlendEnable = AlphaBlendEnable;
            if (myDevice.RenderState.AlphaBlendOperation != AlphaBlendOperation)
                myDevice.RenderState.AlphaBlendOperation = AlphaBlendOperation;
            if (myDevice.RenderState.SourceBlend != SourceBlend)
                myDevice.RenderState.SourceBlend = SourceBlend;
            if (myDevice.RenderState.DestinationBlend != DestinationBlend)
                myDevice.RenderState.DestinationBlend = DestinationBlend;
            if (myDevice.RenderState.SeparateAlphaBlendEnabled != SeparateAlphaBlendEnabled)
                myDevice.RenderState.SeparateAlphaBlendEnabled = SeparateAlphaBlendEnabled;
            if (myDevice.RenderState.AlphaTestEnable != AlphaTestEnable)
                myDevice.RenderState.AlphaTestEnable = AlphaTestEnable;
            if (myDevice.RenderState.AlphaFunction != AlphaFunction)
                myDevice.RenderState.AlphaFunction = AlphaFunction;
            if (myDevice.RenderState.ReferenceAlpha != ReferenceAlpha)
                myDevice.RenderState.ReferenceAlpha = ReferenceAlpha;
            if (myDevice.RenderState.DepthBufferWriteEnable != DepthBufferWriteEnable)
                myDevice.RenderState.DepthBufferWriteEnable = DepthBufferWriteEnable;

        }

        public void Draw()
        {
            bool PointSpriteEnable = myDevice.RenderState.PointSpriteEnable;
            float PointSize = myDevice.RenderState.PointSize;
            bool AlphaBlendEnable = myDevice.RenderState.AlphaBlendEnable;
            BlendFunction AlphaBlendOperation = myDevice.RenderState.AlphaBlendOperation;
            Blend SourceBlend = myDevice.RenderState.SourceBlend;
            Blend DestinationBlend = myDevice.RenderState.DestinationBlend;
            bool SeparateAlphaBlendEnabled = myDevice.RenderState.SeparateAlphaBlendEnabled;
            bool AlphaTestEnable = myDevice.RenderState.AlphaTestEnable;
            CompareFunction AlphaFunction = myDevice.RenderState.AlphaFunction;
            int ReferenceAlpha = myDevice.RenderState.ReferenceAlpha;
            bool DepthBufferWriteEnable = myDevice.RenderState.DepthBufferWriteEnable;

            if (myDevice.RenderState.PointSpriteEnable != true)
                myDevice.RenderState.PointSpriteEnable = true;

            float TargetPointSize = myPointSize;
            //if (this.system == ParticleSystemType.Jet) TargetPointSize *= JetScale;

            if (myDevice.RenderState.PointSize != TargetPointSize)
            {
                myDevice.RenderState.PointSize = TargetPointSize;
            }




            if (myDevice.RenderState.AlphaBlendEnable != true)
                myDevice.RenderState.AlphaBlendEnable = true;
            if (myDevice.RenderState.AlphaBlendOperation != BlendFunction.Add)
                myDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            if (myDevice.RenderState.SourceBlend != Blend.SourceAlpha)
                myDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            if (myDevice.RenderState.DestinationBlend != Blend.One)
                myDevice.RenderState.DestinationBlend = Blend.One;
            if (myDevice.RenderState.SeparateAlphaBlendEnabled != false)
                myDevice.RenderState.SeparateAlphaBlendEnabled = false;
            if (myDevice.RenderState.AlphaTestEnable != true)
                myDevice.RenderState.AlphaTestEnable = true;
            if (myDevice.RenderState.AlphaFunction != CompareFunction.Greater)
                myDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            if (myDevice.RenderState.ReferenceAlpha != 0)
                myDevice.RenderState.ReferenceAlpha = 0;
            if (myDevice.RenderState.DepthBufferWriteEnable != false)
                myDevice.RenderState.DepthBufferWriteEnable = false;

            myDevice.VertexDeclaration = m_vDec;

            Matrix wvp = Matrix.CreateScale(5) * (Matrix.CreateScale(myScale) * Matrix.CreateFromQuaternion(myRotation) * Matrix.CreateTranslation(myPosition)) * CameraClass.getLookAt() * CameraClass.getPerspective();
            effect.Parameters["WorldViewProj"].SetValue(wvp);

            if (show)
            {
                effect.Begin();
                for (int ps = 0; ps < effect.CurrentTechnique.Passes.Count; ps++)
                {
                    effect.CurrentTechnique.Passes[ps].Begin();

                    if (m_sprites.Length >= 15000)
                    {
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 0, m_sprites.Length / 3);
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, m_sprites.Length / 3, m_sprites.Length / 3);
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 2 * m_sprites.Length / 3, m_sprites.Length / 3);
                    }
                    else
                        myDevice.DrawUserPrimitives<VertexParticle>(PrimitiveType.PointList, m_sprites, 0, m_sprites.Length);

                    effect.CurrentTechnique.Passes[ps].End();
                }
                effect.End();
            }

            if (myDevice.RenderState.PointSpriteEnable != PointSpriteEnable)
                myDevice.RenderState.PointSpriteEnable = PointSpriteEnable;
            if (myDevice.RenderState.PointSize != PointSize)
                myDevice.RenderState.PointSize = PointSize;
            if (myDevice.RenderState.AlphaBlendEnable != AlphaBlendEnable)
                myDevice.RenderState.AlphaBlendEnable = AlphaBlendEnable;
            if (myDevice.RenderState.AlphaBlendOperation != AlphaBlendOperation)
                myDevice.RenderState.AlphaBlendOperation = AlphaBlendOperation;
            if (myDevice.RenderState.SourceBlend != SourceBlend)
                myDevice.RenderState.SourceBlend = SourceBlend;
            if (myDevice.RenderState.DestinationBlend != DestinationBlend)
                myDevice.RenderState.DestinationBlend = DestinationBlend;
            if (myDevice.RenderState.SeparateAlphaBlendEnabled != SeparateAlphaBlendEnabled)
                myDevice.RenderState.SeparateAlphaBlendEnabled = SeparateAlphaBlendEnabled;
            if (myDevice.RenderState.AlphaTestEnable != AlphaTestEnable)
                myDevice.RenderState.AlphaTestEnable = AlphaTestEnable;
            if (myDevice.RenderState.AlphaFunction != AlphaFunction)
                myDevice.RenderState.AlphaFunction = AlphaFunction;
            if (myDevice.RenderState.ReferenceAlpha != ReferenceAlpha)
                myDevice.RenderState.ReferenceAlpha = ReferenceAlpha;
            if (myDevice.RenderState.DepthBufferWriteEnable != DepthBufferWriteEnable)
                myDevice.RenderState.DepthBufferWriteEnable = DepthBufferWriteEnable;

        }

        public void SetSystemToBall(float rad) { system = ParticleSystemType.Ball; BallRadius = rad; }
        public void SetSystemToExplode() { system = ParticleSystemType.Explosion; }
        public void SetSystemToJet() { system = ParticleSystemType.Jet; JetScale = 1; }
        public void SetSystemToJet(float scl) { system = ParticleSystemType.Jet; JetScale = scl; }
        public void SetSystemToStream() { system = ParticleSystemType.Stream; }
    }

}
