using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public static class ParticleCube
    {
        

        static short[] cubeIndices = new short[] {
                                        0,  1,  2,  // front face
                                        1,  3,  2,
                                        4,  5,  6,  // back face
                                        6,  5,  7,
                                        8,  9, 10,  // top face
                                        8, 11,  9,
                                        12, 13, 14, // bottom face
                                        12, 14, 15,
                                        16, 17, 18, // left face
                                        19, 17, 16,
                                        20, 21, 22, // right face
                                        23, 20, 22  };

        public static void Create(ref VertexPositionNormalColor[] verts, ref short[] indexes, Vector3 pos, int partNum, float scale, Color col)
        {
            int vertOffset = partNum * 24;

            Vector3 topLeftFront = new Vector3(-1.0f, 1.0f, 1.0f) * scale;
            Vector3 bottomLeftFront = new Vector3(-1.0f, -1.0f, 1.0f) * scale;
            Vector3 topRightFront = new Vector3(1.0f, 1.0f, 1.0f) * scale;
            Vector3 bottomRightFront = new Vector3(1.0f, -1.0f, 1.0f) * scale;
            Vector3 topLeftBack = new Vector3(-1.0f, 1.0f, -1.0f) * scale;
            Vector3 topRightBack = new Vector3(1.0f, 1.0f, -1.0f) * scale;
            Vector3 bottomLeftBack = new Vector3(-1.0f, -1.0f, -1.0f) * scale;
            Vector3 bottomRightBack = new Vector3(1.0f, -1.0f, -1.0f) * scale;

            // Front face
            verts[vertOffset+0] = new VertexPositionNormalColor(pos +  topLeftFront, Vector3.Normalize(topLeftFront), col);
            verts[vertOffset+1] = new VertexPositionNormalColor(pos + bottomLeftFront, Vector3.Normalize(bottomLeftFront), col);
            verts[vertOffset+2] = new VertexPositionNormalColor(pos + topRightFront, Vector3.Normalize(topRightFront), col);
            verts[vertOffset+3] = new VertexPositionNormalColor(pos + bottomRightFront, Vector3.Normalize(bottomRightFront), col);

            // Back face
            verts[vertOffset+4] = new VertexPositionNormalColor(pos + topLeftBack, Vector3.Normalize(topLeftBack), col);
            verts[vertOffset+5] = new VertexPositionNormalColor(pos + topRightBack, Vector3.Normalize(topRightBack), col);
            verts[vertOffset+6] = new VertexPositionNormalColor(pos + bottomLeftBack, Vector3.Normalize(bottomLeftBack), col);
            verts[vertOffset+7] = new VertexPositionNormalColor(pos + bottomRightBack, Vector3.Normalize(bottomRightBack), col);

            // Top face
            verts[vertOffset+8] = new VertexPositionNormalColor(pos + topLeftFront, Vector3.Normalize(topLeftFront), col);
            verts[vertOffset+9] = new VertexPositionNormalColor(pos + topRightBack, Vector3.Normalize(topRightBack), col);
            verts[vertOffset+10] = new VertexPositionNormalColor(pos + topLeftBack, Vector3.Normalize(topLeftBack), col);
            verts[vertOffset+11] = new VertexPositionNormalColor(pos + topRightFront, Vector3.Normalize(topRightFront), col);

            // Bottom face
            verts[vertOffset+12] = new VertexPositionNormalColor(pos + bottomLeftFront, Vector3.Normalize(bottomLeftFront), col);
            verts[vertOffset+13] = new VertexPositionNormalColor(pos + bottomLeftBack, Vector3.Normalize(bottomLeftBack), col);
            verts[vertOffset+14] = new VertexPositionNormalColor(pos + bottomRightBack, Vector3.Normalize(bottomRightBack), col);
            verts[vertOffset+15] = new VertexPositionNormalColor(pos + bottomRightFront, Vector3.Normalize(bottomRightFront), col);

            // Left face
            verts[vertOffset+16] = new VertexPositionNormalColor(pos + topLeftFront, Vector3.Normalize(topLeftFront), col);
            verts[vertOffset+17] = new VertexPositionNormalColor(pos + bottomLeftBack, Vector3.Normalize(bottomLeftBack), col);
            verts[vertOffset+18] = new VertexPositionNormalColor(pos + bottomLeftFront, Vector3.Normalize(bottomLeftFront), col);
            verts[vertOffset+19] = new VertexPositionNormalColor(pos + topLeftBack, Vector3.Normalize(topLeftBack), col);

            // Right face
            verts[vertOffset+20] = new VertexPositionNormalColor(pos + topRightFront, Vector3.Normalize(topRightFront), col);
            verts[vertOffset+21] = new VertexPositionNormalColor(pos + bottomRightFront, Vector3.Normalize(bottomRightFront), col);
            verts[vertOffset+22] = new VertexPositionNormalColor(pos + bottomRightBack, Vector3.Normalize(bottomRightBack), col);
            verts[vertOffset+23] = new VertexPositionNormalColor(pos + topRightBack, Vector3.Normalize(topRightBack), col);

            for (int i = 0; i < 36; i++) indexes[(partNum * 36) + i] = (short)((partNum*24) + cubeIndices[i]);
        }
    }

}
