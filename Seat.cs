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
			Console.WriteLine("Overloaded GetPointer");
		}
		
		public override void GetKeyboard(IntPtr client, IntPtr resource, UInt32 id)
		{
			WlKeyboard keyboard = new WlKeyboard(client, id);
			if (keyboard.GetVersion() >= 4)
			{
				keyboard.SendRepeatInfo(30, 200);
			}
			Tuple<int, int> fdAndSize = Starfury.keymap.GetKeymap();
			keyboard.SendKeymap(1, fdAndSize.Item1, (uint) fdAndSize.Item2);
		}
    }
}
