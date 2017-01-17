
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMCompositor : WlCompositor
    {
		public WMCompositor(IntPtr clientPtr, Int32 version, UInt32 id) : base(clientPtr, version, id)
		{
			// Call the base class constructor
		}
		
		public override void CreateSurface(IntPtr clientPtr, IntPtr resourcePtr, UInt32 id)
		{
			WMSurface surface = new WMSurface(clientPtr, 3, id); // Make new Surface : WlSurface resource
			WindowManager.Surfaces.Add(surface);
			//Starfury.CurrentVirtualDesktop.Surfaces.Add(surface);
			//WindowManager.CurrentVirtualDesktop.Surfaces.Add(surface);
		}

		public override void CreateRegion(IntPtr clientPtr, IntPtr resourcePtr, UInt32 id)
		{
			WMRegion region = new WMRegion(clientPtr, 1, id); // Make new Surface : WlSurface resource
			// Starfury.surfaces.Add(surface);
		}
    }
}