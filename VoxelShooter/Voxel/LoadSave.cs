using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VoxelShooter
{
    public static class LoadVoxels
    {
        
        public static void LoadSprite(string fn, ref VoxelSprite sprite)
        {
       
            byte[] buffer;

            using (FileStream gstr = new FileStream(fn, FileMode.Open))
            {
                byte[] lb = new byte[4];
                gstr.Position = gstr.Length - 4;
                gstr.Read(lb, 0, 4);
                int msgLength = BitConverter.ToInt32(lb, 0);

                buffer = new byte[msgLength];

                gstr.Position = 0;

                using (GZipStream str = new GZipStream(gstr, CompressionMode.Decompress))
                {

                    str.Read(buffer, 0, msgLength);
                }
            }

            int pos = 0;

            int xs = buffer[0];
            int ys = buffer[1];
            int zs = buffer[2];
            int frames = buffer[3];
            sprite = new VoxelSprite(xs, ys, zs);
            sprite.AnimChunks.Clear();
            sprite.ChunkRTs.Clear();

            pos = 4;

            for (int i = 0; i < 10; i++)
            {
                // don't need swatches in game!
                pos += 3;
            }


            for (int frame = 0; frame < frames; frame++)
            {
                sprite.AddFrame(false);

                AnimChunk c = sprite.AnimChunks[frame];

                while (pos < buffer.Length)
                {
                    if (Convert.ToChar(buffer[pos]) != 'c')
                    {
                        //str.Seek(-1, SeekOrigin.Current);
                        //str.Read(ba, 0, 10);
                        int vx = buffer[pos];
                        int vy = buffer[pos + 1];
                        int vz = buffer[pos + 2];
                        Color top = new Color(buffer[pos + 3], buffer[pos + 4], buffer[pos + 5]);

                        c.SetVoxel(vx, vy, vz, true, top);
                        pos += 6;

                    }
                    else
                    {
                        pos++;
                        break;
                    }

                }

                c.UpdateMesh();

            }

            GC.Collect();

        }
        
    }
}
