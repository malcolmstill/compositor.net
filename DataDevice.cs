
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMDataDevice : WlDataDevice
    {
        public WMDataDevice(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
        {

        }

        public WMDataDevice(IntPtr client, Int32 version, UInt32 id, IntPtr resource) : base(client, version, id, resource)
        {
            
        }
    }
}