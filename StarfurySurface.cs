
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;

namespace Starfury
{
    public class StarfurySurface
	{
		public Client client { get; set; }
		public Surface surfaceResource { get; set; }
		public int x { get; set; } = 0;
		public int y { get; set; } = 0;
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;
		public IntPtr buffer { get; set; } = IntPtr.Zero;
		public WlCallback callbackResource { get; set; }
		public int width { get; set; } = 0;
		public int height { get; set; } = 0;
		//private List<Subsurfaces> subsurfaces;

		public StarfurySurface()
		{

		}

		public StarfuryXdgSurfaceV6 ToStarfuryXdgSurfaceV6(XdgSurfaceV6 xdgSurface)
		{
			StarfuryXdgSurfaceV6 newSurface = new StarfuryXdgSurfaceV6
			{
				client = this.client,
				surfaceResource = this.surfaceResource,
				x = this.x,
				y = this.y,
				originX = this.originX,
				originY = this.originY,
				buffer = this.buffer,
				callbackResource = this.callbackResource,
				width = this.width,
				height = this.height,
				xdgSurfaceResource = xdgSurface
			};
			Starfury.surfaces.Remove(this);
			Starfury.surfaces.Add(newSurface);
			return newSurface;
		}
    }

	public class StarfuryXdgSurfaceV6 : StarfurySurface
	{
		public XdgSurfaceV6 xdgSurfaceResource { get; set; }

		public StarfuryXdgToplevelV6 ToStarfuryXdgToplevelV6(XdgToplevelV6 resource)
		{
			StarfuryXdgToplevelV6 newSurface = new StarfuryXdgToplevelV6
			{
				client = this.client,
				surfaceResource = this.surfaceResource,
				x = this.x,
				y = this.y,
				originX = this.originX,
				originY = this.originY,
				buffer = this.buffer,
				callbackResource = this.callbackResource,
				width = this.width,
				height = this.height,
				xdgSurfaceResource = this.xdgSurfaceResource,
				xdgToplevelResource = resource
			};
			Starfury.surfaces.Remove(this);
			Starfury.surfaces.Add(newSurface);
			return newSurface;
		}
	}

	public class StarfuryXdgToplevelV6 : StarfuryXdgSurfaceV6
	{
		public XdgToplevelV6 xdgToplevelResource { get; set; }
	}
}
