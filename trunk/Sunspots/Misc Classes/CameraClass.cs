using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    static class CameraClass
    {
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 cameraPointingAt;
        private static Vector3 cameraUpVector;
        
        private static Matrix LookAt;
        private static Matrix Perspective;

        private static float farDistance;

        public static float FarDistance
        {
            get { return farDistance; }
            internal set { farDistance = value; }
        }

        public static Vector3 Position
        {
            get { return position; }
            internal set { position = value; }
        }

        public static Quaternion Rotation
        {
            get { return rotation; }
            internal set { rotation = value; }
        }

        public static Vector3 CameraPointingAt
        {
            get { return cameraPointingAt; }
            internal set { cameraPointingAt = value; }
        }

        public static Vector3 CameraUpVector
        {
            get { return CameraUpVector; }
            internal set { cameraUpVector = value; }
        }

        public static void setUpCameraClass()
        {
            position = new Vector3();
            cameraPointingAt = new Vector3();
            cameraUpVector = new Vector3(0, 1, 0);
            farDistance = 15000f;
        }


        public static void Update()
        {
            LookAt = Matrix.CreateLookAt(position, cameraPointingAt, cameraUpVector);
            float width = (float)Game1.iWidth;
            float height = (float)Game1.iHeight;
            float aspectRatio = width / height;
            Perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Game1.FieldOfView), aspectRatio, 1.0f, farDistance);
        }

        public static Matrix getLookAt() { return LookAt; }
        public static Matrix getPerspective() { return Perspective; }
    }
}
