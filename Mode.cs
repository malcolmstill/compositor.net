
using System;
using System.Collections.Generic;

namespace Starfury
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

    public interface IMode
    {
        void Render(int width, int height);
    }

    public class Mode
    {
        private static List<KeyBinding> keyBindings = new List<KeyBinding>();
        private int view;

        public void KeyboardHandler(UInt32 time, int key, int state)
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
    }
}
