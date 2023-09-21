
namespace DVT_LR3
{
    partial class Application
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
            this.scatterPlot = new SharpGL.OpenGLControl();
            this.label1 = new System.Windows.Forms.Label();
            this.pointsAmount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.thinning = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.scatterPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pointsAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thinning)).BeginInit();
            this.SuspendLayout();
            // 
            // scatterPlot
            // 
            this.scatterPlot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scatterPlot.DrawFPS = false;
            this.scatterPlot.Location = new System.Drawing.Point(12, 39);
            this.scatterPlot.Name = "scatterPlot";
            this.scatterPlot.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL4_4;
            this.scatterPlot.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.scatterPlot.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.scatterPlot.Size = new System.Drawing.Size(500, 500);
            this.scatterPlot.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scatter";
            // 
            // pointsAmount
            // 
            this.pointsAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pointsAmount.ForeColor = System.Drawing.Color.White;
            this.pointsAmount.Location = new System.Drawing.Point(538, 176);
            this.pointsAmount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.pointsAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pointsAmount.Name = "pointsAmount";
            this.pointsAmount.Size = new System.Drawing.Size(96, 23);
            this.pointsAmount.TabIndex = 2;
            this.pointsAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pointsAmount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(526, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Amount of points";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(552, 228);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Thinning";
            // 
            // thinning
            // 
            this.thinning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.thinning.ForeColor = System.Drawing.Color.White;
            this.thinning.Location = new System.Drawing.Point(538, 246);
            this.thinning.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.thinning.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thinning.Name = "thinning";
            this.thinning.Size = new System.Drawing.Size(96, 23);
            this.thinning.TabIndex = 2;
            this.thinning.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.thinning.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnStart.Location = new System.Drawing.Point(543, 288);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnLoad.Location = new System.Drawing.Point(528, 332);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(104, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load points";
            this.btnLoad.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSave.Location = new System.Drawing.Point(528, 376);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save image";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(648, 555);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.thinning);
            this.Controls.Add(this.pointsAmount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scatterPlot);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Application";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.Application_Load);
            ((System.ComponentModel.ISupportInitialize)(this.scatterPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pointsAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thinning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl scatterPlot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown pointsAmount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown thinning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

