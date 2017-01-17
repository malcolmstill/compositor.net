
using System;
using OpenTK.Graphics.OpenGL;

namespace WindowManager
{
    public class Pipeline : IDisposable
    {
        int vertexShaderID;
        int fragmentShaderID;
        int programID;

        private void LoadShader(string text, ShaderType type, int program, out int shader)
        {
            shader = GL.CreateShader(type);
            GL.ShaderSource(shader, text);
            GL.CompileShader(shader);
            Console.WriteLine(GL.GetShaderInfoLog(shader));
            int compileStatus;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0) {
                Console.WriteLine("Failed to compile shader " + shader + " with status " + compileStatus);
                Console.WriteLine("Shader code:");
                Console.WriteLine(text);
            }
            GL.AttachShader(program, shader);
        }

        public Pipeline(string vertexShaderText, string fragmentShaderText)
        {
            programID = GL.CreateProgram();
            Console.WriteLine("Creating shader program " + programID + " (language version needs to be " + GL.GetString(StringName.ShadingLanguageVersion) + ")");
            this.LoadShader(vertexShaderText, ShaderType.VertexShader, programID, out vertexShaderID);
            this.LoadShader(fragmentShaderText, ShaderType.FragmentShader, programID, out fragmentShaderID);
            GL.LinkProgram(programID);           
            int linkStatus;
            GL.GetProgram(programID, ProgramParameter.LinkStatus, out linkStatus);
            if (linkStatus == 0)
            {
                Console.WriteLine("Failed to link shader program" + programID + " with status " + linkStatus);
            }
            GL.ValidateProgram(programID);
        }

        public void Use()
        {
            GL.UseProgram(programID);
        }

        
        public void Draw()
        {
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(BeginMode.Triangles, 0,3);
        }

        public int GetUniformLocation(string location)
        {
            return GL.GetUniformLocation(programID, location);
        }

        public int GetAttribLocation(string location)
        {
            return GL.GetAttribLocation(programID, location);
        }

        public void Dispose()
        {
            
        }
    }
}