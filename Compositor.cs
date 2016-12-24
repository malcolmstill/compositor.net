
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace Starfury
{
    public class SfCompositor : WlCompositor
    {
		public SfCompositor(IntPtr clientPtr, UInt32 id) : base(clientPtr, id)
		{
			// Call the base class constructor
		}
		
		public override void CreateSurface(IntPtr clientPtr, IntPtr resourcePtr, UInt32 id)
		{
			SfSurface surface = new SfSurface(clientPtr, id); // Make new Surface : WlSurface resource
			Starfury.surfaces.Add(surface);
		}

		public override void CreateRegion(IntPtr clientPtr, IntPtr resourcePtr, UInt32 id)
		{
			SfRegion region = new SfRegion(clientPtr, id); // Make new Surface : WlSurface resource
			// Starfury.surfaces.Add(surface);
		}
    }
}