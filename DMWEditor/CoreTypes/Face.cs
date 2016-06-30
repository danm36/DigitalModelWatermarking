using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public class Face
    {
        public int Vertex1 { get; private set; }
        public int Vertex2 { get; private set; }
        public int Vertex3 { get; private set; }
        public Vector3 Normal = Vector3.UnitY;

        public Face(int vert1, int vert2, int vert3)
        {
            Vertex1 = vert1;
            Vertex2 = vert2;
            Vertex3 = vert3;
        }

        public void RecalcNormal(Model model)
        {
            Normal = Vector3.Cross(model.Vertices[Vertex2].Position - model.Vertices[Vertex1].Position, model.Vertices[Vertex3].Position - model.Vertices[Vertex1].Position).Normalized();
        }
    }
}
