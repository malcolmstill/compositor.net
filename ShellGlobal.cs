
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace Starfury
{
    public class ShellGlobal : WlShellGlobal
    {
		public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
		{
			SfSurface surface = new SfSurface(client, id);
		}
    }
}
