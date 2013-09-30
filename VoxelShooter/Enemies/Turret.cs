using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    class Turret : Enemy
    {
        public float barrelRot;

        public bool Inverted = false;

        public Turret(Vector3 pos, VoxelSprite sprite, bool inverted)
            : base(pos, sprite)
        {
            numFrames = 1;

            Inverted = inverted;

            if (Inverted) Rotation.Z = MathHelper.Pi;

            Speed = Vector3.Zero;
            Health = 3f;

            attackRate = 1000;
        }

        public override void Die()
        {
            base.Die();

            if(Health<=0f)
                for (int i = 0; i < 3; i++)
                    PowerupController.Instance.Spawn(Position + new Vector3(Helper.RandomFloat(-3f, 3f), Helper.RandomFloat(-3f, 3f), 0f));
        }

        public override void DoAttack()
        {
            Vector2 v2Pos = new Vector2(Position.X, Position.Y+(Inverted?-3f:3f));

            ProjectileController.Instance.Spawn(ProjectileType.Laser4, this, new Vector3(Helper.PointOnCircle(ref v2Pos, 3f, barrelRot), Position.Z), Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationZ(barrelRot + MathHelper.PiOver2), new Vector3(Helper.AngleToVector(barrelRot, 1f), 0f), 3f, 2000, false);

            base.DoAttack();
        }

        public override void DoCollide(bool x, bool y, bool z, Vector3 checkPosition, Hero gameHero, VoxelWorld gameWorld, bool withPlayer)
        {
            gameWorld.Explode(checkPosition, 5f);

            Die();


            base.DoCollide(x, y, z, checkPosition, gameHero, gameWorld, withPlayer);
        }

        public override void Update(GameTime gameTime, VoxelWorld gameWorld, Hero gameHero)
        {
            barrelRot = Helper.TurnToFace(new Vector2(Position.X, Position.Y), new Vector2(gameHero.Position.X, gameHero.Position.Y), barrelRot, 1f, 0.1f);

            barrelRot = Inverted ? MathHelper.Clamp(barrelRot, 0.5f, MathHelper.Pi-0.5f) : MathHelper.Clamp(barrelRot, -MathHelper.Pi + 0.5f, -0.5f);
            //f(Helper.Random.Next(10)==1) ParticleController.Instance.Spawn(new Vector3(Helper.RandomPointInCircle(new Vector2(Position.X, Position.Y), 0f, 4f), Position.Z), Vector3.Zero, 0.3f, new Color(Color.Gray.ToVector3() * Helper.RandomFloat(0.4f, 0.8f)), 1000, false);

            base.Update(gameTime, gameWorld, gameHero);
        }
    }
}
