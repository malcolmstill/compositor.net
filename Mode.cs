
using System;
using System.Collections.Generic;
using System.Linq;
using Wayland.Server;

namespace WindowManager
{
    public class KeyBinding
    {
        public enum State
        {
            PRESSED,
            RELEASED
        }

        //public State op;
        //public Key key;
        //public Mods mod;

        public virtual void Function(Mode mode)
        {

        }
    }

    /*
    public interface IMode
    {
        void Render(int width, int height);
        void MouseMove(int x, int y);
        void MouseDown();
        ISurface SurfaceUnderPointer(int x, int y);
    }
    */

    public class Mode
    {
        private static List<KeyBinding> keyBindings = new List<KeyBinding>();
        public VirtualDesktop virtualDesktop;
        // public Screen screen;

        public virtual void KeyPress(uint time, uint key, uint state)
        {
            /*
            ISurface surface = view.ActiveSurface();
            foreach (KeyBinding kb in keyBindings)
            {
                if (kb.op == State.PRESSED && (key != null || key == kb.key) && state == 1 )
                {
                    // cancel mods
                    kb.Function(this);
                    return;
                }
                if (kb.op == State.RELEASED && (key != null || key == kb.key) && state == 1 )
                {
                    // cancel mods
                    kb.Function(this);
                    return;
                }
            }
            if(surface != null)
            {
                surface.Client.Keyboard.SendKey(0, time, key, state);
                surface.Client.Keyboard.SendModifiers(0, time, key, state);
            }
            */
        }

        public virtual void Render(int width, int height)
        {

        }

        public virtual void MouseMove(uint time, int x, int y)
        {

        }

        public virtual void MouseButton(uint time, uint button, uint state)
        {

        }

        public virtual ISurface SurfaceUnderPointer(int x, int y)
        {
            foreach (ISurface surface in this.virtualDesktop.Surfaces.AsEnumerable().Reverse())
            {
                if (surface.Surface.InputRegion == null)
                {
                    if (x >= surface.X && x <= (surface.X + surface.Surface.Width) && y >= surface.Y && y <= (surface.Y + surface.Surface.Height))
                    {
                        return surface;
                    }
                }
                else
                {
                    bool inSurface = false;
                    foreach (WlRect rect in surface.Surface.InputRegion.Rects.AsEnumerable().Reverse())
                    {
                        if (rect.operation == WlRect.Operation.ADD && rect.ContainsPoint(x - surface.X, y - surface.Y))
                        {
                            inSurface = true;
                            break;
                        }
                        if (rect.operation == WlRect.Operation.ADD && rect.ContainsPoint(x - surface.X, y - surface.Y))
                        {
                            break;
                        }
                    }
                    if (inSurface)
                    {
                        return surface;
                    }
                }
            }        
            return null;  
        }
    }
}
