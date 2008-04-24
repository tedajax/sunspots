using System;
using System.Collections.Generic;
using System.Text;

namespace StarForce_PendingTitle_
{
    public class Specs
    {
        public enum Detail
        {
            High,
            Medium,
            Low
        }

        public static Detail ParticleEffects = Detail.Medium;
        public static Detail ModelQuality = Detail.High;
        public static Detail TextureQuality = Detail.High;
        public static Detail Shaders = Detail.High;
    }
}
