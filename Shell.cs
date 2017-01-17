
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMShell : WlShell
    {
        public WMShell(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
        {
            
        }

        public override void GetShellSurface(IntPtr client, IntPtr resource, UInt32 id, IntPtr surface)
        {
            WMShellSurface shellSurface = new WMShellSurface(client, 1, id, surface);
            WindowManager.CurrentVirtualDesktop.Surfaces.Add(shellSurface);
        }
    }
}