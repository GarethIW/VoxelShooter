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
    public class ProjectileController
    {
        public static ProjectileController Instance;
        GraphicsDevice graphicsDevice;

        List<Projectile> Projectiles; 

        BasicEffect drawEffect;

        VoxelSprite projectileStrip;

        public ProjectileController(GraphicsDevice gd)
        {
            Instance = this;
            graphicsDevice = gd;
            Projectiles = new List<Projectile>();

            drawEffect = new BasicEffect(gd)
            {
                VertexColorEnabled = true
            };

        }

        public void LoadContent(ContentManager content)
        {
            projectileStrip = new VoxelSprite(5, 5, 5);
            LoadVoxels.LoadSprite(Path.Combine(content.RootDirectory, "projectiles.vxs"), ref projectileStrip);
        }

        public void Update(GameTime gameTime, Camera gameCamera, Hero gameHero)
        {
            foreach (Projectile p in Projectiles.Where(proj => proj.Active))
            {
                p.Update(gameTime, currentRoom, gameHero);
            }

            Projectiles.RemoveAll(proj => !proj.Active);

            drawEffect.World = gameCamera.worldMatrix;
            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.Projection = gameCamera.projectionMatrix;
        }

        public void Draw(Camera gameCamera, Room currentRoom)
        {
            foreach (Projectile p in Projectiles.Where(proj => proj.Type == ProjectileType.Laserbolt && proj.Room == currentRoom))
            {
                drawEffect.Alpha = 0.5f;
                drawEffect.World = gameCamera.worldMatrix *
                                   Matrix.CreateRotationX(MathHelper.PiOver2) *
                                   Matrix.CreateRotationZ(-MathHelper.PiOver2) *
                                   p.Rotation *
                                   Matrix.CreateScale(0.5f) *
                                   Matrix.CreateTranslation(p.Position);
                foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    
                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, projectileStrip.AnimChunks[1].VertexArray, 0, projectileStrip.AnimChunks[1].VertexArray.Length, projectileStrip.AnimChunks[1].IndexArray, 0, projectileStrip.AnimChunks[1].VertexArray.Length / 2);

                }
                drawEffect.Alpha = 1f;
            }
            foreach (Projectile p in Projectiles.Where(proj => proj.Type == ProjectileType.Rocket && proj.Room == currentRoom))
            {
                
                drawEffect.World = gameCamera.worldMatrix *
                                   Matrix.CreateRotationX(MathHelper.PiOver2) *
                                   Matrix.CreateRotationZ(-MathHelper.PiOver2) *
                                   p.Rotation *
                                   Matrix.CreateScale(0.5f) *
                                   Matrix.CreateTranslation(p.Position);
                foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, projectileStrip.AnimChunks[0].VertexArray, 0, projectileStrip.AnimChunks[0].VertexArray.Length, projectileStrip.AnimChunks[0].IndexArray, 0, projectileStrip.AnimChunks[0].VertexArray.Length / 2);

                }

            }
        }

        public void Reset()
        {
            Projectiles.Clear();
        }

        public void Spawn(ProjectileType type, Room room, Vector3 pos, Matrix rot, Vector3 speed, double life, bool gravity)
        {
            Projectile p = null;
            switch(type)
            {
                case ProjectileType.Laserbolt:
                    p = new Projectile()
                    {
                        Type = ProjectileType.Laserbolt,
                        Room = room,
                        Active = true,
                        Position = pos,
                        Speed = speed,
                        Rotation = rot,
                        affectedByGravity = gravity,
                        Life = life,
                        Time = 0
                    };
                    break;
                case ProjectileType.Acid:
                    p = new Projectile()
                    {
                        Type = ProjectileType.Acid,
                        Room = room,
                        Active = true,
                        Position = pos,
                        Speed = speed,
                        Rotation = rot,
                        affectedByGravity = gravity,
                        Life = life,
                        Time = 0
                    };
                    break;
                case ProjectileType.Rocket:
                    p = new Projectile()
                    {
                        Type = ProjectileType.Rocket,
                        Room = room,
                        Active = true,
                        Position = pos,
                        Speed = speed,
                        Rotation = rot,
                        affectedByGravity = gravity,
                        Life = life,
                        Time = 0
                    };
                    break;
                case ProjectileType.Gatling:
                    p = new Projectile()
                    {
                        Type = ProjectileType.Gatling,
                        Room = room,
                        Active = true,
                        Position = pos,
                        Speed = speed,
                        Rotation = rot,
                        affectedByGravity = gravity,
                        Life = life,
                        Time = 0
                    };
                    break;
                //case ProjectileType.Grenade:
                //    p = new Projectile()
                //    {
                //        Active = true,
                //        Type = ProjectileType.Grenade,
                //        Position = pos,
                //        Speed = speed,
                //        Rotation = rot,
                //        affectedByGravity = gravity,
                //        Life= life,
                //        Time=0
                //    };
                //    break;
            }

            Projectiles.Add(p);
        }




        
    }
}
