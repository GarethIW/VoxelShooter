using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Enemy
    {
        public EnemyType Type;

        public Vector3 Position;
        public Vector3 Speed;
        public Vector3 Rotation;
        public float Health = 100f;

        public float Scale = 1f;

        public bool Active = true;

        public int CurrentFrame = 0;

        public VoxelSprite spriteSheet;

        public double animTime = 0;
        public double animTargetTime = 500;
        public int numFrames = 2;
        public int offsetFrame = 0;

        public BoundingSphere boundingSphere = new BoundingSphere();

        public float hitAlpha = 0f;

        public double attackRate = 1000;
        public double currentAttackTime = 0;

        public Enemy(Vector3 pos, VoxelSprite sprite)
        {
            Position = pos;
            //Position.Z = 5f;

            spriteSheet = sprite;
        }

        public virtual void Update(GameTime gameTime, VoxelWorld gameWorld, Hero gameHero)
        {
            CheckCollisions(gameWorld, gameHero);

            Position += Speed;
            
            animTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (animTime >= animTargetTime)
            {
                animTime = 0;

                CurrentFrame++;
                if (CurrentFrame == numFrames) CurrentFrame = 0;
            }
           
            boundingSphere = new BoundingSphere(Position, 3f);

            if (hitAlpha > 0f) hitAlpha -= 0.1f;
            if (Health <= 0f) Die();

            currentAttackTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentAttackTime >= attackRate) DoAttack();
        }

        internal void DoExplosionHit(Vector3 pos, float r)
        {
            if (Vector3.Distance(Position, pos) < r)
            {
                float dam = (20f / r) * Vector3.Distance(Position, pos);
                Vector3 speed = (Position - pos);
                speed.Normalize();
                DoHit(Position, speed * 0.5f, dam);
            }
        }

        public virtual void DoHit(Vector3 attackPos, Vector3 speed, float damage)
        {
            hitAlpha = 1f;

            Health -= damage;
        }

        public virtual void DoAttack()
        {
            currentAttackTime = 0;
        }

        public virtual void DoCollide(bool x, bool y, bool z, Vector3 checkPosition, Hero gameHero, VoxelWorld gameWorld, bool withPlayer)
        {
            if (x) Speed.X = 0;
            if (y) Speed.Y = 0;
            if (z) Speed.Z = 0;

            
        }

        public virtual void Die()
        {



            int count = 0;
            for (int x = 0; x < spriteSheet.X_SIZE; x++)
                for (int y = 0; y < spriteSheet.Y_SIZE; y++)
                    for (int z = 0; z < spriteSheet.Z_SIZE; z++)
                    {
                        
                        SpriteVoxel v = spriteSheet.AnimChunks[CurrentFrame].Voxels[x, y, z];
                        if (!v.Active) continue;
                        if (Helper.Random.Next(count<20?5:40) == 1)
                        {
                            Vector3 pos = (-new Vector3(spriteSheet.X_SIZE * Voxel.HALF_SIZE, spriteSheet.Y_SIZE * Voxel.HALF_SIZE, spriteSheet.Z_SIZE * Voxel.HALF_SIZE) * Scale) + (new Vector3(x * Voxel.SIZE, y * Voxel.SIZE, z * Voxel.SIZE) * Scale);
                            pos = Position + Vector3.Transform(pos, Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z));
                            ParticleController.Instance.Spawn(pos, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -1f + ((float)Helper.Random.NextDouble() * 2f)), 0.5f, v.Color, 3000, false);
                            count++;
                        }
                        
                    }

            Active = false;
            
        }

        public virtual void CheckCollisions(VoxelWorld world, Hero gameHero)
        {
            float checkRadius = 3.5f;
            float radiusSweep = 0.75f;
            Vector2 v2Pos = new Vector2(Position.X, Position.Y);
            float checkHeight = Position.Z - 1f;
            Voxel checkVoxel;
            Vector3 checkPos;

            if (gameHero.CollisionBox.Intersects(boundingSphere)) { gameHero.DoHit(Position, null); }

            if (Speed.Y < 0f)
            {
                for (float a = -MathHelper.PiOver2 - radiusSweep; a < -MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        DoCollide(false, true, false, checkPos, gameHero, world, false);
                    }
                    
                }
            }
            if (Speed.Y > 0f)
            {
                for (float a = MathHelper.PiOver2 - radiusSweep; a < MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        DoCollide(false, true, false, checkPos, gameHero, world, false);
                    }
                    //if (gameHero.boundingSphere.Contains(checkPos) == ContainmentType.Contains) { DoCollide(false, true, false, checkPos, currentRoom, gameHero, true); break; }
                }
            }
            if (Speed.X < 0f)
            {
                for (float a = -MathHelper.Pi - radiusSweep; a < -MathHelper.Pi + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        DoCollide(true, false, false, checkPos, gameHero, world, false);
                    }
                    //if (gameHero.boundingSphere.Contains(checkPos) == ContainmentType.Contains) { DoCollide(true, false, false, checkPos, currentRoom, gameHero, true); break; }
                }
            }
            if (Speed.X > 0f)
            {
                for (float a = -radiusSweep; a < radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        DoCollide(true, false, false, checkPos, gameHero, world, false);
                    }
                    //if (gameHero.boundingSphere.Contains(checkPos) == ContainmentType.Contains) { DoCollide(true, false, false, checkPos, currentRoom, gameHero, true); break;}
                }
            }
        }

        
    }
}
