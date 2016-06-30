namespace DMWEditor
{
    partial class Mainform
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.extractEmbeddedDataBtn = new System.Windows.Forms.Button();
            this.embedDataBtn = new System.Windows.Forms.Button();
            this.saveModelBtn = new System.Windows.Forms.Button();
            this.loadModelBtn = new System.Windows.Forms.Button();
            this.leftPanelSplitter = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.waveformChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dmwglControl = new DMWEditor.CoreTypes.DMWGLControl();
            this.checkIfSimilarToBtn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waveformChart)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.propertyGrid);
            this.leftPanel.Controls.Add(this.panel1);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 24);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(264, 515);
            this.leftPanel.TabIndex = 3;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 207);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(264, 308);
            this.propertyGrid.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.extractEmbeddedDataBtn);
            this.panel1.Controls.Add(this.embedDataBtn);
            this.panel1.Controls.Add(this.checkIfSimilarToBtn);
            this.panel1.Controls.Add(this.saveModelBtn);
            this.panel1.Controls.Add(this.loadModelBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 207);
            this.panel1.TabIndex = 3;
            // 
            // extractEmbeddedDataBtn
            // 
            this.extractEmbeddedDataBtn.Location = new System.Drawing.Point(134, 61);
            this.extractEmbeddedDataBtn.Name = "extractEmbeddedDataBtn";
            this.extractEmbeddedDataBtn.Size = new System.Drawing.Size(124, 23);
            this.extractEmbeddedDataBtn.TabIndex = 0;
            this.extractEmbeddedDataBtn.Text = "Extract Emb Data";
            this.extractEmbeddedDataBtn.UseVisualStyleBackColor = true;
            this.extractEmbeddedDataBtn.Click += new System.EventHandler(this.extractEmbeddedDataBtn_Click);
            // 
            // embedDataBtn
            // 
            this.embedDataBtn.Location = new System.Drawing.Point(3, 61);
            this.embedDataBtn.Name = "embedDataBtn";
            this.embedDataBtn.Size = new System.Drawing.Size(125, 23);
            this.embedDataBtn.TabIndex = 0;
            this.embedDataBtn.Text = "Embed Data";
            this.embedDataBtn.UseVisualStyleBackColor = true;
            this.embedDataBtn.Click += new System.EventHandler(this.embedDataBtn_Click);
            // 
            // saveModelBtn
            // 
            this.saveModelBtn.Location = new System.Drawing.Point(3, 32);
            this.saveModelBtn.Name = "saveModelBtn";
            this.saveModelBtn.Size = new System.Drawing.Size(255, 23);
            this.saveModelBtn.TabIndex = 0;
            this.saveModelBtn.Text = "Save Model...";
            this.saveModelBtn.UseVisualStyleBackColor = true;
            this.saveModelBtn.Click += new System.EventHandler(this.saveModelBtn_Click);
            // 
            // loadModelBtn
            // 
            this.loadModelBtn.Location = new System.Drawing.Point(3, 3);
            this.loadModelBtn.Name = "loadModelBtn";
            this.loadModelBtn.Size = new System.Drawing.Size(255, 23);
            this.loadModelBtn.TabIndex = 0;
            this.loadModelBtn.Text = "Load Model...";
            this.loadModelBtn.UseVisualStyleBackColor = true;
            this.loadModelBtn.Click += new System.EventHandler(this.loadModelBtn_Click);
            // 
            // leftPanelSplitter
            // 
            this.leftPanelSplitter.Location = new System.Drawing.Point(264, 24);
            this.leftPanelSplitter.Name = "leftPanelSplitter";
            this.leftPanelSplitter.Size = new System.Drawing.Size(3, 515);
            this.leftPanelSplitter.TabIndex = 4;
            this.leftPanelSplitter.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.waveformChart);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(267, 390);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(517, 149);
            this.panel2.TabIndex = 6;
            // 
            // waveformChart
            // 
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.waveformChart.ChartAreas.Add(chartArea1);
            this.waveformChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.waveformChart.Legends.Add(legend1);
            this.waveformChart.Location = new System.Drawing.Point(0, 0);
            this.waveformChart.Name = "waveformChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Red;
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "xWave";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Lime;
            series2.IsXValueIndexed = true;
            series2.Legend = "Legend1";
            series2.Name = "yWave";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Single;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Color = System.Drawing.Color.Blue;
            series3.IsXValueIndexed = true;
            series3.Legend = "Legend1";
            series3.Name = "zWave";
            this.waveformChart.Series.Add(series1);
            this.waveformChart.Series.Add(series2);
            this.waveformChart.Series.Add(series3);
            this.waveformChart.Size = new System.Drawing.Size(517, 149);
            this.waveformChart.TabIndex = 0;
            this.waveformChart.Text = "chart1";
            // 
            // dmwglControl
            // 
            this.dmwglControl.BackColor = System.Drawing.Color.Black;
            this.dmwglControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dmwglControl.Location = new System.Drawing.Point(267, 24);
            this.dmwglControl.Name = "dmwglControl";
            this.dmwglControl.Size = new System.Drawing.Size(517, 366);
            this.dmwglControl.TabIndex = 5;
            this.dmwglControl.VSync = false;
            // 
            // checkIfSimilarToBtn
            // 
            this.checkIfSimilarToBtn.Location = new System.Drawing.Point(3, 178);
            this.checkIfSimilarToBtn.Name = "checkIfSimilarToBtn";
            this.checkIfSimilarToBtn.Size = new System.Drawing.Size(255, 23);
            this.checkIfSimilarToBtn.TabIndex = 0;
            this.checkIfSimilarToBtn.Text = "Check if Similar To...";
            this.checkIfSimilarToBtn.UseVisualStyleBackColor = true;
            this.checkIfSimilarToBtn.Click += new System.EventHandler(this.checkIfSimilarToBtn_Click);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.dmwglControl);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.leftPanelSplitter);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Mainform";
            this.Text = "Digital Model Watermarking Suite";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.waveformChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button extractEmbeddedDataBtn;
        private System.Windows.Forms.Button embedDataBtn;
        private System.Windows.Forms.Button saveModelBtn;
        private System.Windows.Forms.Button loadModelBtn;
        private System.Windows.Forms.Splitter leftPanelSplitter;
        private CoreTypes.DMWGLControl dmwglControl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataVisualization.Charting.Chart waveformChart;
        private System.Windows.Forms.Button checkIfSimilarToBtn;
    }
}

