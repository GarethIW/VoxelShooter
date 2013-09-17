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
        Omega
	}

	public class EnemyController
	{
		public static EnemyController Instance;

		public List<Enemy> Enemies = new List<Enemy>();

        List<MapObject> Spawns = new List<MapObject>();

        List<Wave> Waves = new List<Wave>();

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
			VoxelSprite asteroid = new VoxelSprite(16,16,16);
			LoadVoxels.LoadSprite(Path.Combine(content.RootDirectory, "enemies", "asteroids.vxs"), ref asteroid);
			spriteSheets.Add("Asteroid", asteroid);
            VoxelSprite omega = new VoxelSprite(15,15,15);
            LoadVoxels.LoadSprite(Path.Combine(content.RootDirectory, "enemies", "omega.vxs"), ref omega);
            spriteSheets.Add("Omega", omega);

            foreach (MapObject o in spawnLayer.Objects) Spawns.Add(o);
		}

		public Enemy Spawn(EnemyType type, Vector3 pos)
		{
            Enemy e = null;
			switch (type)
			{
				case EnemyType.Asteroid:
                    e = new Asteroid(pos, spriteSheets["Asteroid"]);
				    break;
                case EnemyType.Omega:
                    e = new Omega(pos, spriteSheets["Omega"]);
                    break;
			}

            Enemies.Add(e);
            return e;
		}

		
		public void Update(GameTime gameTime, Camera gameCamera, Hero gameHero, VoxelWorld gameWorld, float scrollPos, float scrollSpeed)
		{
            for(int i=Spawns.Count-1;i>=0;i--)
            {
                if (gameWorld.ToScreenSpace(Spawns[i].Location.Center.X, Spawns[i].Location.Center.Y, 5).X < (int)scrollPos + 75)
                {
                    if (Spawns[i].Properties.Contains("IsWave"))
                    {
                        Wave w = new Wave(gameWorld.ToScreenSpace(Spawns[i].Location.Center.X, Spawns[i].Location.Center.Y, 5), WaveType.Circle, (EnemyType)Enum.Parse(typeof(EnemyType), Spawns[i].Name), Convert.ToInt16(Spawns[i].Properties["Count"]));
                        Waves.Add(w);
                    }
                    else
                    {
                        Spawn((EnemyType)Enum.Parse(typeof(EnemyType), Spawns[i].Name), gameWorld.ToScreenSpace(Spawns[i].Location.Center.X, Spawns[i].Location.Center.Y, 5));
                    }
                    Spawns.RemoveAt(i);
                }
            }

			for(int i=Enemies.Count-1;i>=0;i--) Enemies[i].Update(gameTime, gameWorld, gameHero);

			Enemies.RemoveAll(en => !en.Active || en.Position.X<scrollPos-100f);

            foreach (Wave w in Waves) w.Update(gameTime, scrollSpeed);

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