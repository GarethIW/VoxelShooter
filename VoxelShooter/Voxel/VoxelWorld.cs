using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public class VoxelWorld
    {
        public int X_CHUNKS = 25, Y_CHUNKS = 25, Z_CHUNKS = 1;
        public int X_SIZE;
        public int Y_SIZE;
        public int Z_SIZE;

        const double REDRAW_INTERVAL = 0;

        public Chunk[, ,] Chunks;

        double redrawTime = 0;

        public List<VertexPositionNormalColor> Vertices = new List<VertexPositionNormalColor>();
        public List<short> Indexes = new List<short>();

        Queue<Chunk> updateQueue = new Queue<Chunk>();

        public VoxelWorld()
        {
            Init();
        }
        public VoxelWorld(int xs, int ys, int zs)
        {
            X_CHUNKS = xs;
            Y_CHUNKS = ys;
            Z_CHUNKS = zs;

            Init();
        }

        void Init()
        {
            Chunks = new Chunk[X_CHUNKS, Y_CHUNKS, Z_CHUNKS];

           
            X_SIZE = X_CHUNKS * Chunk.X_SIZE;
            Y_SIZE = Y_CHUNKS * Chunk.Y_SIZE;
            Z_SIZE = Z_CHUNKS * Chunk.Z_SIZE;
        }

        public void Update(GameTime gameTime, Camera gameCamera)
        {
            X_SIZE = X_CHUNKS * Chunk.X_SIZE;
            Y_SIZE = Y_CHUNKS * Chunk.Y_SIZE;
            Z_SIZE = Z_CHUNKS * Chunk.Z_SIZE;

            redrawTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (redrawTime > REDRAW_INTERVAL)
            {
                for(int i=0;i<3;i++)
                {
                    if (updateQueue.Count > 0)
                    {
                        redrawTime = 0;
                        Chunk uC = updateQueue.Dequeue();
                        uC.UpdateMesh();
                    }
                }
            }

            for (int y = 0; y < Y_CHUNKS; y++)
            {
                for (int x = 0; x < X_CHUNKS; x++)
                {
                    Chunk c = Chunks[x, y, 0];
                    if (c == null) continue;
                    if (!gameCamera.boundingFrustum.Intersects(c.boundingSphere))//.Transform(Matrix.CreateTranslation(-gameCamera.Position))))
                    {
                        if (c.Visible)
                        {
                            c.Visible = false;
                            //c.ClearMem();
                        }
                    }
                    else
                    {
                        if (!c.Visible)
                        {
                            c.Visible = true;
                            //if (c.Updated) c.UpdateMesh();
                            //c.UpdateMesh();
                        }
                    }
                }
            }
        }

        public void Explode(Vector3 pos, float radius)
        {
            BoundingSphere sphere = new BoundingSphere(pos, radius);
            for (float x = pos.X - radius; x < pos.X + radius; x += Voxel.SIZE)
                for (float y = pos.Y - radius; y < pos.Y + radius; y += Voxel.SIZE)
                    for (float z = pos.Z - radius; z < pos.Z + radius; z += Voxel.SIZE)
                    {
                        Vector3 screen = new Vector3(x, y, z);
                        Vector3 world = FromScreenSpace(screen);

                        if ((int)world.Z >= Z_SIZE - 1) continue;

                        if (sphere.Contains(screen) == ContainmentType.Contains)
                        {
                            Voxel v = GetVoxel(screen);
                            if (v.Active && (v.Destructable > 0 || v.Type == VoxelType.Ground))
                            {
                                SetVoxelActive((int)world.X, (int)world.Y, (int)world.Z, false);
                                if (Helper.Random.Next(10) == 1) ParticleController.Instance.Spawn(screen, new Vector3(-0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -0.05f + ((float)Helper.Random.NextDouble() * 0.1f), -((float)Helper.Random.NextDouble() * 1f)), 0.25f, new Color(v.SR, v.SG, v.SB), 1000, true);
                            }
                        }
                    }
        }

        public Vector3 FromScreenSpace(Vector3 screen)
        {
            Vector3 vox = new Vector3(screen.X / Voxel.SIZE, screen.Y / Voxel.SIZE, screen.Z / Voxel.SIZE);

            return vox;
        }

        public Vector3 ToScreenSpace(int x, int y, int z)
        {
            Vector3 screen = new Vector3(x * Voxel.SIZE, y * Voxel.SIZE, z * Voxel.SIZE);

            return screen;
        }

        public void CopySprite(int x, int y, int z, AnimChunk c)
        {
            for (int xx = 0; xx < c.X_SIZE; xx++)
            {
                for (int yy = 0; yy < c.Y_SIZE; yy++)
                {
                    for (int zz = 0; zz < c.Z_SIZE; zz++)
                    {
                        if (c.Voxels[xx, yy, zz].Active)
                        {
                            SetVoxel(x + xx, y + ((c.Z_SIZE - 1) - zz), z + yy, true, 0, VoxelType.Prefab, c.Voxels[xx, yy, zz].Color, new Color(c.Voxels[xx, yy, zz].Color.ToVector3() * 0.5f));
                        }
                    }
                }
            }

            AddToUpdateQueue(GetChunkAtWorldPosition(x,y,z));
        }

        public void UpdateWorldMeshes()
        {
            //if(!waitingForUpdate)
            //  ThreadPool.QueueUserWorkItem(delegate { DoAsyncUpdate(); });
            foreach (Chunk c in Chunks) c.UpdateMesh();


        }

        public void DoAsyncUpdate()
        {
            //waitingForUpdate = true;
            //waitingForUpdate = false;
        }

        public void SetVoxel(int x, int y, int z, bool active, byte destruct, VoxelType type, Color top, Color side)
        {
            if (x < 0 || y < 0 || z < 0 || x >= X_SIZE || y >= Y_SIZE || z >= Z_SIZE) return;

            Chunk c = GetChunkAtWorldPosition(x, y, z);

            

            c.SetVoxel(x - ((x / Chunk.X_SIZE) * Chunk.X_SIZE), y - ((y / Chunk.Y_SIZE) * Chunk.Y_SIZE), z - ((z / Chunk.Z_SIZE) * Chunk.Z_SIZE), active, destruct, type, top, side);

            c.Updated = true;
        }

        public void SetVoxelActive(int x, int y, int z, bool active)
        {
            if (x < 0 || y < 0 || z < 0 || x >= X_SIZE || y >= Y_SIZE || z >= Z_SIZE) return;

            Chunk c = GetChunkAtWorldPosition(x, y, z);

            c.Voxels[x - ((x / Chunk.X_SIZE) * Chunk.X_SIZE), y - ((y / Chunk.Y_SIZE) * Chunk.Y_SIZE), z - ((z / Chunk.Z_SIZE) * Chunk.Z_SIZE)].Active = active;

            //c.Updated = true;

            //if (x - ((x / Chunk.X_SIZE) * Chunk.X_SIZE) == 0 && x != 0) GetChunkAtWorldPosition(x - 1, y, z).Updated = true;
            //if (x - ((x / Chunk.X_SIZE) * Chunk.X_SIZE) == Chunk.X_SIZE-1 && x < X_SIZE-1) GetChunkAtWorldPosition(x + 1, y, z).Updated = true;
            //if (y - ((y / Chunk.Y_SIZE) * Chunk.Y_SIZE) == 0 && y != 0) GetChunkAtWorldPosition(x, y-1, z).Updated = true;
            //if (y - ((y / Chunk.Y_SIZE) * Chunk.Y_SIZE) == Chunk.Y_SIZE - 1 && y < Y_SIZE - 1) GetChunkAtWorldPosition(x, y + 1, z).Updated = true;

            for (int xx = c.worldX - 1; xx <= c.worldX + 1; xx++)
                for (int yy = c.worldY - 1; yy <= c.worldY + 1; yy++)
                    for (int zz = c.worldZ - 1; zz <= c.worldZ + 1; zz++)
                        if (xx >= 0 && xx < X_CHUNKS && yy >= 0 && yy < Y_CHUNKS && zz >= 0 && zz < Z_CHUNKS) AddToUpdateQueue(Chunks[xx, yy, zz]);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= X_SIZE || y >= Y_SIZE || z >= Z_SIZE) return new Voxel();

            Chunk c = GetChunkAtWorldPosition(x, y, z);

            return c.Voxels[x - ((x / Chunk.X_SIZE) * Chunk.X_SIZE), y - ((y / Chunk.Y_SIZE) * Chunk.Y_SIZE), z - ((z / Chunk.Z_SIZE) * Chunk.Z_SIZE)];
        }
        public Voxel GetVoxel(Vector3 screen)
        {
            screen = FromScreenSpace(screen);
            return GetVoxel((int)screen.X, (int)screen.Y, (int)screen.Z);
        }

        public float GetGroundHeight(Vector3 screen)
        {
            Vector3 voxSpace = FromScreenSpace(screen);
            int x = (int)voxSpace.X;
            int y = (int)voxSpace.Y;
            int z = (int)voxSpace.Z;

            for (int h = z; h < Chunk.Z_SIZE; h++)
            {
                Voxel v = GetVoxel(x, y, h);
                if (v.Active && CanCollideWith(v.Type))
                {
                    z = h;
                    break;
                }
            }

            return z * Voxel.SIZE;
        }

        public Chunk GetChunkAtWorldPosition(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= X_SIZE || y >= Y_SIZE || z >= Z_SIZE) return null;

            if (Chunks[x / Chunk.X_SIZE, y / Chunk.Y_SIZE, z / Chunk.Z_SIZE] == null)
            {
                Chunks[x / Chunk.X_SIZE, y / Chunk.Y_SIZE, z / Chunk.Z_SIZE] = new Chunk(this, x / Chunk.X_SIZE, y / Chunk.Y_SIZE, z / Chunk.Z_SIZE, false);
            }

            return Chunks[x / Chunk.X_SIZE, y / Chunk.Y_SIZE, z / Chunk.Z_SIZE];
        }



        internal bool CanCollideWith(VoxelType voxelType)
        {
            switch (voxelType)
            {
                case VoxelType.Ground:
                case VoxelType.Leaf:
                case VoxelType.Prefab:
                case VoxelType.Tree:
                    return true;
                case VoxelType.Water:
                    return false;
                default:
                    return false;
            }

        }

        void AddToUpdateQueue(Chunk c)
        {
            c.Updated = true;

            foreach (Chunk cc in updateQueue) if (cc == c) return;

            updateQueue.Enqueue(c);
        }
    }
}
