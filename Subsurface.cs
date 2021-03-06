
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Collections.Generic;

namespace WindowManager
{
    public class WMSubsurface : WlSubsurface, ISurface
    {
        public WMSurface surface;
        public ISurface Parent { get; set; }
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
        
        public WMSubsurface(IntPtr clientPtr, Int32 version, UInt32 id) : base(clientPtr, version, id)
        {

        }

        public WMSubsurface(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
		{
			IntPtr surfacePtr = resource;
			Resource r = this.client.FindResource(surfacePtr);
			if (r != null)
			{
				this.surface = (WMSurface) r;
			}
		}
        
		
		public override void Destroy(IntPtr client, IntPtr resource)
		{
			// Client has Destroy'd this subsurface, we need to remove it from the
			// parent's subsurfaces
			Console.WriteLine("Removing subsurface " + this + " which has parent " + Parent);
			Parent.Surface.Subsurfaces.Remove(this);
		}

        public override string ToString()
		{
			return String.Format("WMSubsurface@{0:X8}", resource.ToInt32());
		}

        public WMSurface Surface
        {
            get
            {
                return surface;
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

        public override void SetPosition(IntPtr client, IntPtr resource, Int32 x, Int32 y)
        {
			Console.WriteLine("Setting position of " + this + ": " + x + " " + y);
            X = x;
            Y = y;           
        }
    }
}