
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WindowManager
{
    public class WMXdgToplevelV6 : ZxdgToplevelV6, ISurface
    {
		public WMXdgSurfaceV6 WMXdgSurfaceV6 { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;

		public WMXdgToplevelV6(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
		
		}

		public WMXdgToplevelV6(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
		{
			IntPtr xdgSurfacePtr = resource;
			Resource r = this.client.FindResource(xdgSurfacePtr);
			if (r != null)
			{
				this.WMXdgSurfaceV6 = (WMXdgSurfaceV6) r;
				this.WMXdgSurfaceV6.Surface.Role = this;
			}
		}

		public override void SetMinSize(IntPtr client, IntPtr resource, Int32 width, Int32 height)
		{

		}

		public override void SetMaxSize(IntPtr client, IntPtr resource, Int32 width, Int32 height)
		{

		}

		public override void Delete(IntPtr resource)
		{
			Console.WriteLine("Delete called on " + this);
			WindowManager.RemoveSurface(this);
		}

		public WMSurface Surface
		{
			get {
				return WMXdgSurfaceV6.Surface;
			}
		}

		public void Render()
		{

		}

		public void Activate()
		{
			WlArray array = new WlArray();
			array.Set(array.Add(4), 4);
			this.SendConfigure(0, 0, array.array);
			this.WMXdgSurfaceV6.SendConfigure(0);
		}

		public void Deactivate()
		{
			WlArray array = new WlArray();
			this.SendConfigure(0, 0, array.array);
			this.WMXdgSurfaceV6.SendConfigure(0);
		}

		public void SendMouseButton(uint time, uint button, uint state)
		{
			this.Surface.SendMouseButton(time, button, state);
		}

		public void SendMouseEnter(int x, int y)
		{
			this.Surface.SendMouseEnter(x, y);
		}

		public void SendMouseLeave()
		{
			this.Surface.SendMouseLeave();
		}

		public void SendMouseMove(uint time, int x, int y)
		{
			this.Surface.SendMouseMove(time, x, y);
		}

		public void SendKeyboardEnter()
		{
			this.Surface.SendKeyboardEnter();
		}
		
		public void SendKeyboardLeave()
		{
			this.Surface.SendKeyboardLeave();
		}

		public void SendKey(uint time, uint key, uint state)
		{
			this.Surface.SendKey(time, key, state);
		}

		public void SendMods(uint mods_depressed, uint mods_latched, uint mods_locked, uint group)
		{
			this.Surface.SendMods(mods_depressed, mods_latched, mods_locked, group);
		}

		public List<ISurface> Subsurfaces { get; set; } = new List<ISurface>();

		public override void Move(IntPtr client, IntPtr resource, IntPtr seat, UInt32 serial)
		{
			WindowManager.MovingSurface = new MovingSurface(this, this.X, this.Y, WindowManager.Compositor.Mouse.X, WindowManager.Compositor.Mouse.Y);
		}
		
    }
    
    public class WMXdgSurfaceV6 : ZxdgSurfaceV6, ISurface
    {
		public WMSurface WMSurface { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }

		public WMXdgSurfaceV6(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
		
		}

		public WMXdgSurfaceV6(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
		{
			IntPtr surfacePtr = Resource.GetUserData(resource);
			Resource r = this.client.FindResource(surfacePtr);
			if (r != null)
			{
				this.WMSurface = (WMSurface) r;
				this.WMSurface.Role = this;
			}
		}

		public override void GetToplevel(IntPtr client, IntPtr resource, UInt32 id)
		{
			WMXdgToplevelV6 WMXdgToplevelV6 = new WMXdgToplevelV6(client, 1, id, this.resource);

			//WindowManager.RemoveSurface(this.resource); Don't need this anymore because we just store the WMSurface in list of Surfaces 
			// ...and we can access the current role via .Role
			//WindowManager.Surfaces.Add(WMXdgToplevelV6);
			
			// HERE
			WindowManager.CurrentVirtualDesktop.Surfaces.Add(WMXdgToplevelV6);

			WlArray array = new WlArray();
			WMXdgToplevelV6.SendConfigure(0, 0, array.array);
			WMXdgToplevelV6.WMXdgSurfaceV6.SendConfigure(0);
		}

		public override void Delete(IntPtr resource)
		{
			WindowManager.RemoveSurface(this);
		}

		public override void SetWindowGeometry(IntPtr client, IntPtr resource, Int32 x, Int32 y, Int32 width, Int32 height)
		{
			
		}

		public WMSurface Surface
		{
			get {
				return WMSurface;
			}
		}

		public void Render()
		{

		}

		public void Activate()
		{

		}

		public void Deactivate()
		{

		}

		public void SendMouseButton(uint time, uint button, uint state)
		{
			this.Surface.SendMouseButton(time, button, state);
		}

		public void SendMouseEnter(int x, int y)
		{
			this.Surface.SendMouseEnter(x, y);
		}

		public void SendMouseLeave()
		{
			this.Surface.SendMouseLeave();
		}

		public void SendMouseMove(uint time, int x, int y)
		{
			this.Surface.SendMouseMove(time, x, y);
		}		

		public void SendKeyboardEnter()
		{
			this.Surface.SendKeyboardEnter();
		}
		
		public void SendKeyboardLeave()
		{
			this.Surface.SendKeyboardLeave();
		}

		public void SendKey(uint time, uint key, uint state)
		{
			this.Surface.SendKey(time, key, state);
		}

		public void SendMods(uint mods_depressed, uint mods_latched, uint mods_locked, uint group)
		{
			this.Surface.SendMods(mods_depressed, mods_latched, mods_locked, group);
		}

		public List<ISurface> Subsurfaces { get; set; } = new List<ISurface>();
    }
    
    public class WMXdgShellV6 : ZxdgShellV6
    {
		public WMXdgShellV6(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
			
		}

		public WMXdgShellV6(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
		{
			
		}
		
		public override void GetXdgSurface(IntPtr client, IntPtr resource, UInt32 id, IntPtr surfaceResource)
		{
			IntPtr surfacePtr = Resource.GetUserData(surfaceResource);
			WMXdgSurfaceV6 WMXdgSurface = new WMXdgSurfaceV6(client, 1, id, surfacePtr); // We store the original surface ptr in the new resource
			//WindowManager.RemoveSurface(surfaceResource);
			// WindowManager.Surfaces.Add(WMXdgSurface);
			// We're probably safe to just add this to the global Surface list at the moment...
			// add to a particular VirtualDesktop on first commit
		}

		public override void Destroy(IntPtr client, IntPtr resource)
		{
			Display.RemoveClient(client);
		}

		public override void Delete(IntPtr resource)
		{
			Display.RemoveClient(client);
		}
    }
}
