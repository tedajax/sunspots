using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    public class PointSpriteManager
    {
        List<PointSpriteParticles> ParticleList;

        protected static string particleImage = "Content\\Particle";
        protected static string particleEffect = "Content\\Effects\\Particle";

        public static string ParticleImage
        {
            get { return particleImage; }
        }

        public static string ParticleEffect
        {
            get { return particleEffect; }
        }

        public PointSpriteManager()
        {
            ParticleList = new List<PointSpriteParticles>();
        }

        public void AddParticle(PointSpriteParticles psparts)
        {
            ParticleList.Add(psparts);
        }

        public PointSpriteParticles RemoveParticle(int index)
        {
            if (IndexValid(index))
            {
                PointSpriteParticles returnparts = ParticleList[index];
                ParticleList.RemoveAt(index);
                return returnparts;
            }

            return null;
        }

        public void ClearList() { ParticleList.Clear(); }

        public void Update(GameTime gameTime)
        {
            foreach (PointSpriteParticles ps in ParticleList)
            {
                ps.Update(gameTime);
            }
        }

        public List<PointSpriteParticles> GetParticleList() { return ParticleList; }

        public void Draw(GameTime gameTime)
        {
            foreach (PointSpriteParticles ps in ParticleList)
            {
                ps.Draw(gameTime);
            }
        }

        //Helper function to determine if an index is within the range of the list
        private bool IndexValid(int i)
        {
            if (i >= 0 && i < ParticleList.Count)
                return true;
            else
                return false;
        }
    }
}
