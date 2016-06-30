namespace DMWEditor
{
    partial class Prompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.questionLabel = new System.Windows.Forms.Label();
            this.questionBox = new System.Windows.Forms.TextBox();
            this.confirmButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // questionLabel
            // 
            this.questionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.questionLabel.Location = new System.Drawing.Point(0, 0);
            this.questionLabel.Name = "questionLabel";
            this.questionLabel.Padding = new System.Windows.Forms.Padding(8);
            this.questionLabel.Size = new System.Drawing.Size(384, 28);
            this.questionLabel.TabIndex = 0;
            this.questionLabel.Text = "Please insert a value";
            this.questionLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // questionBox
            // 
            this.questionBox.Location = new System.Drawing.Point(12, 31);
            this.questionBox.Name = "questionBox";
            this.questionBox.Size = new System.Drawing.Size(360, 20);
            this.questionBox.TabIndex = 1;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(297, 57);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(75, 23);
            this.confirmButton.TabIndex = 2;
            this.confirmButton.Text = "OK";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // Prompt
            // 
            this.AcceptButton = this.confirmButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 90);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.questionBox);
            this.Controls.Add(this.questionLabel);
            this.Name = "Prompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label questionLabel;
        private System.Windows.Forms.TextBox questionBox;
        private System.Windows.Forms.Button confirmButton;
    }
}