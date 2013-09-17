using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    class Omega : Enemy
    {
        Vector3 rotSpeed;

        public Omega(Vector3 pos, VoxelSprite sprite)
            : base(pos, sprite)
        {
            numFrames = 1;
            rotSpeed = new Vector3(0.1f, 0f, 0f);//Helper.RandomFloat(-0.1f, 0.1f));
            //Speed.X = Helper.RandomFloat(-0.5f, -0.3f);
            //Speed.Y = Helper.RandomFloat(-0.2f, 0.2f);
            Health = 3f;
        }

        public override void DoCollide(bool x, bool y, bool z, Vector3 checkPosition, Hero gameHero, VoxelWorld gameWorld, bool withPlayer)
        {
            gameWorld.Explode(checkPosition, 5f);

            Die();

            base.DoCollide(x, y, z, checkPosition, gameHero, gameWorld, withPlayer);
        }

        public override void Update(GameTime gameTime, VoxelWorld gameWorld, Hero gameHero)
        {
            Rotation += rotSpeed;

            //if(Helper.Random.Next(10)==1) ParticleController.Instance.Spawn(new Vector3(Helper.RandomPointInCircle(new Vector2(Position.X, Position.Y), 0f, 4f), Position.Z), Vector3.Zero, 0.3f, new Color(Color.Gray.ToVector3() * Helper.RandomFloat(0.4f, 0.8f)), 1000, false);

            base.Update(gameTime, gameWorld, gameHero);
        }
    }
}
