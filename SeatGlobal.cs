
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Runtime.InteropServices;

namespace WindowManager
{
    public class SeatGlobal : WlSeatGlobal
    {
		public override void Bind(IntPtr client, IntPtr data, UInt32 version, UInt32 id)
		{
	    	WMSeat seat = new WMSeat(client, 4, id);
	    	seat.SendCapabilities(3); // WL_SEAT_CAPABILITY_POINTER | WL_SEAT_CAPABILITY_KEYBOARD
		}
    }
}
