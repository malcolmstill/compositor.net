
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;
// using System.Runtime.InteropServices;

namespace WindowManager
{
    public class XdgShellV6Global : ZxdgShellV6Global
    {
			public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
			{
				WMXdgShellV6 shell = new WMXdgShellV6(client, 1, id, client);
			}
    }
}
