
using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowManager
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
                                        vec4 color = texture(tex, Texcoord);
                                        outColor = vec4(color.b, color.g, color.r, color.a);
                                    }";
        Pipeline desktopPipeline;
        float width;
        float height;
        int orthoLocation;
        Matrix4 orthographicProjection;
        // ISurface virtualDesktop.ActiveSurface { get; set; }

        public DesktopMode()
        {
            desktopPipeline = new Pipeline(vertexSource, fragmentSource);
        }

        public void FirstCommit(WMSurface surface)
        {
            
        }
        
        public void RenderSurface(WMSurface surface)
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
            surface.SendDone();
            //surface.callback.Remove();
            //surface.callback = null;
        }

        public void RenderSurface(ISurface surface)
        {
            if (surface.Surface.committed)
            {
                //Console.WriteLine("Surface offset: " + surface.X + " " + surface.Y);
                int translateLocation = desktopPipeline.GetUniformLocation("translate");
                Matrix4 translate = Matrix4.CreateTranslation(surface.X, surface.Y, 0.0f);
                GL.UniformMatrix4(translateLocation, false, ref translate);
                this.RenderSurface(surface.Surface);
                foreach (ISurface subsurface in surface.Surface.Subsurfaces)
                {
                    if (subsurface.Surface.committed)
                    {
                        translate = Matrix4.CreateTranslation(surface.X + subsurface.X, surface.Y + subsurface.Y, 0.0f);
                        GL.UniformMatrix4(translateLocation, false, ref translate);
                        this.RenderSurface(subsurface.Surface);
                    }
                }
            }
        }

        /*
        public void RenderSurface(WMXdgToplevelV6 surface)
        {
            if (surface.Surface.committed)
            {
                //Console.WriteLine("Surface offset: " + surface.X + " " + surface.Y);
                int translateLocation = desktopPipeline.GetUniformLocation("translate");
                Matrix4 translate = Matrix4.CreateTranslation(surface.X, surface.Y, 0.0f);
                GL.UniformMatrix4(translateLocation, false, ref translate);
                this.RenderSurface(surface.Surface);
                foreach (ISurface subsurface in surface.Subsurfaces)
                {
                    // Console.WriteLine("Drawing subsurface: " + subsurface);
                    if (subsurface.Surface.committed)
                    {
                        //Console.WriteLine("Subsurface offset: " + subsurface.X + " " + subsurface.Y);
                        translate = Matrix4.CreateTranslation(surface.X + subsurface.X, surface.Y + subsurface.Y, 0.0f);
                        GL.UniformMatrix4(translateLocation, false, ref translate);
                        this.RenderSurface(subsurface.Surface);
                    }
                }
            }

        }
        */

        public override void KeyPress(uint time, uint key, uint state)
        {
            virtualDesktop.ActiveSurface?.SendKey(time, key, state);
            virtualDesktop.ActiveSurface?.SendMods(WindowManager.Mods[0], WindowManager.Mods[1], WindowManager.Mods[2], WindowManager.Mods[3]);
        }

        public override void MouseMove(uint time, int x, int y)
        {
            WindowManager.MovingSurface?.Update(x, y);

            ISurface surface = SurfaceUnderPointer(WindowManager.Compositor.Mouse.X, WindowManager.Compositor.Mouse.Y);
            if (surface != WindowManager.PointerSurface)
            {
                WindowManager.PointerSurface?.SendMouseLeave();
                WindowManager.PointerSurface = surface;
                WindowManager.PointerSurface?.SendMouseEnter(x - WindowManager.PointerSurface.X, y - WindowManager.PointerSurface.Y);
            }

            WindowManager.PointerSurface?.SendMouseMove(time, x - WindowManager.PointerSurface.X, y - WindowManager.PointerSurface.Y);
        }

        public override void MouseButton(uint time, uint button, uint state)
        {
            // Case where mouse button 1 is released and there is a currently moving surface...
            if (state == 0 && button == 0x110 && WindowManager.MovingSurface != null)
            {
                // ...the surface should stop moving
                WindowManager.MovingSurface = null;
            }

            // Find the topmost surface under the pointer
            ISurface surface = SurfaceUnderPointer(WindowManager.Compositor.Mouse.X, WindowManager.Compositor.Mouse.Y);
            if (surface != null && surface != virtualDesktop.ActiveSurface && state == 1)
            {
                //Console.WriteLine("Deactivating active surface " + virtualDesktop.ActiveSurface);
                virtualDesktop.ActiveSurface?.Deactivate();
                virtualDesktop.ActiveSurface?.SendKeyboardLeave();

                this.virtualDesktop.Surfaces.Remove(surface);
                this.virtualDesktop.Surfaces.Add(surface);
                surface.Activate();
                surface.SendKeyboardEnter();
                virtualDesktop.ActiveSurface = surface;
            }
            else if (surface == null && state == 1)
            {
                //Console.WriteLine("Deactivating active surface " + virtualDesktop.ActiveSurface);
                virtualDesktop.ActiveSurface?.Deactivate();
                virtualDesktop.ActiveSurface?.SendKeyboardLeave();
                virtualDesktop.ActiveSurface = null;
            }
            else
            {

            }

            surface?.SendMouseButton(time, button, state);
        }

        /*
        public override ISurface SurfaceUnderPointer(int x, int y)
        {
            foreach (ISurface surface in this.virtualDesktop.Surfaces.AsEnumerable().Reverse())
            {
                if (surface.Surace.InputRegion == null)
                {
                    if (x >= surface.X && x <= (surface.X + surface.Surface.Width) && y >= surface.Y && y <= (surface.Y + surface.Surface.Height))
                    {
                        return surface;
                    }
                }
            }        
            return null;    
        }
        */

        /*
        public void RenderSurface(WMSubsurface subsurface)
        {

        }

        public void RenderSurface(WMCursor cursor)
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
            //Console.WriteLine("Rendering surfaces:");
            foreach (ISurface surface in this.virtualDesktop.Surfaces)
            {
                //Console.WriteLine(surface);
                RenderSurface(surface);
            }
            //Console.WriteLine("Done rendering");
        }
    }
}