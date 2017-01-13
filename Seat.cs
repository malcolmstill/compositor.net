using System;
using Wayland.Server;
using Wayland.Server.Protocol;

namespace Starfury
{
    public class Seat : WlSeat
    {
		public Seat(IntPtr client, UInt32 id) : base(client, id)
		{
			// Call the base class constructor
		}

		public override void GetPointer(IntPtr client, IntPtr resource, UInt32 id)
		{
			WlPointer pointer = new WlPointer(client, id);
			pointer.client.pointer = pointer;
		}
		
		public override void GetKeyboard(IntPtr client, IntPtr resource, UInt32 id)
		{
			WlKeyboard keyboard = new WlKeyboard(client, id);
			keyboard.client.keyboard = keyboard;
			if (keyboard.GetVersion() >= 4)
			{
				keyboard.SendRepeatInfo(30, 200);
			}
			Tuple<int, int> fdAndSize = Starfury.keymap.GetKeymap();
			keyboard.SendKeymap(1, fdAndSize.Item1, (uint) fdAndSize.Item2);
		}
    }
}
