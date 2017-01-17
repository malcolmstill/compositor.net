
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMSubcompositor : WlSubcompositor
    {
        public WMSubcompositor(IntPtr clientPtr, Int32 version, UInt32 id) : base(clientPtr, version, id)
        {

        }

        public override void GetSubsurface(IntPtr clientPtr, IntPtr resource, UInt32 id, IntPtr surface, IntPtr parent)
        {
            WMSubsurface subsurface = new WMSubsurface(clientPtr, Resource.GetVersion(resource), id, surface);
            Console.WriteLine("Creating SUBSURFACE: " + subsurface + " for PARENT SURFACE: " + WindowManager.FindSurface(parent).Surface);
            subsurface.Parent = WindowManager.FindSurface(parent).Surface;
            subsurface.Parent.Subsurfaces.Add(subsurface);
        }
    }
}