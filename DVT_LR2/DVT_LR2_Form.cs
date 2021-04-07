using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVT_LR2
{
    public partial class DVT_LR2_Form : Form
    {
        public DVT_LR2_Form()
        {
            InitializeComponent();

            this.points_view.Rows.Add(new object[] { "600, 600, 600" });
        }


        private void DVT_LR2_Form_Load(object sender, EventArgs e)
        {
            this.btn_rnd_generate.MouseDown += Button_Fashion_Down;
            this.btn_rnd_generate.MouseUp += Button_Fashion_Up;

            this.btn_func_generate.MouseDown += Button_Fashion_Down;
            this.btn_func_generate.MouseUp += Button_Fashion_Up;

            this.btn_save.MouseDown += Button_Fashion_Down;
            this.btn_save.MouseUp += Button_Fashion_Up;

            this.pictureBox1.MouseWheel += picImage_MouseWheel;

            this.points_view.KeyUp += Add_Or_Delete_Row;
        }


        private void DVT_LR2_Form_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.points_view.ClearSelection();
        }


        private void picImage_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                this.num_rnd_count.Value += e.Delta;
            }
            catch
            {

            }
        }


        private void Button_Fashion_Down(object sender, MouseEventArgs e)
        {
            var cock = (Button)sender;
            cock.BackColor = Color.Fuchsia;
        }

        private void Button_Fashion_Up(object sender, MouseEventArgs e)
        {
            var cock = (Button)sender;
            cock.BackColor = Color.FromArgb(64, 64, 64);
        }


        private void Add_Or_Delete_Row(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.points_view.Rows.Add(new object[] { });

            if (e.KeyCode == Keys.Delete &&
                this.points_view.Rows.Count != 0)
                this.points_view.Rows.RemoveAt(this.points_view.SelectedCells[0].RowIndex);
        }
    }
}
