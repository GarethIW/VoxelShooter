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
        public Vector3 ScreenPosition;

        BasicEffect drawEffect;
        VoxelSprite shipSprite;

        public Hero()
        {
            ScreenPosition = new Vector3(0f, 10f, 5f);
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

        public void Update(GameTime gameTime, Camera gameCamera, float scrollPos)
        {
            Position = new Vector3(+scrollPos, 0f, 0f) + ScreenPosition;

            drawEffect.Projection = gameCamera.projectionMatrix;
            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.World = (Matrix.CreateTranslation(Position));
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
    }
}
