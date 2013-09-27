using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Powerup
    {

        public bool Active;
        public Vector3 Position;
        public Vector3 Speed;

        bool affectedByGravity;



        public Powerup()
        {

        }

        public void Update(GameTime gameTime, VoxelWorld gameWorld, Hero gameHero, float scrollPos)
        {

            Position += Speed;
            if(Position.X < scrollPos - 75f) Active = false;

            if (Vector3.Distance(gameHero.Position, Position) < 25f) Position = Vector3.Lerp(Position, gameHero.Position, 0.05f);
            if (Vector3.Distance(gameHero.Position, Position) < 10f) Position = Vector3.Lerp(Position, gameHero.Position, 0.1f);


            if(Vector3.Distance(gameHero.Position, Position) < 3f)
            {
                gameHero.XP += 0.1f;
                Active = false;
            }
        }

        public void Spawn(Vector3 pos, Vector3 speed)
        {
            Active = true;
            Position = pos;
            Speed = speed;
        }


    }
}
