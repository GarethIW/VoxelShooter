using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public enum WaveType
    {
        Circle,
        Line
    }

    public class Wave
    {
        public Vector3 Position;
        public WaveType Type;
        public EnemyType EnemyType;

        public int EnemyCount;
        public int EnemiesSpawned = 0;

        public List<Enemy> Members = new List<Enemy>();

        float radius;

        float spawnDist = 0f;
        float enemyDist = 0f;

        public Wave(Vector3 pos, WaveType type, EnemyType enemytype, int enemycount)
        {
            Position = pos;
            Type = type;
            EnemyType = enemytype;
            EnemyCount = enemycount;

            radius = 20f;

            Enemy e = EnemyController.Instance.Spawn(EnemyType, Position + new Vector3(0, 0, 200f));
            e.Scale = 0f;
            Members.Add(e);
            EnemiesSpawned++;
        }

        public void Update(GameTime gameTime, float scrollSpeed)
        {
            enemyDist += 0.025f;
            spawnDist += 0.025f;

            Position.X += scrollSpeed/2;

            if (scrollSpeed <= 0f) Position.X -= 0.1f;

            if (enemyDist >= MathHelper.TwoPi) enemyDist = 0f;

            if (spawnDist > MathHelper.TwoPi / ((float)EnemyCount+1) && EnemiesSpawned<EnemyCount)
            {
                Enemy e = EnemyController.Instance.Spawn(EnemyType, Position + new Vector3(0, 0, 200f));
                e.Scale = 0f;
                Members.Add(e);
                spawnDist = 0f;
                EnemiesSpawned++;
            }

            int count = 0;
            foreach(Enemy e in Members)
            {
                if(e!=null)
                {
                    e.Speed = Vector3.Zero;
                    if (e.Position.Z > Position.Z) e.Position.Z -= 1f;
                    if (e.Scale < 1f) e.Scale += 0.01f;
                    Vector2 cp = new Vector2(Position.X,Position.Y);
                    
                    e.Position = new Vector3(Helper.PointOnCircle(ref cp, radius, enemyDist - ((float)count * ((float)MathHelper.TwoPi/(float)EnemyCount))), e.Position.Z);
                }
                count ++;
            }
        }
    }
}
