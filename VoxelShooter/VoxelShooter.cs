using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using TiledLib;

namespace VoxelShooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VoxelShooter : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        VoxelSprite tilesSprite;
        VoxelWorld gameWorld;

        Camera gameCamera;

        BasicEffect drawEffect;

        ParticleController particleController;
        Starfield gameStarfield;

        Map gameMap;
        TileLayer tileLayer;

        Hero gameHero;

        float scrollDist = 0f;
        float scrollPos = 0f;
        int scrollColumn;

        public VoxelShooter()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tilesSprite = new VoxelSprite(16, 16, 16);
            LoadVoxels.LoadSprite(Path.Combine(Content.RootDirectory, "tiles.vxs"), ref tilesSprite);

            gameWorld = new VoxelWorld(100, 11, 1);

            gameMap = Content.Load<Map>("1");
            tileLayer = (TileLayer)gameMap.GetLayer("tiles");

            for(int yy=0;yy<11;yy++)
                for(int xx=0;xx<12;xx++)
                    if(tileLayer.Tiles[xx,yy]!=null) gameWorld.CopySprite(xx*Chunk.X_SIZE, yy*Chunk.Y_SIZE, 0, tilesSprite.AnimChunks[tileLayer.Tiles[xx,yy].Index-1]);

            scrollColumn = 12;

            gameCamera = new Camera(GraphicsDevice, GraphicsDevice.Viewport);
            gameCamera.Position = new Vector3(0f, gameWorld.Y_SIZE * Voxel.HALF_SIZE, 0f);
            gameCamera.Target = gameCamera.Position;

            gameHero = new Hero();
            gameHero.LoadContent(Content, GraphicsDevice);

            particleController = new ParticleController(GraphicsDevice);
            gameStarfield = new Starfield(GraphicsDevice);

            drawEffect = new BasicEffect(GraphicsDevice)
            {
                World = gameCamera.worldMatrix,
                View = gameCamera.viewMatrix,
                Projection = gameCamera.projectionMatrix,
                VertexColorEnabled = true,
            };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Helper.Random.Next(10) == 1)
            {
                Vector3 pos = new Vector3(100f, -50f+((float)Helper.Random.NextDouble()*100f), 5f + ((float)Helper.Random.NextDouble()*10f));
                Vector3 col = (Vector3.One * 0.5f) + (Vector3.One*((float)Helper.Random.NextDouble()*0.5f));
                gameStarfield.Spawn(pos, new Vector3(-0.1f-((float)Helper.Random.NextDouble()*1f), 0f, 0f), 0.5f, new Color(col), 20000, false);
            }

            if (scrollColumn < gameWorld.X_CHUNKS - 2)
            {
                gameCamera.Target.X += 0.1f;
                scrollDist += 0.1f;
                scrollPos += 0.1f;
                if (scrollDist >= Chunk.X_SIZE * Voxel.SIZE)
                {
                    scrollDist = 0f;
                    for (int yy = 0; yy < 11; yy++)
                        if (tileLayer.Tiles[scrollColumn, yy] != null) gameWorld.CopySprite(scrollColumn * Chunk.X_SIZE, yy * Chunk.Y_SIZE, 0, tilesSprite.AnimChunks[tileLayer.Tiles[scrollColumn, yy].Index - 1]);
                    scrollColumn++;
                }
            }

            gameCamera.Update(gameTime, gameWorld);
            gameWorld.Update(gameTime, gameCamera);

            gameHero.Update(gameTime, gameCamera, scrollPos);

            particleController.Update(gameTime, gameCamera, gameWorld);
            gameStarfield.Update(gameTime, gameCamera, gameWorld);

            drawEffect.View = gameCamera.viewMatrix;
            drawEffect.World = gameCamera.worldMatrix;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            gameStarfield.Draw();

            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int y = 0; y < gameWorld.Y_CHUNKS; y++)
                {
                    for (int x = 0; x < gameWorld.X_CHUNKS; x++)
                    {
                        Chunk c = gameWorld.Chunks[x, y, 0];
                        if (c == null) continue;
                        if (!c.Visible) continue;

                        if (c == null || c.VertexArray==null || c.VertexArray.Length == 0) continue;
                        if (!gameCamera.boundingFrustum.Intersects(c.boundingSphere)) continue;
                        GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, c.VertexArray, 0, c.VertexArray.Length, c.IndexArray, 0, c.VertexArray.Length / 2);
                    }
                }
            }

            gameHero.Draw(GraphicsDevice);

            particleController.Draw();

            base.Draw(gameTime);
        }
    }
}
