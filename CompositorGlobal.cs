
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace Starfury
{
    public class CompositorGlobal : WlCompositorGlobal
    {
			public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
			{
				SfCompositor compositor = new SfCompositor(client, id);
			}
    }
}
