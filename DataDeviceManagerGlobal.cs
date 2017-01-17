
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace WindowManager
{
    public class DataDeviceManagerGlobal : WlDataDeviceManagerGlobal
    {
	    public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
	    {
            new WMDataDeviceManager(client, 1, id);
	    }
    }
}
