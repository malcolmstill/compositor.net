
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace Starfury
{
    public class OutputGlobal : WlOutputGlobal
    {
	public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
	{
	    WlOutput output = new WlOutput(client, id);
	    output.SendGeometry(0, 0, 1440, 900, 0, "apple", "apple", 0);
	}
    }
}
