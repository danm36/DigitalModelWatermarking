using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor
{
    public partial class Mainform : Form
    {
        class ModelInstantationHelper
        {
            public ModelImporter ModelImporter { get; private set; }
            public ModelInfoAttribute ModelInfo { get; private set; }

            public ModelInstantationHelper(ModelImporter importer, ModelInfoAttribute mia)
            {
                ModelImporter = importer;
                ModelInfo = mia;
            }
        }

        Dictionary<string, ModelInstantationHelper> modelImporters = new Dictionary<string, ModelInstantationHelper>();
        public static Model currentlyLoadedModel = null;

        public Mainform()
        {
            InitializeComponent();

            LoadPluginTypes(Assembly.GetExecutingAssembly());
            foreach (string assemblyFile in Directory.GetFiles("plugins"))
            {
                if (!assemblyFile.EndsWith(".dll"))
                    return;

                LoadPluginTypes(Assembly.LoadFrom(assemblyFile));
            }
        }

        void LoadPluginTypes(Assembly assembly)
        {
            Type modelImporterType = typeof(ModelImporter);
            foreach (Type t in assembly.GetTypes().Where(t => t.IsSubclassOf(modelImporterType)))
            {
                ModelInfoAttribute mia = t.GetCustomAttribute<ModelInfoAttribute>();
                if (mia == null)
                    throw new Exception("Error: ModelImporter " + t.ToString() + " from importer " + assembly.Location + " is missing a ModelInfoAttribute");

                modelImporters.Add(mia.Extension, new ModelInstantationHelper(Activator.CreateInstance(t) as ModelImporter, mia));
            }
        }

        private void loadModelBtn_Click(object sender, EventArgs e)
        {
            StringBuilder filterStr = new StringBuilder();
            string allFilter = "All Supported Formats|";
            foreach (KeyValuePair<string, ModelInstantationHelper> kvp in modelImporters)
            {
                filterStr.Append("|" + kvp.Value.ModelInfo.TypeName + "|*." + kvp.Key);
                allFilter += "*." + kvp.Key + ";";
            }
            allFilter = allFilter.Substring(0, allFilter.Length - 1);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = allFilter + filterStr.ToString();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            ModelInstantationHelper mih = modelImporters[Path.GetExtension(ofd.FileName).Substring(1)];
            Model imported = mih.ModelImporter.DoImportModel(ofd.FileName);

            if (imported == null)
                return;

            if (currentlyLoadedModel != null)
                currentlyLoadedModel.Dispose();

            WaveformID lastWave = currentlyLoadedModel != null ? currentlyLoadedModel.GetWaveID() : null;

            currentlyLoadedModel = imported;
            propertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(currentlyLoadedModel.Metadata);
            dmwglControl.UpdateMatrices();

            WaveformID wave = currentlyLoadedModel.GetWaveID();

            waveformChart.Series[0].Points.Clear();
            waveformChart.Series[1].Points.Clear();
            waveformChart.Series[2].Points.Clear();
            for (int i = 0; i < wave.XWave.Length; ++i)
            {
                waveformChart.Series[0].Points.Add(wave.XWave[i]);
                waveformChart.Series[1].Points.Add(wave.YWave[i]);
                waveformChart.Series[2].Points.Add(wave.ZWave[i]);
            }

            if (lastWave != null)
            {
                double similarity = wave.CompareTo(lastWave);

                string similarityString = "NOT Similar";
                if (similarity < 1.0)
                    similarityString = "Identical";
                else if (similarity < 10.0)
                    similarityString = "Almost identical";
                else if (similarity < 500.0)
                    similarityString = "Very similar";
                else if (similarity < 1500.0)
                    similarityString = "Similar";

                MessageBox.Show("Models are " + similarityString + "\n\nDifference index = " + similarity);
            }
        }

        private void saveModelBtn_Click(object sender, EventArgs e)
        {
            StringBuilder filterStr = new StringBuilder();
            foreach (KeyValuePair<string, ModelInstantationHelper> kvp in modelImporters)
            {
                filterStr.Append("|" + kvp.Value.ModelInfo.TypeName + " (*." + kvp.Key + ")|*." + kvp.Key);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = filterStr.ToString().Substring(1);
            sfd.FileName = currentlyLoadedModel.FilePath;

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            ModelInstantationHelper mih = modelImporters[Path.GetExtension(sfd.FileName).Substring(1)];
            mih.ModelImporter.DoSaveModel(sfd.FileName, currentlyLoadedModel);
        }

        private void embedDataBtn_Click(object sender, EventArgs e)
        {
            if (currentlyLoadedModel == null)
            {
                MessageBox.Show("Please load a model first", "No Model Loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string toEmbed = Prompt.Show("Enter the data to embed");

            if (toEmbed == "")
            {
                MessageBox.Show("Cannot embed nothing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            toEmbed = "DMW" + toEmbed;

            for (int i = 0; i < currentlyLoadedModel.Vertices.Count; ++i)
            {
                byte[] vertexVal = BitConverter.GetBytes(currentlyLoadedModel.Vertices[i].Position.X);
                vertexVal[0] &= unchecked((byte)(~(1 << currentlyLoadedModel.lowestViableBit % 8))); //Clear the bit

                if (i < toEmbed.Length * 8)
                {
                    byte chr = (byte)toEmbed[i / 8];
                    vertexVal[currentlyLoadedModel.lowestViableBit / 8] |= (byte)(((chr >> (i % 8)) & 1) << currentlyLoadedModel.lowestViableBit % 8);
                }

                currentlyLoadedModel.Vertices[i].Position = new Vector3(BitConverter.ToSingle(vertexVal, 0), currentlyLoadedModel.Vertices[i].Position.Y, currentlyLoadedModel.Vertices[i].Position.Z);
            }

            MessageBox.Show("Data embedded into model", "Data Embedded", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void extractEmbeddedDataBtn_Click(object sender, EventArgs e)
        {
            if (currentlyLoadedModel == null)
            {
                MessageBox.Show("Please load a model first", "No Model Loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<byte> bytesRead = new List<byte>();
            byte currentByte = 0;
            for (int bitsRead = 0; bitsRead < currentlyLoadedModel.Vertices.Count; ++bitsRead)
            {
                if (bitsRead != 0 && bitsRead % 8 == 0)
                {
                    bytesRead.Add(currentByte);
                    currentByte = 0;
                }

                byte[] vertexVal = BitConverter.GetBytes(currentlyLoadedModel.Vertices[bitsRead].Position.X);
                byte bit = (byte)(((vertexVal[currentlyLoadedModel.lowestViableBit / 8]) & (1 << currentlyLoadedModel.lowestViableBit % 8)) != 0 ? 1 : 0);
                currentByte |= (byte)(bit << (bitsRead % 8));
            }

            string str = Encoding.ASCII.GetString(bytesRead.ToArray());
            if (str.StartsWith("DMW"))
                MessageBox.Show("Read data: " + str.Substring(3), "Embedded Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No embedded data found", "Embedded Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void checkIfSimilarToBtn_Click(object sender, EventArgs e)
        {
            if (currentlyLoadedModel == null)
            {
                MessageBox.Show("Please load a model first", "Embedded Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            StringBuilder filterStr = new StringBuilder();
            string allFilter = "All Supported Formats|";
            foreach (KeyValuePair<string, ModelInstantationHelper> kvp in modelImporters)
            {
                filterStr.Append("|" + kvp.Value.ModelInfo.TypeName + "|*." + kvp.Key);
                allFilter += "*." + kvp.Key + ";";
            }
            allFilter = allFilter.Substring(0, allFilter.Length - 1);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = allFilter + filterStr.ToString();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            ModelInstantationHelper mih = modelImporters[Path.GetExtension(ofd.FileName).Substring(1)];
            Model imported = mih.ModelImporter.DoImportModel(ofd.FileName);

            if (imported == null)
                return;

            WaveformID wave = currentlyLoadedModel.GetWaveID();
            WaveformID testWave = imported.GetWaveID();

            double similarity = wave.CompareTo(testWave);

            string similarityString = "NOT Similar";
            if (similarity == 0.0)
                similarityString = "Identical";
            else if (similarity < 10.0)
                similarityString = "Almost identical";
            else if (similarity < 500.0)
                similarityString = "Very similar";
            else if(similarity < 1500.0)
                similarityString = "Similar";

            MessageBox.Show("Models are " + similarityString + "\n\nDifference index = " + similarity);
        }
    }
}
