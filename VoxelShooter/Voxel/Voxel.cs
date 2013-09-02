using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelShooter
{
    public enum VoxelType : byte
    {
        Ground,
        Tree,
        Leaf,
        Water,
        Prefab
    }

    public struct Voxel
    {
        public const float SIZE = 0.5f;
        public const float HALF_SIZE = SIZE / 2f;

        public bool Active;// = false;

        public byte TR;
        public byte TG;
        public byte TB;
        public byte SR;
        public byte SG;
        public byte SB;
        
        public byte Destructable;

        public VoxelType Type;

        public Voxel(bool active, VoxelType type, byte tr, byte tg, byte tb, byte sr,byte sg,byte sb, byte destruct)
        {
            Active = active;
            Type = type;
            TR = tr;
            TG = tg;
            TB = tb;
            SR = sr;
            SG = sg;
            SB = sb;
            Destructable = destruct;
        }
    }
}
