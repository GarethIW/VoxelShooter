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
        Laserbolt,
        Rocket,
        Gatling,
        Acid
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

        public void Update(GameTime gameTime, Room currentRoom, Hero gameHero)
        {
            Time += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (affectedByGravity) Speed.Z += GRAVITY;

            CheckCollisions(currentRoom, gameHero);

            Position += Speed;

            Color c;
            switch (Type)
            {
                case ProjectileType.Laserbolt:
                    
                    break;
                case ProjectileType.Rocket:
                    if (gameHero.Position.X < Position.X) Speed.X -= 0.01f;
                    if (gameHero.Position.X > Position.X) Speed.X += 0.01f;
                    if (gameHero.Position.Y < Position.Y) Speed.Y -= 0.01f;
                    if (gameHero.Position.Y > Position.Y) Speed.Y += 0.01f;

                    Rotation = Matrix.CreateRotationZ(Helper.V2ToAngle(new Vector2(Speed.X, Speed.Y)));

                    if(Helper.Random.Next(2)==0)
                        c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                    else
                        c = new Color(Vector3.One * (0.5f+((float)Helper.Random.NextDouble()*0.3f)));

                    ParticleController.Instance.Spawn(Position, -(Speed*0.3f) + new Vector3(-0.01f + ((float)Helper.Random.NextDouble() * 0.02f), -0.01f + ((float)Helper.Random.NextDouble() * 0.02f), -0.01f + ((float)Helper.Random.NextDouble() * 0.02f)), 0.4f, c, 100, false);

                    break;
                case ProjectileType.Gatling:
                    c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                    ParticleController.Instance.Spawn(Position, Vector3.Zero, 0.4f, c, 20, false);
                    break;

                case ProjectileType.Acid:
                    c = new Color(new Vector3(0f, 1f, 0.0f)) * (0.5f + ((float)Helper.Random.NextDouble() * 0.5f));
                    if(Helper.Random.Next(2)==1) ParticleController.Instance.Spawn(Position, Vector3.Zero, 0.5f, c, 0, false);
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

        void CheckCollisions(Room currentRoom, Hero gameHero)
        {
            Vector3 worldSpace; 
            switch (Type)
            {
                case ProjectileType.Laserbolt:
                case ProjectileType.Gatling:
                    for (float d = 0f; d < 1f; d += 0.25f)
                    {
                        worldSpace = VoxelWorld.FromScreenSpace(Position + (d * ((Position + Speed) - Position)));
                        Voxel v = Room.World.GetVoxel(Position + (d*((Position+Speed)-Position)));

                        if (v.Active && Active)
                        {
                            if (v.Destructable == 1)
                            {
                                Room.World.SetVoxelActive((int)worldSpace.X, (int)worldSpace.Y, (int)worldSpace.Z, false);
                                if(Room == currentRoom) for (int i = 0; i < 4; i++) ParticleController.Instance.Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.25f, new Color(v.SR, v.SG, v.SB), 1000, true);
                               
                            }
                            Active = false;
                        }
                        if (!gameHero.Dead && gameHero.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains)
                        {
                            if (!gameHero.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 2f))
                            {
                                Speed = -Speed;
                                float rot = Helper.V2ToAngle(new Vector2(Speed.X,Speed.Y));
                                if(Type== ProjectileType.Gatling) rot = (rot-0.2f) + ((float)Helper.Random.NextDouble() * 0.4f);
                                Speed = new Vector3(Helper.AngleToVector(rot, 1f),0f);
                                Deflected = true;
                                Rotation = Matrix.CreateRotationZ(rot);
                                AudioController.PlaySFX(Type== ProjectileType.Laserbolt?"deflect":"gatling_deflect", Type== ProjectileType.Laserbolt?0.5f:1f, -0.1f, 0.1f);
                            }
                            else Active = false;
                        }
                        if (Deflected) foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Room == Room)) { if (e.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains) { e.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 5f); Active = false; } }

                    }
                    break;
                case ProjectileType.Rocket:
                    for (float d = 0f; d < 1f; d += 0.25f)
                    {
                        worldSpace = VoxelWorld.FromScreenSpace(Position + (d * ((Position + Speed) - Position)));
                        Voxel v = Room.World.GetVoxel(Position + (d * ((Position + Speed) - Position)));

                        if (v.Active && Active)
                        {

                            Room.World.Explode(Position + (d * ((Position + Speed) - Position)), 5f, Room == currentRoom);
                            if (Room == currentRoom) ParticleController.Instance.SpawnExplosion(Position);
                            gameHero.DoExplosionHit(Position + (d * ((Position + Speed) - Position)), 5f);
                            if (Deflected)
                            {
                                foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Room == Room))
                                {
                                    e.DoExplosionHit(Position + (d * ((Position + Speed) - Position)), 5f);
                                    Room.World.Explode(Position + (d * ((Position + Speed) - Position)), 5f, Room == currentRoom);
                                    if (Room == currentRoom) ParticleController.Instance.SpawnExplosion(Position);
                                }
                            }
                           
                            Active = false;
                            AudioController.PlaySFX("explosion2",1f, -0.1f, 0.1f);

                        }
                        if (!gameHero.Dead && gameHero.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains)
                        {
                            if (!gameHero.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 0f))
                            {
                                Speed = -Speed;
                                Deflected = true;
                                Rotation = Matrix.CreateRotationZ(Helper.V2ToAngle(new Vector2(Speed.X, Speed.Y)));
                                AudioController.PlaySFX("deflect", 0.5f, -0.1f, 0.1f);

                            }
                            else
                            {
                                gameHero.DoExplosionHit(Position + (d * ((Position + Speed) - Position)), 5f);
                                Room.World.Explode(Position + (d * ((Position + Speed) - Position)), 5f, Room == currentRoom);
                                if (Room == currentRoom) ParticleController.Instance.SpawnExplosion(Position);
                                Active = false;
                                AudioController.PlaySFX("explosion2", 1f, -0.1f, 0.1f);

                            }
                        }
                        if (Deflected) 
                            foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Room == Room)) 
                            { 
                                if (e.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains) 
                                { 
                                    e.DoExplosionHit(Position + (d * ((Position + Speed) - Position)), 5f);
                                    Room.World.Explode(Position + (d * ((Position + Speed) - Position)), 5f, Room == currentRoom);
                                    if (Room == currentRoom) ParticleController.Instance.SpawnExplosion(Position);
                                    Active = false;
                                    AudioController.PlaySFX("explosion2", 1f, -0.1f, 0.1f);

                                } 
                            }

                    }
                    break;
                case ProjectileType.Acid:
                    for (float d = 0f; d < 1f; d += 0.25f)
                    {
                        worldSpace = VoxelWorld.FromScreenSpace(Position + (d * ((Position + Speed) - Position)));
                        Voxel v = Room.World.GetVoxel(Position + (d * ((Position + Speed) - Position)));

                        if (v.Active && Active)
                        {
                            Room.World.Explode(Position + (d * ((Position + Speed) - Position)), 2f, Room == currentRoom);
                            for (int i = 0; i < 4; i++)
                            {
                                ParticleController.Instance.Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.5f, new Color(0f, 0.5f + ((float)Helper.Random.NextDouble() * 0.5f), 0f), 1000, true);
                            }
                           
                            Active = false;
                            AudioController.PlaySFX("acid_hit", 1f, -0.1f, 0.1f);

                        }
                        if (!gameHero.Dead && gameHero.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains)
                        {
                            if (!gameHero.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 5f))
                            {
                                Speed = -Speed;
                                float rot = Helper.V2ToAngle(new Vector2(Speed.X, Speed.Y));
                                if (Type == ProjectileType.Gatling) rot = (rot - 0.2f) + ((float)Helper.Random.NextDouble() * 0.4f);
                                Speed = new Vector3(Helper.AngleToVector(rot, 0.2f), Speed.Z);
                                Deflected = true;
                                Rotation = Matrix.CreateRotationZ(rot);
                            }
                            else
                            {
                                Active = false;
                                AudioController.PlaySFX("acid_hit", 1f, -0.1f, 0.1f);

                                for (int i = 0; i < 4; i++)
                                {
                                    ParticleController.Instance.Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.5f, new Color(0f, 0.5f + ((float)Helper.Random.NextDouble() * 0.5f), 0f), 1000, true);
                                }
                            }
                        }
                        //if (Deflected) 
                        //    foreach (Enemy e in EnemyController.Instance.Enemies.Where(en => en.Room == Room)) 
                        //    { if (e.boundingSphere.Contains(Position + (d * ((Position + Speed) - Position))) == ContainmentType.Contains) { e.DoHit(Position + (d * ((Position + Speed) - Position)), Speed, 5f); Active = false; } }

                    }
                    break;
                //case ProjectileType.Grenade:
                //    float checkRadius = 1f;
                //    float radiusSweep = 0.5f;
                //    Vector2 v2Pos = new Vector2(Position.X,Position.Y);
                //    Voxel checkVoxel;
                //    Vector3 checkPos;
                //    if (Speed.Z > 0f)
                //    {
                //        for (float z = 0f; z < 2f; z+=1f)
                //        {
                //            Voxel v = gameWorld.GetVoxel(Position + new Vector3(0f, 0f, z));
                //            if (v.Active && gameWorld.CanCollideWith(v.Type)) Speed = new Vector3(Speed.X * 0.6f, Speed.Y * 0.6f, -(Speed.Z / 2f));
                //        }
                //    }
                //    if (Speed.Y < 0f)
                //    {
                //        for (float r = checkRadius; r > 0f; r -= 1f)
                //        {
                //            for (float a = -MathHelper.PiOver2 - radiusSweep; a < -MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                //            {
                //                checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, r, a), Position.Z);
                //                checkVoxel = gameWorld.GetVoxel(checkPos);
                //                if ((checkVoxel.Active && gameWorld.CanCollideWith(checkVoxel.Type)))
                //                {
                //                    Speed.Y = 0f;
                //                }
                //            }
                //        }
                //    }
                //    if (Speed.Y > 0f)
                //    {
                //        for (float r = checkRadius; r > 0f; r -= 1f)
                //        {
                //            for (float a = MathHelper.PiOver2 - radiusSweep; a < MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                //            {
                //                checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, r, a), Position.Z);
                //                checkVoxel = gameWorld.GetVoxel(checkPos);
                //                if ((checkVoxel.Active && gameWorld.CanCollideWith(checkVoxel.Type)))
                //                {
                //                    Speed.Y = 0f;
                //                }
                //            }
                //        }
                //    }
                //    if (Speed.X < 0f)
                //    {
                //        for (float r = checkRadius; r > 0f; r -= 1f)
                //        {
                //            for (float a = -MathHelper.Pi - radiusSweep; a < -MathHelper.Pi + radiusSweep; a += 0.02f)
                //            {
                //                checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, r, a), Position.Z);
                //                checkVoxel = gameWorld.GetVoxel(checkPos);
                //                if ((checkVoxel.Active && gameWorld.CanCollideWith(checkVoxel.Type)))
                //                {
                //                    Speed.X = 0f;
                //                }
                //            }
                //        }
                //    }
                //    if (Speed.X > 0f)
                //    {
                //        for (float r = checkRadius; r > 0f; r -= 1f)
                //        {
                //            for (float a = -radiusSweep; a < radiusSweep; a += 0.02f)
                //            {
                //                checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, r, a), Position.Z);
                //                checkVoxel = gameWorld.GetVoxel(checkPos);
                //                if ((checkVoxel.Active && gameWorld.CanCollideWith(checkVoxel.Type)))
                //                {
                //                    Speed.X = 0f;
                //                }
                //            }
                //        }
                //    }
                //    break;
            }
        }

        
    }
}
