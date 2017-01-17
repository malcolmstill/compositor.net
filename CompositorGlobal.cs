
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class CompositorGlobal : WlCompositorGlobal
    {
			public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
			{
				WMCompositor compositor = new WMCompositor(client, 1, id);
			}
    }
}
