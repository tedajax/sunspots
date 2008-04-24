using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    public class Quad
    {
        private Vector3[] vertices;

        /*
         * index    corner
         * 0        top left
         * 1        bottom right
         * 2        top right
         * 3        bottom left
         */

        public Quad(List<Vector3> veclist)
        {
            veclist.RemoveAt(3);
            veclist.RemoveAt(4);

            vertices = veclist.ToArray();
        }

        public Vector3[] GetVertices() { return vertices; }
    }
}
