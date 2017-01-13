
namespace Starfury
{
	public interface ISurface
	{
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
		SfSurface GetSurface();
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
		//int Texture();
		//List<ISurface> Subsurfaces { get; set; }
	}
}