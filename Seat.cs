using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace WindowManager
{
    public class WMSeat : WlSeat
    {
		public WMSeat(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
			// Call the base class constructor
		}

		public override void GetPointer(IntPtr client, IntPtr resource, UInt32 id)
		{
			WlPointer pointer = new WlPointer(client, 1, id);
			pointer.client.pointer = pointer;
		}
		
		public override void GetKeyboard(IntPtr client, IntPtr resource, UInt32 id)
		{
			WlKeyboard keyboard = new WlKeyboard(client, Resource.GetVersion(resource), id);
			keyboard.client.keyboard = keyboard;
			if (keyboard.GetVersion() >= 4)
			{
				keyboard.SendRepeatInfo(30, 200);
			}
			Tuple<int, int> fdAndSize = WindowManager.keymap.GetKeymap();
			keyboard.SendKeymap(1, fdAndSize.Item1, (uint) fdAndSize.Item2);
		}
    }
}
