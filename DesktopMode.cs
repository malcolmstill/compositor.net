
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Starfury
{
    public class DesktopMode : Mode, IMode
    {
        string vertexSource = "#version 130\n" + 
                                "in vec3 position;\n" +
                                "in vec2 texcoord;\n" +
                                "out vec2 Texcoord;\n" +
                                "uniform mat4 ortho;" +
                                "void main()\n" +
                                "{\n" +
                                "    Texcoord = texcoord;\n" + 
                                "    gl_Position = ortho * vec4(position, 1.0);\n" +
                                "}";
        string fragmentSource = @"#version 130
                                    in vec2 Texcoord;
                                    out vec4 outColor;
                                    uniform sampler2D tex;
                                    void main()
                                    {
                                        outColor = texture(tex, Texcoord);
                                        //outColor = vec4(1.0, 0.0, 0.0, 0.5);
                                        //outColor = mix(vec4(1.0,0.0,0.0,0.5), texture(tex, Texcoord), 0.5);
                                    }";
        Pipeline desktopPipeline;
        float width;
        float height;
        int orthoLocation;
        Matrix4 orthographicProjection;

        public DesktopMode(int width, int height)
        {
            desktopPipeline = new Pipeline(vertexSource, fragmentSource);
        }

        public void FirstCommit(SfSurface surface)
        {
            
        }
        
        public void RenderSurface(SfSurface surface)
        {
            /*
            GL.BindTexture(TextureTarget.Texture2D, surface.Texture());
            GL.Begin(BeginMode.Triangles);
            GL.Vertex3(-1.0f, -1.0f, 0.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(1.0f, -1.0f, 0.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(1.0f, 1.0f, 0.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.End();
            */

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
            this.RenderSurface(surface.GetSurface());
        }

        /*
        public void RenderSurface(SfSubsurface subsurface)
        {

        }

        public void RenderSurface(SfCursor cursor)
        {

        }
        */

        // Implement IMode.Render
        public void Render(int width, int height)
        {
            desktopPipeline.Use();
            // GL.Enable (EnableCap.Texture2D);
            orthographicProjection = Matrix4.CreateOrthographicOffCenter(0.0f, width, height, 0.0f, 1.0f, -1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            orthoLocation = desktopPipeline.GetUniformLocation("ortho");
            GL.UniformMatrix4(orthoLocation, false, ref orthographicProjection);
            
            foreach (dynamic surface in Starfury.surfaces)
            {
                RenderSurface(surface);
                surface.GetSurface().SendDone();
            }
        }
    }
}