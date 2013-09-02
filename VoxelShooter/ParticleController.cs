using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class ParticleController
    {
        const int MAX_PARTICLES = 500;

        public static ParticleController Instance;

        GraphicsDevice graphicsDevice;

        VertexPositionNormalColor[] verts = new VertexPositionNormalColor[MAX_PARTICLES * 24];
        short[] indexes = new short[MAX_PARTICLES * 36];

        List<Particle> Particles; 

        int currentParticleCount = 0;

        BasicEffect drawEffect;

        double updateTime = 0;
        double updateTargetTime = 0;

        int parts = 0;

        public ParticleController(GraphicsDevice gd)
        {
            Instance = this;

            graphicsDevice = gd;

            Particles = new List<Particle>(MAX_PARTICLES);
            for (int i = 0; i < MAX_PARTICLES; i++) Particles.Add(new Particle());

            drawEffect = new BasicEffect(gd)
            {
                VertexColorEnabled = true
            };

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

            drawEffect.World = gameCamera.worldMatrix;
            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.Projection = gameCamera.projectionMatrix;
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

        public void SpawnExplosion(Vector3 Position)
        {
            for (int i = 0; i < 20; i++)
            {
                Color c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                Spawn(Position, new Vector3(-0.2f + ((float)Helper.Random.NextDouble() * 0.4f), -0.2f + ((float)Helper.Random.NextDouble() * 0.4f), -((float)Helper.Random.NextDouble() * 0.5f)), 0.25f, c, 2000, true);
            }
            for (int i = 0; i < 20; i++)
            {
                Color c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                Spawn(Position, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 1f)), 0.25f, c, 2000, true);
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
