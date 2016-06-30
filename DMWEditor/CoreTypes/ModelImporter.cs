using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor
{
    public abstract class ModelImporter
    {
        public abstract bool ImportModel(string path, ref Model model);
        public abstract bool SaveModel(ref string path, ref Model model);

        internal Model DoImportModel(string path)
        {
            Model model = new Model(path);

            model.Metadata.Add(new StringKey("Name", "Metadata", "The name of this model", true, true, true), "");
            model.Metadata.Add(new StringKey("Author", "Metadata", "The author of this model", true, true, true), "");
            model.Metadata.Add(new StringKey("Copyright", "Metadata", "The copyright information for this model", true, true, true), "");
            model.Metadata.Add(new StringKey("LastEdited", "Metadata", "The last edited time and date for this model", false, true, true), "");

            model.Metadata.Add(new StringKey("ComputedHash", "Internal", "The hash computed from the model data", false, true, false), "");
            model.Metadata.Add(new StringKey("DataHash", "Internal", "The hash already stored within the file", false, false, true), "");

            if (!ImportModel(path, ref model))
            {
                MessageBox.Show("Model could not be loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (model.Metadata["DataHash"] != "" && model.Metadata["ComputedHash"] != model.Metadata["DataHash"])
            {
                MessageBox.Show("Warning: This model has been modified in some way from its source!", "Invalid Data Hash", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            model.ReloadData();
            return model;
        }

        internal void DoSaveModel(string path, Model model)
        {
            if (!SaveModel(ref path, ref model))
            {
                MessageBox.Show("Model could not be saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            model.FilePath = path;
        }
    }

    public class ModelInfoAttribute : Attribute
    {
        public string TypeName { get; private set; }
        public string Extension { get; private set; }

        public ModelInfoAttribute(string name, string extension)
        {
            TypeName = name;
            Extension = extension;
        }
    }
}
