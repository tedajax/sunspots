using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarForce_PendingTitle_
{
    class Enemy
    {
        private Vector3 Position;
        private Vector3 Rotation;
        private Vector3 RotationOffset = new Vector3(0, 180, 0);
        private Model EnemyModel;
        private Obj3d EnemyObj;

        //Will be used in some cases such as routine identifier 0
        private Vector3 TargetPosition;

        //Temporary, will be replaced by AI routine class
        //0 will just be circling around a target position
        private int RoutineIdentifier;

        private bool TurnLeft = false;
        private bool TurnRight = true;
        private bool PitchUp = false;
        private bool PitchDown = false;

        //Enemy speed multiplier
        private float EnemySpeed = 6f;

        #region Accessors
        public Vector3 GetPosition() { return Position; }
        public Vector3 GetRotation() { return Rotation; }
        public Model GetEnemyModel() { return EnemyModel; }
        public int GetRoutineIdentifier() { return RoutineIdentifier; }
        public float GetEnemySpeedMultiplier() { return EnemySpeed; }
        public Vector3 GetRotationOffset() { return RotationOffset; }
        #endregion
        #region Mutators
        public void SetPosition(Vector3 pos) { Position = pos; }
        public void SetRotation(Vector3 rot) { Rotation = rot; }
        public void SetEnemyModel(Model mod) { EnemyModel = mod; }
        public void SetRoutineIdentifier(int rid) { RoutineIdentifier = rid; }
        public void SetEnemySpeedMultiplier(float esm) { EnemySpeed = esm; }
        #endregion

        public Enemy(Model enemymodel, Vector3 position, Vector3 rotation)
        {
            EnemyModel = enemymodel;
            Position = position;
            Rotation = rotation;

            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);
        }

        private void AdvanceForward()
        {
            //Create a forward pointing vector
            Vector3 AdvanceVector = new Vector3(0, 0, 1);

            //Transform the vector so it's adjusted to move forward based on rotation values
            AdvanceVector = Vector3.Transform(AdvanceVector, Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));

            //Normalize the vector so all components are between 0 and 1.0
            AdvanceVector = Vector3.Normalize(AdvanceVector);

            //Add the vector times a speed multiplier (for speed up/down) to the position of the enemy
            Position += AdvanceVector * EnemySpeed;
        }

        public void Update(GameTime gameTime)
        {
            if (TurnRight)
            {
                Rotation.Y -= MathHelper.Pi / 450;
            }
            if (TurnLeft)
            {
                Rotation.Y += MathHelper.Pi / 450;
            }
            if (PitchUp)
            {
                Rotation.X -= MathHelper.Pi / 450;
            }
            if (PitchDown)
            {
                Rotation.X += MathHelper.Pi / 450;
            }

            AdvanceForward();

            EnemyObj.setPosition(Position);

            EnemyObj.setRotation(Rotation);
        }

        public void Draw()
        {
            Rotation += RotationOffset;
            EnemyObj.DisplayModel();
            Rotation -= RotationOffset;
        }
    }
}
