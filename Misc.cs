
using System;

namespace Starfury
{
    public class MovingSurface
    {
        ISurface surface;
        int surfaceX;
        int surfaceY;
        int pointerX;
        int pointerY;

        public MovingSurface(ISurface surface, int surfaceX, int surfaceY, int pointerX, int pointerY)
        {
            this.surface = surface;
            this.surfaceX = surfaceX;
            this.surfaceY = surfaceY;
            this.pointerX = pointerX;
            this.pointerY = pointerY;
        }

        public void Update(int newPointerX, int newPointerY)
        {
            surface.X = surfaceX + newPointerX - pointerX;
            surface.Y = surfaceY + newPointerY - pointerY;
            Starfury.renderNeeded = true;
        }
    }
}