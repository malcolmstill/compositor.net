
using System;
using System.Collections.Generic;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMRegion : WlRegion
    {
        public List<WlRect> Rects { get; set; } = new List<WlRect>();

		public WMRegion(IntPtr clientPtr, Int32 version, UInt32 id) : base(clientPtr, version, id)
		{
			// Call the base class constructor
		}

        public override void Add(IntPtr client, IntPtr resource, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            Rects.Add(new WlRect(x, y, width, height, WlRect.Operation.ADD));
        }

        public override void Subtract(IntPtr client, IntPtr resource, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            Rects.Add(new WlRect(x, y, width, height, WlRect.Operation.SUBTRACT));
        }
    }
}