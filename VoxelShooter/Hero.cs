using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Hero
    {
        public Vector3 Position;
        public Vector3 Speed;

        Vector3 tempSpeed;

        BasicEffect drawEffect;
        VoxelSprite shipSprite;

        float moveSpeed = 0.5f;

        public Hero()
        {
            Position = new Vector3(-50f, 45f, 5f);
        }

        public void LoadContent(ContentManager content, GraphicsDevice gd)
        {
            shipSprite = new VoxelSprite(15, 15, 15);
            LoadVoxels.LoadSprite(Path.Combine(content.RootDirectory, "ship.vxs"), ref shipSprite);

            drawEffect = new BasicEffect(gd)
            {
                VertexColorEnabled = true
            };
        }

        public void Update(GameTime gameTime, Camera gameCamera, VoxelWorld gameWorld, float scrollSpeed)
        {
            tempSpeed = Speed;
            tempSpeed.X += scrollSpeed;

            CheckCollisions(gameWorld);

            Position += tempSpeed;

            if(Helper.Random.Next(3)==1)
                ParticleController.Instance.Spawn(Position + new Vector3(-4f, 0f, 0f),
                                              new Vector3(Helper.RandomFloat(-0.5f, -0.3f), Helper.RandomFloat(-0.05f, 0.05f), 0f),
                                              0.3f,
                                              new Color(new Vector3(1f, Helper.RandomFloat(0f, 1.0f), 0f) * Helper.RandomFloat(0.5f, 1.0f)),
                                              1000,
                                              false);

            drawEffect.Projection = gameCamera.projectionMatrix;
            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.World = Matrix.CreateTranslation(Position);
        }

        public void Draw(GraphicsDevice gd)
        {
            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                AnimChunk c = shipSprite.AnimChunks[0];
                if (c == null) continue;

                if (c == null || c.VertexArray == null || c.VertexArray.Length == 0) continue;
                gd.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, c.VertexArray, 0, c.VertexArray.Length, c.IndexArray, 0, c.VertexArray.Length / 2); 
            }
        }

        public void Move(Vector2 virtualJoystick)
        {
            //if (Dead) return;
            //if (exitReached) return;

            Vector3 dir = new Vector3(virtualJoystick, 0f);

            Speed = dir * moveSpeed;
        }

        void CheckCollisions(VoxelWorld world)
        {
            float checkRadius = 3.5f;
            float radiusSweep = 0.75f;
            Vector2 v2Pos = new Vector2(Position.X, Position.Y);
            float checkHeight = Position.Z - 1f;
            Voxel checkVoxel;
            Vector3 checkPos;

            if (tempSpeed.Y < 0f)
            {
                for (float a = -MathHelper.PiOver2 - radiusSweep; a < -MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        tempSpeed.Y = 0f;
                    }
                    
                }
            }
            if (tempSpeed.Y > 0f)
            {
                for (float a = MathHelper.PiOver2 - radiusSweep; a < MathHelper.PiOver2 + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        tempSpeed.Y = 0f;
                    }
                    
                }
            }
            if (tempSpeed.X < 0f)
            {
                for (float a = -MathHelper.Pi - radiusSweep; a < -MathHelper.Pi + radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        tempSpeed.X = 0f;
                    }
                    
                }
            }
            if (tempSpeed.X > 0f)
            {
                for (float a = -radiusSweep; a < radiusSweep; a += 0.02f)
                {
                    checkPos = new Vector3(Helper.PointOnCircle(ref v2Pos, checkRadius, a), checkHeight);
                    checkVoxel = world.GetVoxel(checkPos);
                    if ((checkVoxel.Active && world.CanCollideWith(checkVoxel.Type)))
                    {
                        tempSpeed.X = 0f;
                    }
                    
                }
            }
        }
    }
}
