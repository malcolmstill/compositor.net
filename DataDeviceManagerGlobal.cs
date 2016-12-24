
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace Starfury
{
    public class DataDeviceManagerGlobal : WlDataDeviceManagerGlobal
    {
	public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
	{

	}
    }
}
