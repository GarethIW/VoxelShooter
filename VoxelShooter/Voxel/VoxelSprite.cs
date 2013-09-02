using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class VoxelSprite
    {
        public List<AnimChunk> AnimChunks = new List<AnimChunk>();
        public List<RenderTarget2D> ChunkRTs = new List<RenderTarget2D>();

        public int CurrentFrame = 0;

        public int X_SIZE, Y_SIZE, Z_SIZE;

        GraphicsDevice graphicsDevice;

        public VoxelSprite(int xs, int ys, int zs)
        {
            X_SIZE = xs;
            Y_SIZE = ys;
            Z_SIZE = zs;
            //AnimChunks.Add(new AnimChunk(xs, ys, zs, false));
        }
        public VoxelSprite(int xs, int ys, int zs, GraphicsDevice gd)
        {
            graphicsDevice = gd;

            X_SIZE = xs;
            Y_SIZE = ys;
            Z_SIZE = zs;

            AnimChunks.Add(new AnimChunk(xs, ys, zs, true));
            ChunkRTs.Add(new RenderTarget2D(gd,200,150,false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8));
        }

        public void AddFrame(bool guides)
        {
            AnimChunks.Add(new AnimChunk(X_SIZE, Y_SIZE, Z_SIZE, guides));
            //ChunkRTs.Add(new RenderTarget2D(graphicsDevice, 200, 150, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8));
        }

        public void InsertFrame()
        {
            AnimChunks.Insert(CurrentFrame, new AnimChunk(X_SIZE, Y_SIZE, Z_SIZE, true));
            ChunkRTs.Add(new RenderTarget2D(graphicsDevice, 200, 150, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8));
            for (int z = Z_SIZE - 1; z >= 0; z--)
                for (int y = Y_SIZE - 1; y >= 0; y--)
                    for (int x = 0; x < X_SIZE; x++)
                        AnimChunks[CurrentFrame].SetVoxel(x, y, z, AnimChunks[CurrentFrame + 1].Voxels[x, y, z].Active, AnimChunks[CurrentFrame + 1].Voxels[x, y, z].Color);
            AnimChunks[CurrentFrame].UpdateMesh();

        }

        public void DeleteFrame()
        {
            if (AnimChunks.Count > 1)
            {
                ChunkRTs.RemoveAt(CurrentFrame);
                AnimChunks.RemoveAt(CurrentFrame);
            }
            if(CurrentFrame>0) CurrentFrame--;
        }

        public void CopyFrame()
        {
            AnimChunks.Add(new AnimChunk(X_SIZE, Y_SIZE, Z_SIZE, true));
            ChunkRTs.Add(new RenderTarget2D(graphicsDevice, 200, 150, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8));
            for (int z = Z_SIZE - 1; z >= 0; z--)
                for (int y = Y_SIZE - 1; y >= 0; y--)
                    for (int x = 0; x < X_SIZE; x++)
                        AnimChunks[AnimChunks.Count - 1].SetVoxel(x, y, z, AnimChunks[CurrentFrame].Voxels[x, y, z].Active, AnimChunks[CurrentFrame].Voxels[x, y, z].Color);
            AnimChunks[AnimChunks.Count - 1].UpdateMesh();
        }
    }
}
