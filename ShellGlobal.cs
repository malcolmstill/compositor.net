
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace WindowManager
{
	public class ShellGlobal : WlShellGlobal
	{
		public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
		{
			//WMSurface surface = new WMSurface(client, 1, id);
			WMShell shell = new WMShell(client, 1, id);
		}
	}
}
