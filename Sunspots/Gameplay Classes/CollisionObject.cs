using System;
using System.Collections.Generic;
using System.Text;
using Ziggyware.Xna;
namespace StarForce_PendingTitle_
{
    public abstract class CollisionObject
    {
        protected CollisionData CollisionData;

        public void setCollisionBoxes(OBB[] Boxes) { this.CollisionData = new CollisionData(Boxes); }

        public CollisionData getCollisionData() { return this.CollisionData; }

        public void Init(OBB[] Boxes) 
        {
            if (Boxes != null)
            {
                this.CollisionData = new CollisionData(Boxes);
            }
            else
            {
                Boxes = new OBB[0];
                this.CollisionData = new CollisionData(Boxes);
            }
        }
    }
}
