
using System;
using System.Collections.Generic;

namespace Starfury
{
    public class View
    {
        Stack<Mode> modes = new Stack<Mode>();
        List<ISurface> surfaces = new List<ISurface>();
        ISurface activeSurface = null;

        public void PushMode(Mode mode)
        {
            modes.Push(mode);
        }

        public void PopMode()
        {
            modes.Pop();
        }

        public virtual void AddSurface(ISurface surface)
        {
            surfaces.Insert(0, surface);
        }
    }
}