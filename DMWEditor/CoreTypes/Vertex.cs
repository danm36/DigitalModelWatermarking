using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public class Vertex
    {
        public Vector3 Position;

        public Vertex(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }
}
