
namespace LRs
{
    partial class Application
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Application));
            this.objects_grid = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frame_box = new System.Windows.Forms.PictureBox();
            this.btn_pp = new System.Windows.Forms.Button();
            this.framerate = new System.Windows.Forms.Timer(this.components);
            this.num_max_points = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.trail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.objects_grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frame_box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_max_points)).BeginInit();
            this.SuspendLayout();
            // 
            // objects_grid
            // 
            this.objects_grid.AllowUserToAddRows = false;
            this.objects_grid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.objects_grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objects_grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Fuchsia;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.objects_grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.objects_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.objects_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.position,
            this.trail});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.objects_grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.objects_grid.Location = new System.Drawing.Point(525, 12);
            this.objects_grid.Name = "objects_grid";
            this.objects_grid.ReadOnly = true;
            this.objects_grid.RowHeadersVisible = false;
            this.objects_grid.Size = new System.Drawing.Size(321, 434);
            this.objects_grid.TabIndex = 0;
            this.objects_grid.SelectionChanged += new System.EventHandler(this.Objects_Grid_SelectionChanged);
            // 
            // id
            // 
            this.id.HeaderText = "obj_id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            // 
            // position
            // 
            this.position.HeaderText = "obj_position";
            this.position.Name = "position";
            this.position.ReadOnly = true;
            this.position.Width = 120;
            // 
            // frame_box
            // 
            this.frame_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frame_box.Location = new System.Drawing.Point(12, 61);
            this.frame_box.Name = "frame_box";
            this.frame_box.Size = new System.Drawing.Size(488, 385);
            this.frame_box.TabIndex = 1;
            this.frame_box.TabStop = false;
            // 
            // btn_pp
            // 
            this.btn_pp.BackColor = System.Drawing.SystemColors.Control;
            this.btn_pp.ForeColor = System.Drawing.Color.White;
            this.btn_pp.Location = new System.Drawing.Point(15, 14);
            this.btn_pp.Name = "btn_pp";
            this.btn_pp.Size = new System.Drawing.Size(98, 31);
            this.btn_pp.TabIndex = 2;
            this.btn_pp.Text = "Pause";
            this.btn_pp.UseVisualStyleBackColor = false;
            this.btn_pp.Click += new System.EventHandler(this.Btn_pp_Click);
            // 
            // framerate
            // 
            this.framerate.Interval = 500;
            this.framerate.Tick += new System.EventHandler(this.Framerate_Tick);
            // 
            // num_max_points
            // 
            this.num_max_points.Location = new System.Drawing.Point(436, 18);
            this.num_max_points.Maximum = new decimal(new int[] {
            666,
            0,
            0,
            0});
            this.num_max_points.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.num_max_points.Name = "num_max_points";
            this.num_max_points.Size = new System.Drawing.Size(62, 23);
            this.num_max_points.TabIndex = 3;
            this.num_max_points.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_max_points.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(313, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Maximum of points";
            // 
            // trail
            // 
            this.trail.HeaderText = "trail";
            this.trail.Name = "trail";
            this.trail.ReadOnly = true;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(856, 466);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.num_max_points);
            this.Controls.Add(this.btn_pp);
            this.Controls.Add(this.frame_box);
            this.Controls.Add(this.objects_grid);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Application";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.Application_Load);
            this.Click += new System.EventHandler(this.Application_Click);
            ((System.ComponentModel.ISupportInitialize)(this.objects_grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frame_box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_max_points)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView objects_grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn position;
        private System.Windows.Forms.PictureBox frame_box;
        private System.Windows.Forms.Button btn_pp;
        private System.Windows.Forms.Timer framerate;
        private System.Windows.Forms.NumericUpDown num_max_points;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn trail;
    }
}

