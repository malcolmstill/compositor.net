
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace Starfury
{
    public class SfSubsurface : WlSubsurface, ISurface
    {
        /*
		public SfSurface sfSurface { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
        */

        public override void SetPosition(IntPtr client, IntPtr resource, int x, int y)
        {
            X = x;
            Y = y;           
        }
    }
}