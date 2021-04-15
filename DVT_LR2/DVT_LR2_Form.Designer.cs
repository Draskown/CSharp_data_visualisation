
namespace DVT_LR2
{
    partial class DVT_LR2_Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DVT_LR2_Form));
            this.frame = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_rnd_generate = new System.Windows.Forms.Button();
            this.num_rnd_count = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_func_generate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.num_func_count = new System.Windows.Forms.NumericUpDown();
            this.num_func_deviation = new System.Windows.Forms.NumericUpDown();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_load = new System.Windows.Forms.Button();
            this.rotationY_grid = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.frame)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_rnd_count)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_func_count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_func_deviation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationY_grid)).BeginInit();
            this.SuspendLayout();
            // 
            // frame
            // 
            this.frame.Location = new System.Drawing.Point(12, 12);
            this.frame.Name = "frame";
            this.frame.Size = new System.Drawing.Size(600, 600);
            this.frame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.frame.TabIndex = 0;
            this.frame.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_rnd_generate);
            this.groupBox1.Controls.Add(this.num_rnd_count);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(657, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(114, 88);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Random";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "N:";
            // 
            // btn_rnd_generate
            // 
            this.btn_rnd_generate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_rnd_generate.ForeColor = System.Drawing.Color.White;
            this.btn_rnd_generate.Location = new System.Drawing.Point(6, 48);
            this.btn_rnd_generate.Name = "btn_rnd_generate";
            this.btn_rnd_generate.Size = new System.Drawing.Size(102, 30);
            this.btn_rnd_generate.TabIndex = 3;
            this.btn_rnd_generate.Text = "Generate";
            this.btn_rnd_generate.UseVisualStyleBackColor = false;
            // 
            // num_rnd_count
            // 
            this.num_rnd_count.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.num_rnd_count.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.num_rnd_count.ForeColor = System.Drawing.Color.White;
            this.num_rnd_count.Location = new System.Drawing.Point(39, 19);
            this.num_rnd_count.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_rnd_count.Name = "num_rnd_count";
            this.num_rnd_count.Size = new System.Drawing.Size(69, 23);
            this.num_rnd_count.TabIndex = 3;
            this.num_rnd_count.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_rnd_count.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btn_func_generate);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.num_func_count);
            this.groupBox2.Controls.Add(this.num_func_deviation);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(638, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 114);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Function";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Deviation:";
            // 
            // btn_func_generate
            // 
            this.btn_func_generate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_func_generate.ForeColor = System.Drawing.Color.White;
            this.btn_func_generate.Location = new System.Drawing.Point(6, 76);
            this.btn_func_generate.Name = "btn_func_generate";
            this.btn_func_generate.Size = new System.Drawing.Size(140, 30);
            this.btn_func_generate.TabIndex = 3;
            this.btn_func_generate.Text = "Generate";
            this.btn_func_generate.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "N:";
            // 
            // num_func_count
            // 
            this.num_func_count.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.num_func_count.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.num_func_count.ForeColor = System.Drawing.Color.White;
            this.num_func_count.Location = new System.Drawing.Point(77, 17);
            this.num_func_count.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_func_count.Name = "num_func_count";
            this.num_func_count.Size = new System.Drawing.Size(69, 23);
            this.num_func_count.TabIndex = 3;
            this.num_func_count.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_func_count.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // num_func_deviation
            // 
            this.num_func_deviation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.num_func_deviation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.num_func_deviation.DecimalPlaces = 2;
            this.num_func_deviation.ForeColor = System.Drawing.Color.White;
            this.num_func_deviation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_func_deviation.Location = new System.Drawing.Point(77, 46);
            this.num_func_deviation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_func_deviation.Name = "num_func_deviation";
            this.num_func_deviation.Size = new System.Drawing.Size(69, 23);
            this.num_func_deviation.TabIndex = 3;
            this.num_func_deviation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_func_deviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // btn_save
            // 
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.ForeColor = System.Drawing.Color.White;
            this.btn_save.Location = new System.Drawing.Point(636, 539);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(65, 30);
            this.btn_save.TabIndex = 3;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = false;
            // 
            // btn_load
            // 
            this.btn_load.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_load.ForeColor = System.Drawing.Color.White;
            this.btn_load.Location = new System.Drawing.Point(725, 539);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(65, 30);
            this.btn_load.TabIndex = 3;
            this.btn_load.Text = "Load";
            this.btn_load.UseVisualStyleBackColor = false;
            // 
            // rotationY_grid
            // 
            this.rotationY_grid.AllowUserToAddRows = false;
            this.rotationY_grid.AllowUserToDeleteRows = false;
            this.rotationY_grid.AllowUserToResizeColumns = false;
            this.rotationY_grid.AllowUserToResizeRows = false;
            this.rotationY_grid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotationY_grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Fuchsia;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.rotationY_grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.rotationY_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rotationY_grid.ColumnHeadersVisible = false;
            this.rotationY_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn1,
            this.Column2});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Fuchsia;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.rotationY_grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.rotationY_grid.EnableHeadersVisualStyles = false;
            this.rotationY_grid.GridColor = System.Drawing.Color.White;
            this.rotationY_grid.Location = new System.Drawing.Point(638, 330);
            this.rotationY_grid.MultiSelect = false;
            this.rotationY_grid.Name = "rotationY_grid";
            this.rotationY_grid.RowHeadersVisible = false;
            this.rotationY_grid.RowTemplate.Height = 40;
            this.rotationY_grid.Size = new System.Drawing.Size(154, 160);
            this.rotationY_grid.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(634, 306);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(162, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Matrix Rotation Multipliers";
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.Width = 50;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Column0";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.Width = 50;
            // 
            // DVT_LR2_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(820, 623);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_load);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rotationY_grid);
            this.Controls.Add(this.frame);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DVT_LR2_Form";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.DVT_LR2_Form_Load);
            this.Click += new System.EventHandler(this.DVT_LR2_Form_Click);
            ((System.ComponentModel.ISupportInitialize)(this.frame)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_rnd_count)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_func_count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_func_deviation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationY_grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox frame;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_rnd_generate;
        private System.Windows.Forms.NumericUpDown num_rnd_count;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown num_func_count;
        private System.Windows.Forms.Button btn_func_generate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_func_deviation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.DataGridView rotationY_grid;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}

