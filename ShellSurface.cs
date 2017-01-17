
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Collections.Generic;

namespace WindowManager
{
    public class WMShellSurface : WlShellSurface, ISurface
    {
		public WMSurface WMSurface { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;

		public WMShellSurface(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
		
		}

        public WMShellSurface(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
		{
			IntPtr surfacePtr = Resource.GetUserData(resource);
			Resource r = this.client.FindResource(surfacePtr);
			if (r != null)
			{
                Console.WriteLine("WlShellSurface: setting WMSurface to " + r);
				this.WMSurface = (WMSurface) r;
				this.WMSurface.Role = this;
			}
		}

		public override void Delete(IntPtr resource)
		{
			Console.WriteLine("Delete called on " + this);
			WindowManager.RemoveSurface(this);
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

		public override void Move(IntPtr client, IntPtr resource, IntPtr seat, UInt32 serial)
		{
			WindowManager.MovingSurface = new MovingSurface(this, this.X, this.Y, WindowManager.Compositor.Mouse.X, WindowManager.Compositor.Mouse.Y);
		}
    }
}