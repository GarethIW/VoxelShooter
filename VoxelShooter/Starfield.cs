using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Starfield
    {
        const int MAX_PARTICLES = 500;

        public static Starfield Instance;

        GraphicsDevice graphicsDevice;

        VertexPositionNormalColor[] verts = new VertexPositionNormalColor[MAX_PARTICLES * 24];
        short[] indexes = new short[MAX_PARTICLES * 36];

        List<Particle> Particles; 

        int currentParticleCount = 0;

        BasicEffect drawEffect;

        double updateTime = 0;
        double updateTargetTime = 0;

        int parts = 0;

        public Starfield(GraphicsDevice gd)
        {
            Instance = this;

            graphicsDevice = gd;

            Particles = new List<Particle>(MAX_PARTICLES);
            for (int i = 0; i < MAX_PARTICLES; i++) Particles.Add(new Particle());

            drawEffect = new BasicEffect(gd)
            {
                VertexColorEnabled = true
            };


            drawEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, -100), new Vector3(0, 0, 100), Vector3.Down);
            drawEffect.World = Matrix.Identity;
            drawEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, gd.Viewport.AspectRatio, 0.1f, 200f);

        }

        public void Update(GameTime gameTime, Camera gameCamera, VoxelWorld gameWorld)
        {
            
            foreach (Particle p in Particles.Where(part => part.Active))
            {
                p.Update(gameTime, gameWorld);
            }

            updateTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (updateTime >= updateTargetTime)
            {
                updateTime = 0;

                parts = 0;
                foreach (Particle p in Particles.Where(part => part.Active))
                {
                    ParticleCube.Create(ref verts, ref indexes, p.Position, parts, p.Scale / 2, p.Color);
                    parts++;
                }
            }

            currentParticleCount = Particles.Count(part => part.Active);

            //drawEffect.World = Matrix.Identity;
            //drawEffect.View = gameCamera.viewMatrix;
            //drawEffect.Projection = gameCamera.projectionMatrix;
        }

        public void Draw()
        {
            if (currentParticleCount == 0) return;
            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, verts, 0, currentParticleCount * 24, indexes, 0, currentParticleCount * 12);
            }
        }

        public void Spawn(Vector3 pos, Vector3 speed, float scale, Color col, double life, bool gravity)
        {
            Particle p = Particles.FirstOrDefault(part => !part.Active);
            if (p == null) p = Particles.OrderByDescending(part => part.Time).First();
            p.Spawn(pos, speed, scale, col, life, gravity);
        }

        internal void Reset()
        {
            foreach (Particle p in Particles) p.Active = false;
        }
    }
}
