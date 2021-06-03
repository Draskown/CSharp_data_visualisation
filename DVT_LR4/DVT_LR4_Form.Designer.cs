
namespace DVT_LR4
{
    partial class DVT_LR4_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DVT_LR4_Form));
            this.graphMain = new SharpGL.OpenGLControl();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numDepth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.hideCube = new System.Windows.Forms.CheckBox();
            this.hideXYScat = new System.Windows.Forms.CheckBox();
            this.hideXYHist = new System.Windows.Forms.CheckBox();
            this.hideXZScat = new System.Windows.Forms.CheckBox();
            this.hideXZHist = new System.Windows.Forms.CheckBox();
            this.hideYZScat = new System.Windows.Forms.CheckBox();
            this.hideYZHist = new System.Windows.Forms.CheckBox();
            this.switchDisplay = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelZ = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.graphMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphMain
            // 
            this.graphMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.graphMain.DrawFPS = false;
            this.graphMain.FrameRate = 60;
            this.graphMain.Location = new System.Drawing.Point(12, 12);
            this.graphMain.Name = "graphMain";
            this.graphMain.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL4_4;
            this.graphMain.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.graphMain.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.graphMain.Size = new System.Drawing.Size(500, 500);
            this.graphMain.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(674, 489);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(537, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "X parameter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(664, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Y parameter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(791, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Z parameter";
            // 
            // numLength
            // 
            this.numLength.Location = new System.Drawing.Point(206, 23);
            this.numLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(58, 23);
            this.numLength.TabIndex = 5;
            this.numLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numLength.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Minimum package length =";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(270, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Depth of average calculation = ";
            // 
            // numDepth
            // 
            this.numDepth.Location = new System.Drawing.Point(206, 66);
            this.numDepth.Name = "numDepth";
            this.numDepth.Size = new System.Drawing.Size(58, 23);
            this.numDepth.TabIndex = 5;
            this.numDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numDepth.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(270, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "messages";
            // 
            // hideCube
            // 
            this.hideCube.AutoSize = true;
            this.hideCube.Location = new System.Drawing.Point(89, 97);
            this.hideCube.Name = "hideCube";
            this.hideCube.Size = new System.Drawing.Size(86, 19);
            this.hideCube.TabIndex = 7;
            this.hideCube.Text = "Hide cube";
            this.hideCube.UseVisualStyleBackColor = true;
            // 
            // hideXYScat
            // 
            this.hideXYScat.AutoSize = true;
            this.hideXYScat.Location = new System.Drawing.Point(10, 25);
            this.hideXYScat.Name = "hideXYScat";
            this.hideXYScat.Size = new System.Drawing.Size(118, 19);
            this.hideXYScat.TabIndex = 7;
            this.hideXYScat.Text = "Hide XY scatter";
            this.hideXYScat.UseVisualStyleBackColor = true;
            // 
            // hideXYHist
            // 
            this.hideXYHist.AutoSize = true;
            this.hideXYHist.Location = new System.Drawing.Point(143, 25);
            this.hideXYHist.Name = "hideXYHist";
            this.hideXYHist.Size = new System.Drawing.Size(135, 19);
            this.hideXYHist.TabIndex = 7;
            this.hideXYHist.Text = "Hide XY histogram";
            this.hideXYHist.UseVisualStyleBackColor = true;
            // 
            // hideXZScat
            // 
            this.hideXZScat.AutoSize = true;
            this.hideXZScat.Location = new System.Drawing.Point(10, 48);
            this.hideXZScat.Name = "hideXZScat";
            this.hideXZScat.Size = new System.Drawing.Size(117, 19);
            this.hideXZScat.TabIndex = 7;
            this.hideXZScat.Text = "Hide XZ scatter";
            this.hideXZScat.UseVisualStyleBackColor = true;
            // 
            // hideXZHist
            // 
            this.hideXZHist.AutoSize = true;
            this.hideXZHist.Location = new System.Drawing.Point(143, 48);
            this.hideXZHist.Name = "hideXZHist";
            this.hideXZHist.Size = new System.Drawing.Size(134, 19);
            this.hideXZHist.TabIndex = 7;
            this.hideXZHist.Text = "Hide XZ histogram";
            this.hideXZHist.UseVisualStyleBackColor = true;
            // 
            // hideYZScat
            // 
            this.hideYZScat.AutoSize = true;
            this.hideYZScat.Location = new System.Drawing.Point(10, 72);
            this.hideYZScat.Name = "hideYZScat";
            this.hideYZScat.Size = new System.Drawing.Size(117, 19);
            this.hideYZScat.TabIndex = 7;
            this.hideYZScat.Text = "Hide YZ scatter";
            this.hideYZScat.UseVisualStyleBackColor = true;
            // 
            // hideYZHist
            // 
            this.hideYZHist.AutoSize = true;
            this.hideYZHist.Location = new System.Drawing.Point(143, 72);
            this.hideYZHist.Name = "hideYZHist";
            this.hideYZHist.Size = new System.Drawing.Size(134, 19);
            this.hideYZHist.TabIndex = 7;
            this.hideYZHist.Text = "Hide YZ histogram";
            this.hideYZHist.UseVisualStyleBackColor = true;
            // 
            // switchDisplay
            // 
            this.switchDisplay.AutoSize = true;
            this.switchDisplay.Location = new System.Drawing.Point(647, 420);
            this.switchDisplay.Name = "switchDisplay";
            this.switchDisplay.Size = new System.Drawing.Size(129, 19);
            this.switchDisplay.TabIndex = 8;
            this.switchDisplay.Text = "Display averages";
            this.switchDisplay.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hideCube);
            this.groupBox1.Controls.Add(this.hideXYScat);
            this.groupBox1.Controls.Add(this.hideYZHist);
            this.groupBox1.Controls.Add(this.hideXYHist);
            this.groupBox1.Controls.Add(this.hideYZScat);
            this.groupBox1.Controls.Add(this.hideXZScat);
            this.groupBox1.Controls.Add(this.hideXZHist);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(570, 245);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 124);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numDepth);
            this.groupBox2.Controls.Add(this.numLength);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(538, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 97);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Span settings";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(534, 42);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(88, 15);
            this.labelX.TabIndex = 11;
            this.labelX.Text = "0x00 -> 0x02";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(667, 42);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(80, 15);
            this.labelY.TabIndex = 11;
            this.labelY.Text = "0x01 -> 0x1F";
            // 
            // labelZ
            // 
            this.labelZ.AutoSize = true;
            this.labelZ.Location = new System.Drawing.Point(791, 42);
            this.labelZ.Name = "labelZ";
            this.labelZ.Size = new System.Drawing.Size(83, 15);
            this.labelZ.TabIndex = 11;
            this.labelZ.Text = "0x01 -> 0x39";
            // 
            // DVT_LR4_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 535);
            this.Controls.Add(this.labelZ);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.switchDisplay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.graphMain);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DVT_LR4_Form";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.DVT_LR4_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.graphMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl graphMain;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numDepth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox hideCube;
        private System.Windows.Forms.CheckBox hideXYScat;
        private System.Windows.Forms.CheckBox hideXYHist;
        private System.Windows.Forms.CheckBox hideXZScat;
        private System.Windows.Forms.CheckBox hideXZHist;
        private System.Windows.Forms.CheckBox hideYZScat;
        private System.Windows.Forms.CheckBox hideYZHist;
        private System.Windows.Forms.CheckBox switchDisplay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelZ;
    }
}

