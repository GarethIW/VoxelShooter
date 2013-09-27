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
        Laser1,
        Laser2,
        Laser3,
        Laser4,
        Rocket
    }

    public class Projectile
    {
        const float GRAVITY = 0.03f;

        public ProjectileType Type;

        public object Owner;

        public bool Active;
        public Vector3 Position;
        public Vector3 Speed;
        public float Damage;
        public double Life;
        public double Time;
        public Matrix Rotation;
        public Vector3 rotSpeed;
        public bool affectedByGravity;

        public bool Deflected = false;

        float rotX;
        float rotY;

        Enemy target;

        public Projectile()
        {

        }

        public void Update(GameTime gameTime, Hero gameHero, VoxelWorld gameWorld, float scrollPos)
        {
            Time += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (affectedByGravity) Speed.Z += GRAVITY;

            CheckCollisions(gameHero, gameWorld);

            Position += Speed;

            if (Owner is Hero && Position.X > scrollPos + 75f) Active = false;

            Color c;
            switch (Type)
            {
                case ProjectileType.Rocket:
                    
                    foreach(Enemy e in EnemyController.Instance.Enemies.OrderBy(en => Vector3.Distance(en.Position,Position)))
                    {
                        if (e.Position.X < scrollPos - 75f || !e.Active) continue;
                        target = e;
                        break;
                    }
                    
                    if(target!=null)
                    {
                        if (target.Position.X > Position.X) Speed.X += 0.03f;
                        if (target.Position.X < Position.X) Speed.X -= 0.03f;
                        if (target.Position.Y > Position.Y) Speed.Y += 0.03f;
                        if (target.Position.Y < Position.Y) Speed.Y -= 0.03f;
                        Rotation = Matrix.CreateRotationZ(Helper.V2ToAngle(new Vector2(Speed.X, Speed.Y)));
                        Speed = Vector3.Clamp(Speed, new Vector3(-1f, -1f, 0f), new Vector3(1f, 1f, 0f));

                    }

                    if (Helper.Random.Next(5) == 1)
                        ParticleController.Instance.Spawn(Position + new Vector3(Helper.RandomFloat(-0.1f,1f),Helper.RandomFloat(-0.1f,1f),0f) ,
                                                      Vector3.Zero,
                                                      0.3f,
                                                      new Color(new Vector3(1f, Helper.RandomFloat(0f, 1.0f), 0f) * Helper.RandomFloat(0.5f, 1.0f)),
                                                      1000,
                                                      false);

                    break;
               
            }

            if (Time >= Life)
            {
                if (Type == ProjectileType.Rocket)
                {
                    ParticleController.Instance.SpawnExplosion(Position);
                }
                Active = false;
            }

            
        }

        void CheckCollisions(Hero gameHero, VoxelWorld gameWorld)
        {
            Vector3 worldSpace; 
            switch (Type)
            {
                case ProjectileType.Laser1:
                case ProjectileType.Laser2:
                case ProjectileType.Laser3:
                case ProjectileType.Laser4:
                case ProjectileType.Rocket:
                    for (float d = 0f; d < 1f; d += 0.25f)
                    {
                        if (!Active) continue;

                        worldSpace = gameWorld.FromScreenSpace(Position + (d * ((Position + Speed) - Position)));
                        Voxel v = gameWorld.GetVoxel(Position + (d * ((Position + Speed) - Position)));

                        if (v.Active && Active)
                        {
                            if (v.Destructable >= 1 && Owner is Hero)
                            {
                                gameWorld.Explode(Position + (d * ((Position + Speed) - Position)), Type!= ProjectileType.Rocket?3f:5f);
                                gameWorld.Explode((Position + (d * ((Position + Speed) - Position))) + new Vector3(0f, 0f, -3f), Type != ProjectileType.Rocket ? 3f : 5f);
                                gameWorld.Explode((Position + (d * ((Position + Speed) - Position))) + new Vector3(0f, 0f, 3f), Type != ProjectileType.Rocket ? 3f : 5f);

                                //gameWorld.SetVoxelActive((int)worldSpace.X, (int)worldSpace.Y, (int)worldSpace.Z, false);
                                //for (int i = 0; i < 4; i++) ParticleController.Instance.Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.25f, new Color(v.SR, v.SG, v.SB), 1000, true);
                               
                            }
                            Active = false;
                        }
                       
                        if(Owner is Enemy)
                            if (!gameHero.Dead && gameHero.CollisionBox.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains)
                            {
                                gameHero.DoHit(Position + (d * ((Position + Speed) - Position)), this);
                                Active = false;
                                if(Type== ProjectileType.Rocket) ParticleController.Instance.SpawnExplosion(Position);
                            }

                        if(Owner is Hero)
                            foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Active)) { if (e.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains) { e.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, Damage); Active = false; if (Type == ProjectileType.Rocket) ParticleController.Instance.SpawnExplosion(Position); } }

                        
                    }
                    break;
            }
        }

        
    }
}
