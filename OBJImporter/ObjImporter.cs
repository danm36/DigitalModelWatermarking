using DMWEditor;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBJImporter
{
    [ModelInfo("Wavefront OBJ", "obj")]
    public class OBJModelImporter : ModelImporter
    {
        abstract class FileSegment
        {
            public abstract string GetContent(Model model);
        }

        class CommentFileSegment : FileSegment
        {
            public string Content { get; private set; }
            public CommentFileSegment(string s)
            {
                Content = s;
            }

            public override string GetContent(Model model)
            {
                return Content;
            }
        }

        class StringFileSegment : FileSegment
        {
            public string Content { get; private set; }
            public StringFileSegment(string s)
            {
                Content = s;
            }

            public override string GetContent(Model model)
            {
                return Content;
            }
        }

        class MetadataFileSegment : FileSegment
        {
            public override string GetContent(Model model)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<StringKey, string> kvp in model.Metadata)
                {
                    if(kvp.Key.Save)
                        sb.AppendLine("#DMW_" + kvp.Key.KeyText + " " + kvp.Value);
                }
                return sb.ToString().TrimEnd();
            }
        }

        class VertexFileSegment : FileSegment
        {
            public override string GetContent(Model model)
            {
                StringBuilder sb = new StringBuilder();

                foreach (Vertex vert in model.Vertices)
                    sb.AppendLine("v " + DoubleConverter.ToExactString(vert.Position.X) + " " + DoubleConverter.ToExactString(vert.Position.Y) + " " + DoubleConverter.ToExactString(vert.Position.Z));
                return sb.ToString().TrimEnd();
            }
        }

        class FaceFileSegment : FileSegment
        {
            public override string GetContent(Model model)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < model.Faces.Count; ++i)
                    sb.AppendLine("f " + (model.Faces[i].Vertex1 + 1) + " " + (model.Faces[i].Vertex2 + 1) + " " + (model.Faces[i].Vertex3 + 1));
                return sb.ToString().TrimEnd();
            }
        }

        //To reconstruct on model save
        List<FileSegment> fileSegments = new List<FileSegment>();


        public override bool ImportModel(string path, ref Model model)
        {
            string[] lines = File.ReadAllLines(path);

            StringBuilder hashSB = new StringBuilder();

            bool hasAddedMetaSeg = false, hasAddedVertSeg = false, hasAddedFaceSeg = false;
            bool hasForcedTriangulation = false;

            foreach (string line in lines)
            {
                if (line.Length == 0)
                {
                    hashSB.AppendLine();
                    fileSegments.Add(new StringFileSegment(""));
                    continue;
                }

                string[] spl = line.Split(' ');

                if (spl[0][0] == '#')
                {
                    if (spl[0].StartsWith("#DMW_"))
                    {
                        string metaType = spl[0].Substring(5);

                        if (spl.Length > 1)
                            model.Metadata[metaType] = string.Join(" ", spl.Skip(1));
                        else
                            model.Metadata[metaType] = "1";
                    }
                    else
                    {
                        fileSegments.Add(new CommentFileSegment(line));
                    }

                    continue;
                }

                if(!hasAddedMetaSeg)
                {
                    fileSegments.Add(new MetadataFileSegment());
                    hasAddedMetaSeg = true;
                }

                hashSB.AppendLine(line);

                switch (spl[0])
                {
                    case "v":
                        if (!hasAddedVertSeg)
                        {
                            fileSegments.Add(new VertexFileSegment());
                            hasAddedVertSeg = true;
                        }

                        model.Vertices.Add(new Vertex(float.Parse(spl[1]), float.Parse(spl[2]), float.Parse(spl[3])));
                        break;
                    case "f":
                        if (!hasAddedFaceSeg)
                        {
                            fileSegments.Add(new FaceFileSegment());
                            hasAddedFaceSeg = true;
                        }

                        int firstPoint = int.Parse(spl[1]);
                        int lastPoint = int.Parse(spl[2]);
                        int curPoint;

                        if (spl.Length > 4)
                            hasForcedTriangulation = true;

                        for (int j = 3; j < spl.Length; j++)
                        {
                            curPoint = int.Parse(spl[j]);
                            model.Faces.Add(new Face(firstPoint - 1, lastPoint - 1, curPoint - 1));
                            lastPoint = curPoint;
                        }
                        break;
                    default:
                        fileSegments.Add(new StringFileSegment(line));
                        break;
                }
            }

            model.Metadata["ComputedHash"] = Hash.Calculate(Encoding.ASCII.GetBytes(hashSB.ToString()));

            if (hasForcedTriangulation)
            {
                //MessageBox.Show("Notice: Model used non-triangulated surfaces. The OBJ importer does not support these and has automatically triangulated all faces.", "Model Trianglulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            return true;
        }

        public override bool SaveModel(ref string path, ref Model model)
        {
            /*
            //First render to calculate new hash
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fileSegments.Count; ++i)
            {
                if (fileSegments[i] is CommentFileSegment || fileSegments[i] is MetadataFileSegment)
                    continue;

                sb.AppendLine(fileSegments[i].GetContent(model));
            }

            model.Metadata["DataHash"] = Hash.Calculate(Encoding.ASCII.GetBytes(sb.ToString()));

            //The render to save
            sb = new StringBuilder();
            for (int i = 0; i < fileSegments.Count; ++i)
            {
                sb.AppendLine(fileSegments[i].GetContent(model));
            }*/

            //TEMP
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new MetadataFileSegment().GetContent(model));
            sb.AppendLine(new VertexFileSegment().GetContent(model));
            sb.AppendLine(new FaceFileSegment().GetContent(model));

            File.WriteAllText(path, sb.ToString());

            return true;
        }
    }
}
