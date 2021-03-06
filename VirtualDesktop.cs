
using System;
using System.Collections.Generic;
using Wayland.Server;

namespace WindowManager
{
    public class VirtualDesktop
    {
        private Stack<Mode> Modes = new Stack<Mode>();
        private readonly Mode DefaultMode;
        public Mode CurrentMode
        { 
            get
            { 
                if (Modes.Count == 0)
                {
                    return DefaultMode;
                }
                else
                {
                    return Modes.Peek();
                }
            }   
        }
        public List<ISurface> Surfaces { get; } = new List<ISurface>();
        public ISurface ActiveSurface { get; set; } = null;

        public VirtualDesktop(Mode defaultMode)
        {
            DefaultMode = defaultMode;
            DefaultMode.virtualDesktop = this;
        }

        public void PushMode(Mode mode)
        {
            mode.virtualDesktop = this;
            Modes.Push(mode);
        }

        public void PopMode()
        {
            Modes.Pop();
        }
        
        public void RemoveSurface(ISurface surface)
		{
            //Console.WriteLine("Calling into VD RemoveSurface: " + surface + "(current ActiveSurface: " + ActiveSurface + ")");
			this.Surfaces.Remove(surface);
            if (ActiveSurface == surface)
            {
                ActiveSurface = null;
            }
		}

		public void RemoveSurface(IntPtr resourcePointer)
		{
			ISurface resource = this.Surfaces.Find(r => ((Resource) r).resource == resourcePointer);
			this.Surfaces.Remove(resource);
		}
    }
}