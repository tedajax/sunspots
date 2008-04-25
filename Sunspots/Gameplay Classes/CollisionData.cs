using System;
using System.Collections.Generic;
using System.Text;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    public class CollisionData
    {
        private OBB[] CollisionBoxes;
        public bool[] CollisionFound;
        public bool foundoverallcollision = false;

        public CollisionData(OBB[] newCollisionBoxes)
        {
            if (newCollisionBoxes != null)
            {

                CollisionBoxes = newCollisionBoxes;
                CollisionFound = new bool[this.CollisionBoxes.Length];
            }
           
        }

        public CollisionData()
        {
            CollisionBoxes = null;
            CollisionFound = null;
        }

       

        public OBB[] getCollisionBoxes() { return this.CollisionBoxes;}


        public void foundCollision(int where) { this.CollisionFound[where] = true; foundoverallcollision = true; }

        public bool checkCollision(OBB Object)
        {
            bool overall = false;
            for (int i = 0; i < this.CollisionBoxes.Length; i++)
            {
                OBB O = CollisionBoxes[i];
                if (O.Intersects(Object))
                {
                    foundCollision(i);
                    overall = true;
                    foundoverallcollision = true;
                }
            }
            return overall;
        }

        public bool checkCollision(CollisionData Object)
        {
            bool overall = false;
            for (int i = 0; i < this.CollisionBoxes.Length; i++)
            {
                OBB O = CollisionBoxes[i];
                OBB[] OtherBoxes = Object.getCollisionBoxes();
                for (int j = 0; j < OtherBoxes.Length; j++)
                {
                    OBB B = OtherBoxes[j];

                    if (O.Intersects(B))
                    {
                        foundCollision(i);
                        Object.foundCollision(j);
                        overall = true;
                        foundoverallcollision = true;
                    }
                }
            }
            return overall;
        }

        public bool checkCollisionSecret(CollisionData Object)
        {
            bool overall = false;
            for (int i = 0; i < this.CollisionBoxes.Length; i++)
            {
                OBB O = CollisionBoxes[i];
                OBB[] OtherBoxes = Object.getCollisionBoxes();
                for (int j = 0; j < OtherBoxes.Length; j++)
                {
                    OBB B = OtherBoxes[j];

                    if (O.Intersects(B))
                    {
                        foundCollision(i);
                        overall = true;
                        foundoverallcollision = true;
                    }
                }
            }
            return overall;
        }



    }
}
