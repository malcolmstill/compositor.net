
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;

namespace Starfury
{
    public class SfXdgToplevelV6 : ZxdgToplevelV6, ISurface
    {
		public SfXdgSurfaceV6 sfXdgSurfaceV6 { get; set; }
		public int x { get; set; } = 0;
		public int y { get; set; } = 0;
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;
		public int width { get; set; } = 0;
		public int height { get; set; } = 0;

		public SfXdgToplevelV6(IntPtr client, UInt32 id) : base(client, id)
		{
		
		}

		public SfXdgToplevelV6(IntPtr client, UInt32 id, IntPtr resource) : base(client, id, resource)
		{
			// Client c = Display.GetClient(client);
			IntPtr xdgSurfacePtr = resource; //Resource.GetUserData(resource);
			Resource r = this.client.FindResource(xdgSurfacePtr);
			if (r != null)
			{
				this.sfXdgSurfaceV6 = (SfXdgSurfaceV6) r;
			}
		}

		public override void Destroy(IntPtr client, IntPtr resource)
		{
			Console.WriteLine("Surface removed " + this);
			Starfury.RemoveSurface(this.resource); 
			this.Remove();
		}

		public override void Delete(IntPtr resource)
		{
			Console.WriteLine("DELETING XdgToplevel");
			Console.WriteLine("Compositor's surfaces before deletion");
			foreach (ISurface surface in Starfury.surfaces)
			{
				Console.WriteLine(surface);
			}
			Starfury.RemoveSurface(this);
			Console.WriteLine("Compositor's surfaces after deletion");
			foreach (ISurface surface in Starfury.surfaces)
			{
				Console.WriteLine(surface);
			} 
			//this.Remove();
		}


		public SfSurface GetSurface()
		{
			return sfXdgSurfaceV6.GetSurface();
		}

		public void Render()
		{

		}
    }
    
    public class SfXdgSurfaceV6 : ZxdgSurfaceV6, ISurface
    {
		public SfSurface sfSurface { get; set; }
		// IntPtr resource === pointer to ZxgSurfaceV6

		public SfXdgSurfaceV6(IntPtr client, UInt32 id) : base(client, id)
		{
		
		}

		public SfXdgSurfaceV6(IntPtr client, UInt32 id, IntPtr resource) : base(client, id, resource)
		{
			//Client c = Display.GetClient(client);
			IntPtr surfacePtr = Resource.GetUserData(resource);
			Resource r = this.client.FindResource(surfacePtr);
			if (r != null)
			{
				this.sfSurface = (SfSurface) r;
			}
		}

		public override void GetToplevel(IntPtr client, IntPtr resource, UInt32 id)
		{
			Console.WriteLine("FUCK SAKE");
			SfXdgToplevelV6 sfXdgToplevelV6 = new SfXdgToplevelV6(client, id, this.resource); // Passing in the ZxdgSurfaceV6 pointer
			// "Change" class of SfXdgSurfaceV6 to SfXdgToplevelV6
			lock (Starfury.surfaces)
			{
				Starfury.RemoveSurface(this.resource); 
				Starfury.surfaces.Add(sfXdgToplevelV6);
			}

			// Configure surface
			WlArray array = new WlArray();
			sfXdgToplevelV6.SendConfigure(0, 0, array.array);
			sfXdgToplevelV6.sfXdgSurfaceV6.SendConfigure(0);
		}

		public override void Delete(IntPtr resource)
		{
			Console.WriteLine("DELETING XdgSurface");
			Console.WriteLine("Compositor's surfaces before deletion");
			foreach (ISurface surface in Starfury.surfaces)
			{
				Console.WriteLine(surface);
			}
			Starfury.RemoveSurface(this);
			Console.WriteLine("Compositor's surfaces after deletion");
			foreach (ISurface surface in Starfury.surfaces)
			{
				Console.WriteLine(surface);
			} 
			//this.Remove();
		}

		public override void Destroy(IntPtr client, IntPtr resource)
		{
			Console.WriteLine("FUCK SAKE");
		}


		public SfSurface GetSurface()
		{
			return sfSurface;
		}

		public void Render()
		{

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
			// 1. Get surface pointer
			// 2. new CnXdgSurface(client, id, surfacePtr)
			// 3. Swap out CnSurface with CnXdgSurface 
			Console.WriteLine("FUCK SAKE");
			IntPtr surfacePtr = Resource.GetUserData(surfaceResource);
			SfXdgSurfaceV6 sfXdgSurface = new SfXdgSurfaceV6(client, id, surfacePtr); // We store the original surface ptr in the new resource
			lock (Starfury.surfaces)
			{
				Starfury.RemoveSurface(surfaceResource);
				Starfury.surfaces.Add(sfXdgSurface);
			}
		}

		public override void Destroy(IntPtr client, IntPtr resource)
		{
			Console.WriteLine("Removing client {0}", client);
			Display.RemoveClient(client);
		}

		public override void Delete(IntPtr resource)
		{
			Console.WriteLine("Removing client {0}", client);
			Display.RemoveClient(client);
		}
    }
}
