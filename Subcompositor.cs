
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace Starfury
{
    public class SfSubcompositor : WlSubcompositor
    {
        public SfSubcompositor(IntPtr clientPtr, UIn32 id) : base(clientPtr, id)
        {

        }

        public override void GetSubsurface(IntPtr clientPtr, IntPtr resource, UInt32 id, IntPtr surface, IntPtr parent)
        {
            SfSubsurface subsurface = new SfSubsurface(client, id);
        }
    }
}