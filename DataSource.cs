
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMDataSource : WlDataSource
    {
        public WMDataSource(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
        {

        }
    }
}