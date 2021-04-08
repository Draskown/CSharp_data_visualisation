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
    public partial class Form1 : Form
    {
        Point sp;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridView1.MouseDown += (ss, ee) => {
                sp = ee.Location;
            };

            this.dataGridView1.MouseMove += (ss, ee) => {
                if (ee.Button != MouseButtons.Left)
                    return;

                this.dataGridView1.Left += ee.X - sp.X;
                this.dataGridView1.Top += ee.Y - sp.Y;
            };
        }
    }
}
