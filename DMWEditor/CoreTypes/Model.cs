using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor
{
    public class Model : IDisposable
    {
        public bool Disposed { get; private set; }
        public string FilePath { get; internal set; }
        public Shader DrawShader { get; internal set; }

        public Dictionary<StringKey, string> Metadata = new Dictionary<StringKey,string>();
        public List<Vertex> Vertices = new List<Vertex>();
        public List<Face> Faces = new List<Face>();
        public AABBox Bounds { get; internal set; }

        public int lowestViableBit = 0;

        int VertexVBO, IndexVBO;

        public Model(string filePath)
        {
            FilePath = filePath;
            VertexVBO = GL.GenBuffer();
            IndexVBO = GL.GenBuffer();

            DrawShader = new Shader("Shaders/vModel.vert", "Shaders/fModel.frag");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            DrawShader.Dispose();
            DrawShader = null;
            Disposed = true;
        }

        public void ReloadData()
        {
            List<int> indexData = new List<int>();
            for (int i = 0; i < Faces.Count; ++i)
            {
                indexData.Add(Faces[i].Vertex1);
                indexData.Add(Faces[i].Vertex3);
                indexData.Add(Faces[i].Vertex2);
                Faces[i].RecalcNormal(this);
            }

            List<float> vertexData = new List<float>();

            Vector3 min = Vector3.Zero, max = Vector3.Zero;

            for (int i = 0; i < Vertices.Count; ++i)
            {
                //Bounds
                if (Vertices[i].Position.X < min.X) min.X = Vertices[i].Position.X;
                if (Vertices[i].Position.Y < min.Y) min.Y = Vertices[i].Position.Y;
                if (Vertices[i].Position.Z < min.Z) min.Z = Vertices[i].Position.Z;

                if (Vertices[i].Position.X > max.X) max.X = Vertices[i].Position.X;
                if (Vertices[i].Position.Y > max.Y) max.Y = Vertices[i].Position.Y;
                if (Vertices[i].Position.Z > max.Z) max.Z = Vertices[i].Position.Z;

                //VBO
                vertexData.Add(Vertices[i].Position.X);
                vertexData.Add(Vertices[i].Position.Y);
                vertexData.Add(Vertices[i].Position.Z);

                List<Face> attachedFaces = new List<Face>();
                for (int f = 0; f < Faces.Count; ++f)
                {
                    if (Faces[f].Vertex1 == i || Faces[f].Vertex2 == i || Faces[f].Vertex3 == i)
                        attachedFaces.Add(Faces[f]);
                }

                Vector3 combinedNormal = Vector3.Zero;
                for (int f = 0; f < attachedFaces.Count; ++f)
                {
                    combinedNormal += attachedFaces[f].Normal;
                }
                combinedNormal.Normalize();

                vertexData.Add(combinedNormal.X);
                vertexData.Add(combinedNormal.Y);
                vertexData.Add(combinedNormal.Z);
            }
            Bounds = new AABBox(min, max);
            

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertexData.Count), vertexData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexVBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indexData.Count), indexData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.UseProgram(DrawShader.shaderProgramHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexVBO);
            GL.EnableVertexAttribArray(DrawShader.vertexAttribLocation);
            GL.VertexAttribPointer(DrawShader.vertexAttribLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(DrawShader.normalAttribLocation);
            GL.VertexAttribPointer(DrawShader.normalAttribLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexVBO);

            GL.Uniform1(DrawShader.GetUniformLocation("isWireframe"), 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawElements(BeginMode.Triangles, Faces.Count * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.Uniform1(DrawShader.GetUniformLocation("isWireframe"), 1);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Triangles, Faces.Count * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public WaveformID GetWaveID()
        {
            Vector3 boundHalfSize = Bounds.LargerSize;
            WaveformID wave = new WaveformID(256);
            Vector3 normVec;

            foreach (Vertex v in Vertices)
            {
                normVec = new Vector3(
                    ((v.Position.X / boundHalfSize.X) / 2 + 0.5f) * (wave.WaveWidth - 1),
                    ((v.Position.Y / boundHalfSize.Y) / 2 + 0.5f) * (wave.WaveWidth - 1),
                    ((v.Position.Z / boundHalfSize.Z) / 2 + 0.5f) * (wave.WaveWidth - 1));

                int xStartIndex = (int)Math.Floor(normVec.X);
                int yStartIndex = (int)Math.Floor(normVec.Y);
                int zStartIndex = (int)Math.Floor(normVec.Z);
                float xStartBias = normVec.X - (float)Math.Floor(normVec.X);
                float yStartBias = normVec.Y - (float)Math.Floor(normVec.Y);
                float zStartBias = normVec.Z - (float)Math.Floor(normVec.Z);

                wave.XWave[xStartIndex] += xStartBias;
                wave.YWave[yStartIndex] += yStartBias;
                wave.ZWave[zStartIndex] += xStartBias;

                wave.XWave[xStartIndex + 1] += (1 - xStartBias);
                wave.YWave[yStartIndex + 1] += (1 - yStartBias);
                wave.ZWave[zStartIndex + 1] += (1 - zStartBias);
            }

            return wave;
        }
    }

    public class AABBox
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Size
        {
            get
            {
                return new Vector3(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z);
            }
        }
        public Vector3 LargerSize
        {
            get
            {
                return new Vector3(
                    Math.Max(Math.Abs(Min.X), Math.Abs(Max.X)),
                    Math.Max(Math.Abs(Min.Y), Math.Abs(Max.Y)),
                    Math.Max(Math.Abs(Min.Z), Math.Abs(Max.Z)));
            }
        }

        public AABBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }

    public class WaveformID
    {
        public int WaveWidth = 256;

        public float[] XWave;
        public float[] YWave;
        public float[] ZWave;

        public WaveformID(int waveWidth)
        {
            WaveWidth = waveWidth;
            XWave = new float[waveWidth + 1];
            YWave = new float[waveWidth + 1];
            ZWave = new float[waveWidth + 1];
        }

        public double CompareTo(WaveformID other)
        {
            double differenceX = 0.0f;
            double differenceY = 0.0f;
            double differenceZ = 0.0f;

            for (int i = 0; i < XWave.Length; ++i)
            {
                differenceX += Math.Pow(Math.Abs(other.XWave[i] - XWave[i]), 2);
                differenceY += Math.Pow(Math.Abs(other.YWave[i] - YWave[i]), 2);
                differenceZ += Math.Pow(Math.Abs(other.ZWave[i] - ZWave[i]), 2);
            }

            double result = (differenceX + differenceY + differenceZ);
            return (double.IsNaN(result) ? 0 : result);
        }

        static float Max(params float[] args)
        {
            float m = 0.0f;

            for (int i = 0; i < args.Length; ++i)
            {
                m = Math.Max(m, args[i]);
            }

            return m;
        }

        static double Max(params double[] args)
        {
            double m = 0.0f;

            for (int i = 0; i < args.Length; ++i)
            {
                m = Math.Max(m, args[i]);
            }

            return m;
        }
    }
}
