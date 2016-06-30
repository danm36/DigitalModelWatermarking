using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor
{
    public partial class Prompt : Form
    {
        private Prompt()
        {
            InitializeComponent();
        }

        public static string Show(string question, string initalValue = "")
        {
            Prompt p = new Prompt();
            p.questionLabel.Text = question;
            p.questionBox.Text = initalValue;
            p.ShowDialog();
            return p.questionBox.Text;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
