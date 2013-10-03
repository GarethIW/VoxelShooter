using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    class Squid : Enemy
    {
        Vector3 rotSpeed;



        public Squid(Vector3 pos, VoxelSprite sprite)
            : base(pos, sprite)
        {
            numFrames = 4;
            rotSpeed = new Vector3(0.1f, 0f, 0f);//Helper.RandomFloat(-0.1f, 0.1f));
            //Speed.X = Helper.RandomFloat(-0.5f, -0.3f);
            //Speed.Y = Helper.RandomFloat(-0.2f, 0.2f);
            Health = 25f;
            attackRate = 1000 + (double)Helper.Random.Next(2000);
        }

        

        public override void DoAttack()
        {
            base.DoAttack();

            if (Position.Z <=5f)
            {
                ProjectileController.Instance.Spawn(ProjectileType.Laser2, this, Position, Matrix.CreateRotationZ(MathHelper.Pi), new Vector3(-2f, 0f, 0f), 3f, 2000, false);
            }
        }

        public override void Die()
        {
            base.Die();

            if (Health <= 0f)
                for (int i = 0; i < 4 + Helper.Random.Next(4); i++)
                    PowerupController.Instance.Spawn(Position + new Vector3(Helper.RandomFloat(-3f, 3f), Helper.RandomFloat(-3f, 3f), 0f));
        }

        public override void DoCollide(bool x, bool y, bool z, Vector3 checkPosition, Hero gameHero, VoxelWorld gameWorld, bool withPlayer)
        {
            //gameWorld.Explode(checkPosition, 5f);

            //Die();

            base.DoCollide(x, y, z, checkPosition, gameHero, gameWorld, withPlayer);
        }

        public override void Update(GameTime gameTime, VoxelWorld gameWorld, Hero gameHero)
        {
            //Rotation += rotSpeed;

            //if(Helper.Random.Next(10)==1) ParticleController.Instance.Spawn(new Vector3(Helper.RandomPointInCircle(new Vector2(Position.X, Position.Y), 0f, 4f), Position.Z), Vector3.Zero, 0.3f, new Color(Color.Gray.ToVector3() * Helper.RandomFloat(0.4f, 0.8f)), 1000, false);

            boundingSphere = new BoundingSphere(Position, 4f);

            base.Update(gameTime, gameWorld, gameHero);
        }
    }
}
