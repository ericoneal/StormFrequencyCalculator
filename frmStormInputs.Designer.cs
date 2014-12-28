namespace StormFrequencyCalculator
{
    partial class frmStormInputs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStormInputs));
            this.lblPointLayer = new System.Windows.Forms.Label();
            this.cboPointLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTxtFileInfo = new System.Windows.Forms.Button();
            this.chkUseBatch = new System.Windows.Forms.CheckBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radGauge = new System.Windows.Forms.RadioButton();
            this.radRealTime = new System.Windows.Forms.RadioButton();
            this.radMonthEnd = new System.Windows.Forms.RadioButton();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.cboLayerFields = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPointLayer
            // 
            this.lblPointLayer.AutoSize = true;
            this.lblPointLayer.BackColor = System.Drawing.Color.Transparent;
            this.lblPointLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPointLayer.Location = new System.Drawing.Point(12, 9);
            this.lblPointLayer.Name = "lblPointLayer";
            this.lblPointLayer.Size = new System.Drawing.Size(149, 17);
            this.lblPointLayer.TabIndex = 0;
            this.lblPointLayer.Text = "Input (Point) Layer:";
            // 
            // cboPointLayer
            // 
            this.cboPointLayer.BackColor = System.Drawing.Color.PapayaWhip;
            this.cboPointLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPointLayer.FormattingEnabled = true;
            this.cboPointLayer.Location = new System.Drawing.Point(158, 8);
            this.cboPointLayer.Name = "cboPointLayer";
            this.cboPointLayer.Size = new System.Drawing.Size(272, 24);
            this.cboPointLayer.TabIndex = 1;
            this.cboPointLayer.SelectedIndexChanged += new System.EventHandler(this.cboPointLayer_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btnTxtFileInfo);
            this.groupBox1.Controls.Add(this.chkUseBatch);
            this.groupBox1.Controls.Add(this.btnCalculate);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dtEnd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtStart);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 168);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // btnTxtFileInfo
            // 
            this.btnTxtFileInfo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTxtFileInfo.BackgroundImage")));
            this.btnTxtFileInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnTxtFileInfo.Location = new System.Drawing.Point(211, 121);
            this.btnTxtFileInfo.Name = "btnTxtFileInfo";
            this.btnTxtFileInfo.Size = new System.Drawing.Size(23, 21);
            this.btnTxtFileInfo.TabIndex = 9;
            this.btnTxtFileInfo.UseVisualStyleBackColor = true;
            this.btnTxtFileInfo.Click += new System.EventHandler(this.btnTxtFileInfo_Click);
            // 
            // chkUseBatch
            // 
            this.chkUseBatch.AutoSize = true;
            this.chkUseBatch.Location = new System.Drawing.Point(22, 121);
            this.chkUseBatch.Name = "chkUseBatch";
            this.chkUseBatch.Size = new System.Drawing.Size(183, 17);
            this.chkUseBatch.TabIndex = 8;
            this.chkUseBatch.Text = "Use Text file of Date/Times";
            this.chkUseBatch.UseVisualStyleBackColor = true;
            this.chkUseBatch.CheckedChanged += new System.EventHandler(this.chkUseBatch_CheckedChanged);
            // 
            // btnCalculate
            // 
            this.btnCalculate.BackColor = System.Drawing.SystemColors.Control;
            this.btnCalculate.Location = new System.Drawing.Point(283, 113);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(133, 38);
            this.btnCalculate.TabIndex = 7;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radGauge);
            this.groupBox2.Controls.Add(this.radRealTime);
            this.groupBox2.Controls.Add(this.radMonthEnd);
            this.groupBox2.Location = new System.Drawing.Point(282, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 88);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pixel Data";
            // 
            // radGauge
            // 
            this.radGauge.AutoSize = true;
            this.radGauge.Enabled = false;
            this.radGauge.Location = new System.Drawing.Point(18, 65);
            this.radGauge.Name = "radGauge";
            this.radGauge.Size = new System.Drawing.Size(92, 17);
            this.radGauge.TabIndex = 6;
            this.radGauge.Text = "Rain Guage";
            this.radGauge.UseVisualStyleBackColor = true;
            this.radGauge.CheckedChanged += new System.EventHandler(this.radGauge_CheckedChanged);
            // 
            // radRealTime
            // 
            this.radRealTime.AutoSize = true;
            this.radRealTime.Checked = true;
            this.radRealTime.Location = new System.Drawing.Point(18, 19);
            this.radRealTime.Name = "radRealTime";
            this.radRealTime.Size = new System.Drawing.Size(82, 17);
            this.radRealTime.TabIndex = 4;
            this.radRealTime.TabStop = true;
            this.radRealTime.Text = "Real Time";
            this.radRealTime.UseVisualStyleBackColor = true;
            this.radRealTime.CheckedChanged += new System.EventHandler(this.radRealTime_CheckedChanged);
            // 
            // radMonthEnd
            // 
            this.radMonthEnd.AutoSize = true;
            this.radMonthEnd.Location = new System.Drawing.Point(18, 42);
            this.radMonthEnd.Name = "radMonthEnd";
            this.radMonthEnd.Size = new System.Drawing.Size(115, 17);
            this.radMonthEnd.TabIndex = 5;
            this.radMonthEnd.Text = "Corrected(EOM)";
            this.radMonthEnd.UseVisualStyleBackColor = true;
            this.radMonthEnd.CheckedChanged += new System.EventHandler(this.radMonthEnd_CheckedChanged);
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "MM/dd/yyyy";
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.Location = new System.Drawing.Point(136, 87);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(98, 20);
            this.dtEnd.TabIndex = 3;
            this.dtEnd.Value = new System.DateTime(2006, 9, 23, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Ending Date/Time:";
            // 
            // dtStart
            // 
            this.dtStart.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtStart.CustomFormat = "MM/dd/yyyy";
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.Location = new System.Drawing.Point(136, 42);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(98, 20);
            this.dtStart.TabIndex = 1;
            this.dtStart.Value = new System.DateTime(2006, 9, 22, 0, 0, 0, 0);
            this.dtStart.ValueChanged += new System.EventHandler(this.dtStart_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Starting Date/Time:";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = "\"TXT files (*.txt)|*.txt|All files (*.*)|*.*\";";
            // 
            // cboLayerFields
            // 
            this.cboLayerFields.BackColor = System.Drawing.Color.PapayaWhip;
            this.cboLayerFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLayerFields.FormattingEnabled = true;
            this.cboLayerFields.Location = new System.Drawing.Point(158, 52);
            this.cboLayerFields.Name = "cboLayerFields";
            this.cboLayerFields.Size = new System.Drawing.Size(272, 24);
            this.cboLayerFields.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Label Field:";
            // 
            // frmStormInputs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 272);
            this.Controls.Add(this.cboLayerFields);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cboPointLayer);
            this.Controls.Add(this.lblPointLayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmStormInputs";
            this.Text = "Storm Frequency Calculator";
            this.Load += new System.EventHandler(this.frmStormInputs_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPointLayer;
        private System.Windows.Forms.ComboBox cboPointLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radRealTime;
        private System.Windows.Forms.RadioButton radMonthEnd;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.CheckBox chkUseBatch;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Button btnTxtFileInfo;
        private System.Windows.Forms.RadioButton radGauge;
        private System.Windows.Forms.ComboBox cboLayerFields;
        private System.Windows.Forms.Label label1;

    }
}