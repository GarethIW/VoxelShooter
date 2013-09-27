using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class PowerupController
    {
        const int MAX_POWERUPS = 100;

        public static PowerupController Instance;

        GraphicsDevice graphicsDevice;

        VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[MAX_POWERUPS * 4];
        short[] indexes = new short[MAX_POWERUPS * 6];

        List<Powerup> Powerups; 

        int currentPowerupCount = 0;

        BasicEffect drawEffect;

        Texture2D texGlow;

        double updateTime = 0;
        double updateTargetTime = 0;

        int parts = 0;

        static short[] faceIndices = new short[] {
                                        0,  1,  2,  // front face
                                        1,  3,  2};

        public PowerupController(GraphicsDevice gd)
        {
            Instance = this;

            graphicsDevice = gd;

            Powerups = new List<Powerup>(MAX_POWERUPS);
            for (int i = 0; i < MAX_POWERUPS; i++) Powerups.Add(new Powerup());

        }

        public void LoadContent(ContentManager content)
        {
            texGlow = content.Load<Texture2D>("glow");

            drawEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
                Texture = texGlow,
                DiffuseColor = Color.Yellow.ToVector3(),
            };
           
        }

        public void Update(GameTime gameTime, Camera gameCamera, VoxelWorld gameWorld, Hero gameHero, float scrollPos)
        {
            
            foreach (Powerup p in Powerups.Where(part => part.Active))
            {
                p.Update(gameTime, gameWorld, gameHero, scrollPos);
            }

            updateTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (updateTime >= updateTargetTime)
            {
                updateTime = 0;

                parts = 0;
                foreach (Powerup p in Powerups.Where(part => part.Active))
                {
                    Vector3 topLeftFront = new Vector3(-1.0f, 1.0f, 0f) * 1f;
                    Vector3 bottomLeftFront = new Vector3(-1.0f, -1.0f, 0f) * 1f;
                    Vector3 topRightFront = new Vector3(1.0f, 1.0f, 0f) * 1f;
                    Vector3 bottomRightFront = new Vector3(1.0f, -1.0f, 0f) * 1f;
                    verts[(parts * 4) + 0] = new VertexPositionNormalTexture(p.Position + topLeftFront, Vector3.Normalize(topLeftFront), new Vector2(0, 0));
                    verts[(parts * 4) + 1] = new VertexPositionNormalTexture(p.Position + bottomLeftFront, Vector3.Normalize(bottomLeftFront), new Vector2(0, 1));
                    verts[(parts * 4) + 2] = new VertexPositionNormalTexture(p.Position + topRightFront, Vector3.Normalize(topRightFront), new Vector2(1, 0));
                    verts[(parts * 4) + 3] = new VertexPositionNormalTexture(p.Position + bottomRightFront, Vector3.Normalize(bottomRightFront), new Vector2(1, 1));

                    for (int i = 0; i < 6; i++) indexes[(parts * 6) + i] = (short)((parts * 4) + faceIndices[i]);

                    parts+=1;
                }
            }

            currentPowerupCount = Powerups.Count(part => part.Active);

            drawEffect.World = gameCamera.worldMatrix;
            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.Projection = gameCamera.projectionMatrix;
        }

        public void Draw()
        {
            if (currentPowerupCount == 0) return;
            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, verts, 0, currentPowerupCount * 4, indexes, 0, currentPowerupCount * 2);
            }
        }

        public void Spawn(Vector3 pos)
        {
            Powerup p = Powerups.FirstOrDefault(part => !part.Active);
            if (p == null) return;
            p.Spawn(pos, new Vector3(Helper.RandomFloat(-0.01f,0.01f), Helper.RandomFloat(-0.01f,0.01f),0f));
        }

        internal void Reset()
        {
            foreach (Powerup p in Powerups) p.Active = false;
        }
    }
}
