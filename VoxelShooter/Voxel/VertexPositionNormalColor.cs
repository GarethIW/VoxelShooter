using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 position;
        public Vector3 normal;
        public Color color;

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            this.position = position;
            this.normal = normal;
            this.color = color;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                                                                    new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                                                                    new VertexElement(sizeof(float) * 6, VertexElementFormat.Color, VertexElementUsage.Color, 0));

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

}
