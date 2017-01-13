
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;
using System.Runtime.InteropServices;

namespace Starfury
{
    public class SfXdgToplevelV6 : ZxdgToplevelV6, ISurface
    {
		public SfXdgSurfaceV6 sfXdgSurfaceV6 { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;

		public SfXdgToplevelV6(IntPtr client, UInt32 id) : base(client, id)
		{
		
		}

		public SfXdgToplevelV6(IntPtr client, UInt32 id, IntPtr resource) : base(client, id, resource)
		{
			IntPtr xdgSurfacePtr = resource; //Resource.GetUserData(resource);
			Resource r = this.client.FindResource(xdgSurfacePtr);
			if (r != null)
			{
				this.sfXdgSurfaceV6 = (SfXdgSurfaceV6) r;
			}
		}

		/*
		public override void Destroy(IntPtr client, IntPtr resource)
		{
			Console.WriteLine("Surface removed " + this);
			Starfury.RemoveSurface(this.resource); 
			this.Remove();
		}
		*/

		public override void Delete(IntPtr resource)
		{
			Starfury.RemoveSurface(this);
		}

		public SfSurface GetSurface()
		{
			return sfXdgSurfaceV6.GetSurface();
		}

		public void Render()
		{

		}

		public void Activate()
		{
			WlArray array = new WlArray();
			array.Set(array.Add(4), 4);
			this.SendConfigure(0, 0, array.array);
			this.sfXdgSurfaceV6.SendConfigure(0);
		}

		public void Deactivate()
		{
			WlArray array = new WlArray();
			this.SendConfigure(0, 0, array.array);
			this.sfXdgSurfaceV6.SendConfigure(0);
		}

		public void SendMouseButton(uint time, uint button, uint state)
		{
			this.GetSurface().SendMouseButton(time, button, state);
		}

		public void SendMouseEnter(int x, int y)
		{
			this.GetSurface().SendMouseEnter(x, y);
		}

		public void SendMouseLeave()
		{
			this.GetSurface().SendMouseLeave();
		}

		public void SendMouseMove(uint time, int x, int y)
		{
			this.GetSurface().SendMouseMove(time, x, y);
		}

		public void SendKeyboardEnter()
		{
			this.GetSurface().SendKeyboardEnter();
		}
		
		public void SendKeyboardLeave()
		{
			this.GetSurface().SendKeyboardLeave();
		}

		public void SendKey(uint time, uint key, uint state)
		{
			this.GetSurface().SendKey(time, key, state);
		}

		public override void Move(IntPtr client, IntPtr resource, IntPtr seat, UInt32 serial)
		{
			Starfury.MovingSurface = new MovingSurface(this, this.X, this.Y, Starfury.Compositor.Mouse.X, Starfury.Compositor.Mouse.Y);
		}
    }
    
    public class SfXdgSurfaceV6 : ZxdgSurfaceV6, ISurface
    {
		public SfSurface sfSurface { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }

		public SfXdgSurfaceV6(IntPtr client, UInt32 id) : base(client, id)
		{
		
		}

		public SfXdgSurfaceV6(IntPtr client, UInt32 id, IntPtr resource) : base(client, id, resource)
		{
			IntPtr surfacePtr = Resource.GetUserData(resource);
			Resource r = this.client.FindResource(surfacePtr);
			if (r != null)
			{
				this.sfSurface = (SfSurface) r;
			}
		}

		public override void GetToplevel(IntPtr client, IntPtr resource, UInt32 id)
		{
			SfXdgToplevelV6 sfXdgToplevelV6 = new SfXdgToplevelV6(client, id, this.resource);

			Starfury.RemoveSurface(this.resource); 
			Starfury.Surfaces.Add(sfXdgToplevelV6);
			Starfury.CurrentVirtualDesktop.Surfaces.Add(sfXdgToplevelV6);

			WlArray array = new WlArray();
			sfXdgToplevelV6.SendConfigure(0, 0, array.array);
			sfXdgToplevelV6.sfXdgSurfaceV6.SendConfigure(0);
		}

		public override void Delete(IntPtr resource)
		{
			Starfury.RemoveSurface(this);
		}

		public SfSurface GetSurface()
		{
			return sfSurface;
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
			this.GetSurface().SendMouseButton(time, button, state);
		}

		public void SendMouseEnter(int x, int y)
		{
			this.GetSurface().SendMouseEnter(x, y);
		}

		public void SendMouseLeave()
		{
			this.GetSurface().SendMouseLeave();
		}

		public void SendMouseMove(uint time, int x, int y)
		{
			this.GetSurface().SendMouseMove(time, x, y);
		}		

		public void SendKeyboardEnter()
		{
			this.GetSurface().SendKeyboardEnter();
		}
		
		public void SendKeyboardLeave()
		{
			this.GetSurface().SendKeyboardLeave();
		}

		public void SendKey(uint time, uint key, uint state)
		{
			this.GetSurface().SendKey(time, key, state);
		}
    }
    
    public class SfXdgShellV6 : ZxdgShellV6
    {
		public SfXdgShellV6(IntPtr client, UInt32 id) : base(client, id)
		{
			
		}

		public SfXdgShellV6(IntPtr client, UInt32 id, IntPtr resource) : base(client, id, resource)
		{
			
		}
		
		public override void GetXdgSurface(IntPtr client, IntPtr resource, UInt32 id, IntPtr surfaceResource)
		{
			IntPtr surfacePtr = Resource.GetUserData(surfaceResource);
			SfXdgSurfaceV6 sfXdgSurface = new SfXdgSurfaceV6(client, id, surfacePtr); // We store the original surface ptr in the new resource
			Starfury.RemoveSurface(surfaceResource);
			Starfury.Surfaces.Add(sfXdgSurface);
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
