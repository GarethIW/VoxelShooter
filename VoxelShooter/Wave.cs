using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

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

        float dir = 1f;

        float bob = 0f;

        Vector3 lineEnd;

        PropertyCollection Props;

        public Wave(Vector3 pos, WaveType type, EnemyType enemytype, int enemycount, PropertyCollection props)
        {
            Position = pos;
            Type = type;
            EnemyType = enemytype;
            EnemyCount = enemycount;
            Props = props;

            if (Type == WaveType.Line)
            {
                lineEnd = new Vector3(float.Parse(props["EndX"]), float.Parse(props["EndY"]), 0f);
            }

            radius = 20f;

            Enemy e = null;
            if(Type== WaveType.Circle) e = EnemyController.Instance.Spawn(EnemyType, Position + new Vector3(0, 0, 200f), props);
            if (Type == WaveType.Line) e = EnemyController.Instance.Spawn(EnemyType, (Position - (lineEnd/2f)) + new Vector3(0, 0, 200f), props);
            e.Scale = 0f;
            Members.Add(e);
            EnemiesSpawned++;
        }

        public void Update(GameTime gameTime, float scrollSpeed)
        {
            

            Position.X += scrollSpeed/2;

            //bob = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);

            //if (EnemyType == EnemyType.Squid) Position.Y += (bob * 0.05f);

            if (scrollSpeed <= 0f) Position.X -= 0.1f;

            switch (Type)
            {
                case WaveType.Circle:
                    enemyDist += 0.025f;
                    spawnDist += 0.025f;

                    if (enemyDist >= MathHelper.TwoPi) enemyDist = 0f;

                    if (spawnDist > MathHelper.TwoPi / ((float)EnemyCount + 1) && EnemiesSpawned < EnemyCount)
                    {
                        Enemy e = EnemyController.Instance.Spawn(EnemyType, Position + new Vector3(0, 0, 200f), Props);
                        e.Scale = 0f;
                        Members.Add(e);
                        spawnDist = 0f;
                        EnemiesSpawned++;
                    }
                    break;

                case WaveType.Line:

                    enemyDist += 0.01f;
                    spawnDist += 0.01f;

                    //if (enemyDist > 1f || enemyDist<0f) dir = -dir;

                    if (spawnDist > 1f / ((float)EnemyCount + 1) && EnemiesSpawned < EnemyCount)
                    {
                        Enemy e = EnemyController.Instance.Spawn(EnemyType, (Position - (lineEnd / 2f)) + new Vector3(0, 0, 200f), Props);
                        e.Scale = 0f;
                        Members.Add(e);
                        spawnDist = 0f;
                        EnemiesSpawned++;
                    }

                    break;
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
                 
                    switch(Type)
                    {
                        case WaveType.Circle:
                            e.Position = new Vector3(Helper.PointOnCircle(ref cp, radius, enemyDist - ((float)count * ((float)MathHelper.TwoPi/(float)EnemyCount))), e.Position.Z);
                            break;
                        case WaveType.Line:
                            e.Position = (Position - (lineEnd / 2f)) + ((lineEnd/(float)(EnemyCount-1)) * (float)count);
                            break;
                    }
                }
                count ++;
            }
        }
    }
}
