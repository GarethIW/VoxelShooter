using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Particle
    {
        const float GRAVITY = 0.03f;

        public bool Active;
        public Vector3 Position;
        public Vector3 Speed;
        public float Scale;
        public Color Color;
        public float Alpha;
        public double Life;
        public double Time;

        bool affectedByGravity;

        

        public Particle()
        {

        }

        public void Update(GameTime gameTime, VoxelWorld gameWorld)
        {
            Time += gameTime.ElapsedGameTime.TotalMilliseconds;
           
            if (affectedByGravity)
            {
                Speed.Z += GRAVITY;

                if (Speed.Z > 0f)
                {
                    Voxel v = gameWorld.GetVoxel(Position +  new Vector3(0f,0f,Scale));
                    if (v.Active && gameWorld.CanCollideWith(v.Type)) Speed = new Vector3(Speed.X/2,Speed.Y/2,-(Speed.Z / 2f));
                }
                Speed.Z = (float)Math.Round(Speed.Z, 3);
            }

            Position += Speed;

            if (Position.Z > 20f) Active = false;

            if (Time >= Life)
            {
                Scale -= 0.01f;
                if(Scale<=0f) Active = false;
            }
        }

        public void UpdateStarField(GameTime gameTime, VoxelWorld gameWorld, float scrollSpeed)
        {
            Time += gameTime.ElapsedGameTime.TotalMilliseconds;
           
            if (affectedByGravity)
            {
                Speed.Z += GRAVITY;

                if (Speed.Z > 0f)
                {
                    Voxel v = gameWorld.GetVoxel(Position +  new Vector3(0f,0f,Scale));
                    if (v.Active && gameWorld.CanCollideWith(v.Type)) Speed = new Vector3(Speed.X/2,Speed.Y/2,-(Speed.Z / 2f));
                }
                Speed.Z = (float)Math.Round(Speed.Z, 3);
            }

            Position += Speed * scrollSpeed;

            if (Position.Z > 20f) Active = false;

            if (Time >= Life && scrollSpeed>0f)
            {
                Scale -= 0.01f;
                if(Scale<=0f) Active = false;
            }
        }

        public void Spawn(Vector3 pos, Vector3 speed, float scale, Color col, double life, bool gravity)
        {
            Active = true;
            Position = pos;
            Speed = speed;
            Scale = scale;
            Color = col;
            Alpha = 1f;
            affectedByGravity = gravity;
            this.Life = life;
            Time = 0;
        }


    }
}
