using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public enum ProjectileType
    {
        Laser,
    }

    public class Projectile
    {
        const float GRAVITY = 0.03f;

        public ProjectileType Type;

        public bool Active;
        public Vector3 Position;
        public Vector3 Speed;
        public double Life;
        public double Time;
        public Matrix Rotation;
        public Vector3 rotSpeed;
        public bool affectedByGravity;

        public bool Deflected = false;

        float rotX;
        float rotY;

        public Projectile()
        {

        }

        public void Update(GameTime gameTime, Hero gameHero, VoxelWorld gameWorld)
        {
            Time += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (affectedByGravity) Speed.Z += GRAVITY;

            CheckCollisions(gameHero, gameWorld);

            Position += Speed;

            Color c;
            switch (Type)
            {
                case ProjectileType.Laser:
                    
                    break;
               
            }

            if (Time >= Life)
            {
                //if (Type == ProjectileType.Grenade || Type == ProjectileType.Rocket)
                //{
                //    ParticleController.Instance.SpawnExplosion(Position);
                //    gameWorld.Explode(Position + new Vector3(0,0,-2f), 5f);
                //}
                Active = false;
            }

            
        }

        void CheckCollisions(Hero gameHero, VoxelWorld gameWorld)
        {
            Vector3 worldSpace; 
            switch (Type)
            {
                case ProjectileType.Laser:
                    for (float d = 0f; d < 1f; d += 0.25f)
                    {
                        worldSpace = gameWorld.FromScreenSpace(Position + (d * ((Position + Speed) - Position)));
                        Voxel v = gameWorld.GetVoxel(Position + (d * ((Position + Speed) - Position)));

                        if (v.Active && Active)
                        {
                            if (v.Destructable >= 1)
                            {
                                gameWorld.Explode(Position + (d * ((Position + Speed) - Position)), 3f);
                                gameWorld.Explode((Position + (d * ((Position + Speed) - Position))) + new Vector3(0f,0f,-3f), 3f);
                                gameWorld.Explode((Position + (d * ((Position + Speed) - Position))) + new Vector3(0f, 0f, 3f), 3f);

                                //gameWorld.SetVoxelActive((int)worldSpace.X, (int)worldSpace.Y, (int)worldSpace.Z, false);
                                //for (int i = 0; i < 4; i++) ParticleController.Instance.Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.25f, new Color(v.SR, v.SG, v.SB), 1000, true);
                               
                            }
                            Active = false;
                        }
                        //if (!gameHero.Dead && gameHero.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains)
                        //{
                        //    if (!gameHero.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 2f))
                        //    {
                        //        Speed = -Speed;
                        //        float rot = Helper.V2ToAngle(new Vector2(Speed.X,Speed.Y));
                        //        if(Type== ProjectileType.Gatling) rot = (rot-0.2f) + ((float)Helper.Random.NextDouble() * 0.4f);
                        //        Speed = new Vector3(Helper.AngleToVector(rot, 1f),0f);
                        //        Deflected = true;
                        //        Rotation = Matrix.CreateRotationZ(rot);
                        //        AudioController.PlaySFX(Type== ProjectileType.Laserbolt?"deflect":"gatling_deflect", Type== ProjectileType.Laserbolt?0.5f:1f, -0.1f, 0.1f);
                        //    }
                        //    else Active = false;
                        //}
                        //if (Deflected) foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Room == Room)) { if (e.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains) { e.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 5f); Active = false; } }

                    }
                    break;
            }
        }

        
    }
}
