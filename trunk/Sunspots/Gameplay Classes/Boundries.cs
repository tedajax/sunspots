using System;
using System.Collections.Generic;
using System.Text;
using Ziggyware.Xna;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    public class Boundries : CollisionObject
    {
        public static float MaxHeight = 800f;

        public Boundries()
        {
            OBB[] OBBs = new OBB[2];
            OBBs[0] = new OBB(new Vector3(5900, 400, -7000)+new Vector3(135*20f,0,2*20f), new Vector3(3000, 3000, 4000));
            OBBs[1] = new OBB(new Vector3(5900, 800, -7000)+new Vector3(135 * 20f, 0, 2 * 20f), new Vector3(3000, 5, 4000));
            base.Init(OBBs);
        }

        public void DrawDebug()
        {
            DebugManager.drawDebugBox(base.CollisionData, Matrix.Identity, CameraClass.getLookAt());
        }


    }
}
