
using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMDataDeviceManager : WlDataDeviceManager
    {
        public WMDataDeviceManager(IntPtr clientPtr, Int32 version, UInt32 id) : base(clientPtr, version, id)
		{
			// Call the base class constructor
		}

        public override void GetDataDevice(IntPtr client, IntPtr resource, UInt32 id, IntPtr seat)
        {
            new WMDataDevice(client, Resource.GetVersion(resource), id, seat);
        }

        public override void CreateDataSource(IntPtr client, IntPtr resource, UInt32 id)
        {
            new WMDataSource(client, Resource.GetVersion(resource), id);
        }
    }
}