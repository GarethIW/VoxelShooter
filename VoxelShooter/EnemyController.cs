using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiledLib;

namespace VoxelShooter
{
	public enum EnemyType
	{
		Asteroid,
	}

	public class EnemyController
	{
		public static EnemyController Instance;

		public List<Enemy> Enemies = new List<Enemy>();

        List<MapObject> Spawns = new List<MapObject>();

		Dictionary<string, VoxelSprite> spriteSheets = new Dictionary<string,VoxelSprite>();

		GraphicsDevice graphicsDevice;
		BasicEffect drawEffect;

		public EnemyController(GraphicsDevice gd)
		{
			Instance = this;

			graphicsDevice = gd;

			drawEffect = new BasicEffect(gd)
			{
				VertexColorEnabled = true
			};
		}

		public void LoadContent(ContentManager content, MapObjectLayer spawnLayer)
		{
			VoxelSprite manhack = new VoxelSprite(16,16,16);
			LoadVoxels.LoadSprite(Path.Combine(content.RootDirectory, "enemies", "asteroids.vxs"), ref manhack);
			spriteSheets.Add("Asteroid", manhack);

            foreach (MapObject o in spawnLayer.Objects) Spawns.Add(o);
		}

		public void Spawn(EnemyType type, Vector3 pos)
		{
			//return;
			switch (type)
			{
				case EnemyType.Asteroid:
				    Enemies.Add(new Asteroid(pos, spriteSheets["Asteroid"]));
				    break;
				
			}
		}

		
		public void Update(GameTime gameTime, Camera gameCamera, Hero gameHero, VoxelWorld gameWorld, float scrollPos)
		{
            for(int i=Spawns.Count-1;i>=0;i--)
            {
                if (gameWorld.ToScreenSpace(Spawns[i].Location.Center.X, Spawns[i].Location.Center.Y, 5).X < (int)scrollPos + 100)
                {
                    Spawn((EnemyType)Enum.Parse(typeof(EnemyType), Spawns[i].Name), gameWorld.ToScreenSpace(Spawns[i].Location.Center.X, Spawns[i].Location.Center.Y, 5));
                    Spawns.RemoveAt(i);
                }
            }

			for(int i=Enemies.Count-1;i>=0;i--) Enemies[i].Update(gameTime, gameWorld, gameHero);

			Enemies.RemoveAll(en => !en.Active || en.Position.X<scrollPos-100f);

			drawEffect.World = gameCamera.worldMatrix;
			drawEffect.View = gameCamera.viewMatrix;
			drawEffect.Projection = gameCamera.projectionMatrix;
		}

		public void Draw(Camera gameCamera)
		{

			foreach (Enemy e in Enemies)
			{
				drawEffect.DiffuseColor = new Vector3(1f,1f-e.hitAlpha,1f-e.hitAlpha);
				drawEffect.Alpha = 1f;
				drawEffect.World = gameCamera.worldMatrix *
					Matrix.CreateRotationX(e.Rotation.X) *
                        Matrix.CreateRotationY(e.Rotation.Y) *
						Matrix.CreateRotationZ(e.Rotation.Z) *
						Matrix.CreateScale(e.Scale) *
						Matrix.CreateTranslation(e.Position);

                foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();


                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, e.spriteSheet.AnimChunks[e.CurrentFrame + e.offsetFrame].VertexArray, 0, e.spriteSheet.AnimChunks[e.CurrentFrame + e.offsetFrame].VertexArray.Length, e.spriteSheet.AnimChunks[e.CurrentFrame + e.offsetFrame].IndexArray, 0, e.spriteSheet.AnimChunks[e.CurrentFrame + e.offsetFrame].VertexArray.Length / 2);

                }

			}
		}


	}
}