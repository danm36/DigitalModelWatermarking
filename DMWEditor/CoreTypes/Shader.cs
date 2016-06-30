using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public class Shader : IDisposable
    {
        public static List<Shader> loadedShaders = new List<Shader>();

        public int shaderProgramHandle = -1;

        public int vertexAttribLocation = -1;
        public int normalAttribLocation = -1;
        public int uPVMMatrixLocation = -1;
        public int uVertexOffsetLocation = -1;

        Dictionary<string, int> uniformCache = new Dictionary<string, int>();

        public bool shaderLoaded { get; private set; }
        bool disposed = false;

        public Shader(string vertFile, string fragFile)
        {
            int vertexShaderHandle = -1;
            int fragmentShaderHandle = -1;

            shaderLoaded = false;

            GL.UseProgram(0);
            int compileStatusCode = -1;

            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, File.ReadAllText(vertFile));
            GL.CompileShader(vertexShaderHandle);
            GL.GetShader(vertexShaderHandle, ShaderParameter.CompileStatus, out compileStatusCode);
            if (compileStatusCode != 1)
            {
                string error;

                GL.GetShaderInfoLog(vertexShaderHandle, out error);
                error = "Vertex Shader compilation error [" + compileStatusCode + "]\n\n" + error;

                Console.WriteLine(error);
                throw new Exception(error);
            }


            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, File.ReadAllText(fragFile));
            GL.CompileShader(fragmentShaderHandle);
            GL.GetShader(fragmentShaderHandle, ShaderParameter.CompileStatus, out compileStatusCode);
            if (compileStatusCode != 1)
            {
                string error;

                GL.GetShaderInfoLog(fragmentShaderHandle, out error);
                error = "Fragment Shader compilation error [" + compileStatusCode + "]\n\n" + error;

                Console.WriteLine(error);
                throw new Exception(error);
            }

            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);
            GL.GetProgram(shaderProgramHandle, ProgramParameter.LinkStatus, out compileStatusCode);
            if (compileStatusCode != 1)
            {
                string error;

                GL.GetProgramInfoLog(shaderProgramHandle, out error);
                error = "Shader link error [" + compileStatusCode + "]\n\n" + error;

                Console.WriteLine(error);
                throw new Exception(error);
            }

            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);

            GL.DeleteShader(fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);

            GL.UseProgram(shaderProgramHandle);

            vertexAttribLocation = GL.GetAttribLocation(shaderProgramHandle, "aVertex");
            normalAttribLocation = GL.GetAttribLocation(shaderProgramHandle, "aNormal");

            uPVMMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "uPVMMatrix");
            uVertexOffsetLocation = GL.GetUniformLocation(shaderProgramHandle, "uVertexOffset");

            shaderLoaded = true;
            loadedShaders.Add(this);
        }

        public int GetUniformLocation(string name)
        {
            if (uniformCache.ContainsKey(name))
                return uniformCache[name];

            int loc = GL.GetUniformLocation(shaderProgramHandle, name);
            uniformCache.Add(name, loc);
            return loc;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            GL.DeleteProgram(shaderProgramHandle);

            loadedShaders.Remove(this);
            shaderLoaded = false;
            disposed = true;
        }
    }
}
