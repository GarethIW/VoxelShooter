using System;

namespace VoxelShooter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (VoxelShooter game = new VoxelShooter())
            {
                game.Run();
            }
        }
    }
#endif
}

