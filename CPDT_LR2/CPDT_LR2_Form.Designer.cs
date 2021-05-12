
namespace CPDT_LR2
{
    partial class CPDT_LR2_Form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CPDT_LR2_Form));
            this.frameIsometric = new System.Windows.Forms.PictureBox();
            this.frameOverhead = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPp = new System.Windows.Forms.Button();
            this.boxLog = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numFOVHor = new System.Windows.Forms.NumericUpDown();
            this.numFOVVer = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numFramerate = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numXYPlane = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numXZPlane = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numYZPlane = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numAllowedPoints = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numCorWidth = new System.Windows.Forms.NumericUpDown();
            this.numCorHeight = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.numCorAngle = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.frameRate = new System.Windows.Forms.Timer(this.components);
            this.numObjectRadius = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.frameIsometric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameOverhead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFOVHor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFOVVer)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFramerate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXYPlane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXZPlane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYZPlane)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllowedPoints)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCorWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCorHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCorAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numObjectRadius)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // frameIsometric
            // 
            this.frameIsometric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frameIsometric.Location = new System.Drawing.Point(14, 93);
            this.frameIsometric.Name = "frameIsometric";
            this.frameIsometric.Size = new System.Drawing.Size(500, 500);
            this.frameIsometric.TabIndex = 0;
            this.frameIsometric.TabStop = false;
            // 
            // frameOverhead
            // 
            this.frameOverhead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frameOverhead.Location = new System.Drawing.Point(533, 93);
            this.frameOverhead.Name = "frameOverhead";
            this.frameOverhead.Size = new System.Drawing.Size(300, 300);
            this.frameOverhead.TabIndex = 0;
            this.frameOverhead.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(217, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Isometric View";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(634, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Overhead View";
            // 
            // btnPp
            // 
            this.btnPp.Location = new System.Drawing.Point(14, 29);
            this.btnPp.Name = "btnPp";
            this.btnPp.Size = new System.Drawing.Size(75, 23);
            this.btnPp.TabIndex = 2;
            this.btnPp.Text = "Start";
            this.btnPp.UseVisualStyleBackColor = true;
            // 
            // boxLog
            // 
            this.boxLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boxLog.ForeColor = System.Drawing.Color.White;
            this.boxLog.Location = new System.Drawing.Point(852, 93);
            this.boxLog.Multiline = true;
            this.boxLog.Name = "boxLog";
            this.boxLog.ReadOnly = true;
            this.boxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.boxLog.Size = new System.Drawing.Size(219, 500);
            this.boxLog.TabIndex = 3;
            this.boxLog.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.boxLog.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(936, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Log";
            // 
            // numFOVHor
            // 
            this.numFOVHor.Location = new System.Drawing.Point(79, 24);
            this.numFOVHor.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numFOVHor.Name = "numFOVHor";
            this.numFOVHor.Size = new System.Drawing.Size(63, 23);
            this.numFOVHor.TabIndex = 4;
            this.numFOVHor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFOVHor.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            // 
            // numFOVVer
            // 
            this.numFOVVer.Location = new System.Drawing.Point(79, 53);
            this.numFOVVer.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numFOVVer.Name = "numFOVVer";
            this.numFOVVer.Size = new System.Drawing.Size(63, 23);
            this.numFOVVer.TabIndex = 4;
            this.numFOVVer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFOVVer.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Horizontal";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Vertical";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.numFOVVer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numFOVHor);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(533, 420);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(156, 92);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Field of View";
            // 
            // numFramerate
            // 
            this.numFramerate.Location = new System.Drawing.Point(117, 29);
            this.numFramerate.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numFramerate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numFramerate.Name = "numFramerate";
            this.numFramerate.Size = new System.Drawing.Size(63, 23);
            this.numFramerate.TabIndex = 4;
            this.numFramerate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFramerate.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(111, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "FrameRate";
            // 
            // numXYPlane
            // 
            this.numXYPlane.DecimalPlaces = 2;
            this.numXYPlane.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numXYPlane.Location = new System.Drawing.Point(33, 23);
            this.numXYPlane.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numXYPlane.Name = "numXYPlane";
            this.numXYPlane.Size = new System.Drawing.Size(63, 23);
            this.numXYPlane.TabIndex = 4;
            this.numXYPlane.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numXYPlane.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 15);
            this.label7.TabIndex = 1;
            this.label7.Text = "XY";
            // 
            // numXZPlane
            // 
            this.numXZPlane.DecimalPlaces = 2;
            this.numXZPlane.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numXZPlane.Location = new System.Drawing.Point(131, 23);
            this.numXZPlane.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numXZPlane.Name = "numXZPlane";
            this.numXZPlane.Size = new System.Drawing.Size(63, 23);
            this.numXZPlane.TabIndex = 4;
            this.numXZPlane.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numXZPlane.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(108, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 15);
            this.label8.TabIndex = 1;
            this.label8.Text = "XZ";
            // 
            // numYZPlane
            // 
            this.numYZPlane.DecimalPlaces = 2;
            this.numYZPlane.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numYZPlane.Location = new System.Drawing.Point(228, 23);
            this.numYZPlane.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numYZPlane.Name = "numYZPlane";
            this.numYZPlane.Size = new System.Drawing.Size(63, 23);
            this.numYZPlane.TabIndex = 4;
            this.numYZPlane.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numYZPlane.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(205, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(22, 15);
            this.label9.TabIndex = 1;
            this.label9.Text = "YZ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numYZPlane);
            this.groupBox2.Controls.Add(this.numXYPlane);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numXZPlane);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(533, 533);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 60);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Planes";
            // 
            // numAllowedPoints
            // 
            this.numAllowedPoints.Location = new System.Drawing.Point(279, 22);
            this.numAllowedPoints.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numAllowedPoints.Name = "numAllowedPoints";
            this.numAllowedPoints.Size = new System.Drawing.Size(63, 23);
            this.numAllowedPoints.TabIndex = 4;
            this.numAllowedPoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numAllowedPoints.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(165, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(108, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "Amount of points";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.numCorWidth);
            this.groupBox3.Controls.Add(this.numCorHeight);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.numCorAngle);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(699, 408);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(134, 114);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "The corridor";
            // 
            // numCorWidth
            // 
            this.numCorWidth.Location = new System.Drawing.Point(61, 81);
            this.numCorWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCorWidth.Name = "numCorWidth";
            this.numCorWidth.Size = new System.Drawing.Size(63, 23);
            this.numCorWidth.TabIndex = 4;
            this.numCorWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numCorWidth.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // numCorHeight
            // 
            this.numCorHeight.Location = new System.Drawing.Point(61, 52);
            this.numCorHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCorHeight.Name = "numCorHeight";
            this.numCorHeight.Size = new System.Drawing.Size(63, 23);
            this.numCorHeight.TabIndex = 4;
            this.numCorHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numCorHeight.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 15);
            this.label11.TabIndex = 1;
            this.label11.Text = "Angle";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 85);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 15);
            this.label13.TabIndex = 1;
            this.label13.Text = "Width";
            // 
            // numCorAngle
            // 
            this.numCorAngle.Location = new System.Drawing.Point(61, 23);
            this.numCorAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numCorAngle.Name = "numCorAngle";
            this.numCorAngle.Size = new System.Drawing.Size(63, 23);
            this.numCorAngle.TabIndex = 4;
            this.numCorAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numCorAngle.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 15);
            this.label12.TabIndex = 1;
            this.label12.Text = "Height";
            // 
            // numObjectRadius
            // 
            this.numObjectRadius.DecimalPlaces = 2;
            this.numObjectRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numObjectRadius.Location = new System.Drawing.Point(87, 22);
            this.numObjectRadius.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numObjectRadius.Name = "numObjectRadius";
            this.numObjectRadius.Size = new System.Drawing.Size(63, 23);
            this.numObjectRadius.TabIndex = 4;
            this.numObjectRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numObjectRadius.Value = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(34, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 15);
            this.label14.TabIndex = 1;
            this.label14.Text = "Radius";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numAllowedPoints);
            this.groupBox4.Controls.Add(this.numObjectRadius);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(208, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(352, 56);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Object parameters";
            // 
            // CPDT_LR2_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 608);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.boxLog);
            this.Controls.Add(this.numFramerate);
            this.Controls.Add(this.btnPp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.frameOverhead);
            this.Controls.Add(this.frameIsometric);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CPDT_LR2_Form";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.CPDT_LR2_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.frameIsometric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameOverhead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFOVHor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFOVVer)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFramerate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXYPlane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXZPlane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYZPlane)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllowedPoints)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCorWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCorHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCorAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numObjectRadius)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPp;
        private System.Windows.Forms.TextBox boxLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numFOVHor;
        private System.Windows.Forms.NumericUpDown numFOVVer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numFramerate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numXYPlane;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numXZPlane;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numYZPlane;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numAllowedPoints;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numCorWidth;
        private System.Windows.Forms.NumericUpDown numCorHeight;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numCorAngle;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Timer frameRate;
        public System.Windows.Forms.PictureBox frameIsometric;
        public System.Windows.Forms.PictureBox frameOverhead;
        private System.Windows.Forms.NumericUpDown numObjectRadius;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}