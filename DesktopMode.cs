
using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Starfury
{
    public class DesktopMode : Mode
    {
        string vertexSource = @"#version 130
                                in vec3 position;
                                in vec2 texcoord;
                                out vec2 Texcoord;
                                uniform mat4 ortho;
                                uniform mat4 translate;
                                void main()
                                {
                                    Texcoord = texcoord;
                                    gl_Position = ortho * translate * vec4(position, 1.0);
                                }";
        string fragmentSource = @"#version 130
                                    in vec2 Texcoord;
                                    out vec4 outColor;
                                    uniform sampler2D tex;
                                    void main()
                                    {
                                        outColor = texture(tex, Texcoord);
                                    }";
        Pipeline desktopPipeline;
        float width;
        float height;
        int orthoLocation;
        Matrix4 orthographicProjection;
        ISurface ActiveSurface { get; set; }

        public DesktopMode()
        {
            desktopPipeline = new Pipeline(vertexSource, fragmentSource);
        }

        public void FirstCommit(SfSurface surface)
        {
            
        }
        
        public void RenderSurface(SfSurface surface)
        {

            int posAttrib = desktopPipeline.GetAttribLocation("position");
            GL.EnableVertexAttribArray(posAttrib);

            int texAttrib = desktopPipeline.GetAttribLocation("texcoord");
            GL.EnableVertexAttribArray(texAttrib);

            surface.Geometry(posAttrib, texAttrib);
            //surface.Geometry(posAttrib, 0);

            int texture = surface.Texture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            desktopPipeline.Draw();
            //Console.WriteLine("Rendering surface");
            //GL.DisableVertexAttribArray(posAttrib);
            //GL.DisableVertexAttribArray(texAttrib);

            GL.Flush();
        }

        public void RenderSurface(SfXdgToplevelV6 surface)
        {
            int translateLocation = desktopPipeline.GetUniformLocation("translate");
            Matrix4 translate = Matrix4.CreateTranslation(surface.X, surface.Y, 0.0f);
            GL.UniformMatrix4(translateLocation, false, ref translate);
            this.RenderSurface(surface.GetSurface());
        }

        public override void KeyPress(uint time, uint key, uint state)
        {
            Console.WriteLine("DesktopMode: key " + key);
            ActiveSurface?.SendKey(time, key, state);
        }

        public override void MouseMove(uint time, int x, int y)
        {
            Starfury.MovingSurface?.Update(x, y);

            ISurface surface = SurfaceUnderPointer(Starfury.Compositor.Mouse.X, Starfury.Compositor.Mouse.Y);
            if (surface != Starfury.PointerSurface)
            {
                Starfury.PointerSurface?.SendMouseLeave();
                Starfury.PointerSurface = surface;
                Starfury.PointerSurface?.SendMouseEnter(x - Starfury.PointerSurface.X, y - Starfury.PointerSurface.Y);
            }

            Starfury.PointerSurface?.SendMouseMove(time, x - Starfury.PointerSurface.X, y - Starfury.PointerSurface.Y);
        }

        public override void MouseButton(uint time, uint button, uint state)
        {
            // Case where mouse button 1 is released and there is a currently moving surface...
            if (state == 0 && button == 0x110 && Starfury.MovingSurface != null)
            {
                // ...the surface should stop moving
                Starfury.MovingSurface = null;
            }

            // Find the topmost surface under the pointer
            ISurface surface = SurfaceUnderPointer(Starfury.Compositor.Mouse.X, Starfury.Compositor.Mouse.Y);
            if (surface != null && surface != ActiveSurface)
            {
                ActiveSurface?.Deactivate();
                ActiveSurface?.SendKeyboardLeave();

                this.virtualDesktop.Surfaces.Remove(surface);
                this.virtualDesktop.Surfaces.Add(surface);
                surface.Activate();
                surface.SendKeyboardEnter();
                ActiveSurface = surface;
            }
            else if (surface == null)
            {
                ActiveSurface?.Deactivate();
                ActiveSurface?.SendKeyboardLeave();
                ActiveSurface = null;
            }
            else
            {

            }

            surface?.SendMouseButton(time, button, state);
        }

        public override ISurface SurfaceUnderPointer(int x, int y)
        {
            foreach (ISurface surface in this.virtualDesktop.Surfaces.AsEnumerable().Reverse())
            {
                if (x >= surface.X && x <= (surface.X + surface.GetSurface().Width) && y >= surface.Y && y <= (surface.Y + surface.GetSurface().Height))
                {
                    return surface;
                }
            }        
            return null;    
        }

        /*
        public void RenderSurface(SfSubsurface subsurface)
        {

        }

        public void RenderSurface(SfCursor cursor)
        {

        }
        */

        public override void Render(int width, int height)
        {
            desktopPipeline.Use();
            // GL.Enable (EnableCap.Texture2D);
            orthographicProjection = Matrix4.CreateOrthographicOffCenter(0.0f, width, height, 0.0f, 1.0f, -1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            orthoLocation = desktopPipeline.GetUniformLocation("ortho");
            GL.UniformMatrix4(orthoLocation, false, ref orthographicProjection);

            // Console.WriteLine(this.virtualDesktop);
            // Console.WriteLine(this.virtualDesktop.Surfaces);
            foreach (dynamic surface in this.virtualDesktop.Surfaces)
            {
                RenderSurface(surface);
                surface.GetSurface().SendDone();
            }
        }
    }
}