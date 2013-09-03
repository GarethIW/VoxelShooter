using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class Camera
    {
        public Matrix worldMatrix;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        public BoundingFrustum boundingFrustum;

        public Vector3 Position = new Vector3(0, 0, 0);
        public Vector3 Target = new Vector3(0, 0, 0);

        public float Yaw = MathHelper.Pi;
        public float Roll = MathHelper.Pi;
        public float Pitch = -0.2f;

        public Vector3 Offset = new Vector3(0, 0, 0f);
        //public Vector3 Offset = new Vector3(0, 0f, -30f);

        const float moveSpeed = 0.05f;

        public Camera(GraphicsDevice gd, Viewport vp)
        {
            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            Matrix cameraRotation = Matrix.CreateRotationZ(Roll) * Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0,0,-150), new Vector3(0, 0, 0), Vector3.Down);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, vp.AspectRatio, 0.1f, 200f);

            boundingFrustum = new BoundingFrustum(viewMatrix * projectionMatrix);

          
        }

        public void Update(GameTime gameTime, VoxelWorld gameWorld)
        {
            //Offset = new Vector3(0, 60, -50);// -(new Vector3(0, Position.Z, -Position.Z) * 0.3f);
            //Offset = new Vector3(0, 20, -10);// - (new Vector3(0,Position.Z,-Position.Z) * 0.3f);
            //Position = Vector3.Clamp(Position, gameWorld.ToScreenSpace(140, 115, 0), gameWorld.ToScreenSpace(gameWorld.X_SIZE - 140, gameWorld.Y_SIZE - 90, 20));
            //Position = Vector3.Clamp(Position, new Vector3(70, 60, 0), gameWorld.ToScreenSpace(gameWorld.X_SIZE, gameWorld.Y_SIZE, 0) - new Vector3(100, 60, 50));
            //Target = Vector3.Clamp(Target, gameWorld.ToScreenSpace(140, 115, 0), gameWorld.ToScreenSpace(gameWorld.X_SIZE - 140, gameWorld.Y_SIZE - 90, 20));
            Position = Vector3.Lerp(Position, Target, moveSpeed);
            //worldMatrix = Matrix.CreateWorld(Position, Vector3.Forward, Vector3.Up);



            //viewMatrix = Matrix.CreateLookAt(Position + Offset, Position, Vector3.Down);
            //viewMatrix = Matrix.CreateLookAt(Position + Offset, Position + new Vector3(0, 0, 0), Vector3.Down);
            boundingFrustum.Matrix = viewMatrix * projectionMatrix;
        }

    }
}
