
using System.Collections.Generic;

namespace WindowManager
{
	public interface ISurface
	{
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
		WMSurface Surface { get; }
		void Render();
		void Activate();
		void Deactivate();
		//void Drag(int startX, int startY, int pointerX, int pointerY);
		void SendMouseButton(uint time, uint button, uint state);
		void SendMouseEnter(int x, int y);
		void SendMouseLeave();
		void SendMouseMove(uint time, int x, int y);
		void SendKeyboardEnter();
		void SendKeyboardLeave();
		void SendKey(uint time, uint key, uint state);
		// SendModifiers(UInt32 serial, UInt32 mods_depressed, UInt32 mods_latched, UInt32 mods_locked, UInt32 group) 
		void SendMods(uint mods_depressed, uint mods_latched, uint mods_locked, uint group);
		//int Texture();
		List<ISurface> Subsurfaces { get; set; }
	}
}