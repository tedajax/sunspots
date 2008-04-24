using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ziggyware.Xna;

namespace StarForce_PendingTitle_
{
    class LoopyEnemy : Enemy
    {
        public LoopyEnemy(Model enemymodel, Vector3 position, Vector3 rotation, short KeyVal)
        {
            EnemyModel = enemymodel;
            Position = position;
            Rotation = rotation;
            this.Key = KeyVal;

            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            PitchUp = true;
            RollRight = true;

            PitchSpeed = MathHelper.Pi / 100;
            RollSpeed = MathHelper.Pi / 25;
        }

        public LoopyEnemy(Enemy enemy)
        {
            EnemyModel = enemy.GetEnemyModel();
            Position = enemy.GetPosition();
            Rotation = enemy.GetRotation();
            this.Key = enemy.getKey();
            Health = enemy.GetHealth();

            TargetPosition = Vector3.Zero;

            EnemyObj = new Obj3d(EnemyModel, Position, Rotation);
            EnemyObj.setScale(10);

            //Setup a temporary collision box
            colboxes[0] = new OBB(Position, new Vector3(50, 10, 10), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            this.Init(colboxes);
            this.setCollisionBoxes(colboxes);

            PitchUp = true;
            RollRight = true;

            PitchSpeed = MathHelper.Pi / 100;
            RollSpeed = MathHelper.Pi / 25;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            base.UpdateLocations(gameTime);
        }
    }
}
