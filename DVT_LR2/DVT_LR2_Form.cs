using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace DVT_LR2
{
    public partial class DVT_LR2_Form : Form
    {
        private List<int[]> frame_coords;
        private List<double[]> coords;
        private readonly Random row;
        PointF initial_point;
        private Bitmap bmp;


        public DVT_LR2_Form()
        {
            InitializeComponent();

            frame_coords = new List<int[]>();
            coords = new List<double[]>();
            row = new Random();
        }


        private void DVT_LR2_Form_Load(object sender, EventArgs e)
        {
            this.btn_rnd_generate.MouseDown += Button_Fashion_Down;
            this.btn_rnd_generate.MouseUp += Button_Fashion_Up;
            this.btn_rnd_generate.Click += Random_Generate;

            this.btn_func_generate.MouseDown += Button_Fashion_Down;
            this.btn_func_generate.MouseUp += Button_Fashion_Up;
            this.btn_func_generate.Click += Function_Generate;

            this.btn_save.MouseDown += Button_Fashion_Down;
            this.btn_save.MouseUp += Button_Fashion_Up;
            this.btn_save.Click += Save_Points;

            this.btn_load.MouseDown += Button_Fashion_Down;
            this.btn_load.MouseUp += Button_Fashion_Up;
            this.btn_load.Click += Load_Points;

            this.points_view.CellEndEdit += Protection_From_A_Fool;
            this.points_view.KeyUp += Add_Or_Delete_Row;

            this.frame.MouseWheel += PicImage_MouseWheel;
            this.frame.MouseDown += PicImage_MouseDown;
            this.frame.MouseMove += PicImage_MouseMove;
        }


        private void DVT_LR2_Form_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.points_view.ClearSelection();
        }


        private void Random_Generate(object sender, EventArgs e)
        {
            coords.Clear();
            this.points_view.Rows.Clear();

            for (int _ = 0; _ < num_rnd_count.Value; _++)
            {
                var da = new double[] { Math.Round(row.NextDouble(), 2),
                                        Math.Round(row.NextDouble(), 2),
                                        Math.Round(row.NextDouble(), 2) 
                };

                coords.Add(da);

                for (int i = 0; i < 3; i++)
                {
                    if (i == row.Next(0, 3))
                        da.SetValue(da[i] * -1, i);
                }

                this.points_view.Rows.Add(new object[] { string.Join(", ", da) });

                Show_Points();
            }
        }


        private void Function_Generate(object sender, EventArgs e)
        {
            coords.Clear();
            this.points_view.Rows.Clear();

            var N = num_func_count.Value;
            var sigma = 1.1 - (double)this.num_func_deviation.Value;

            var x_arr = new List<double>();
            var y_arr = new List<double>();
            var z_arr = new List<double>();

            if (N == 0)
                return;

            for (int i = 0; i < N; i++)
            {
                x_arr.Add(0.7 * Math.Cos(6 * Math.PI * (i / (double)N)));
                y_arr.Add(0.5 * Math.Sin(4 * Math.PI * (i / (double)N)));
                z_arr.Add(-1 + 2 * (i / (double)N));

                var x_avg = x_arr.Sum() / x_arr.Count;
                var y_avg = y_arr.Sum() / y_arr.Count;
                var z_avg = z_arr.Sum() / z_arr.Count;

                var deviation_x = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((x_arr[i] - x_avg), 2) / (2 * Math.Pow(sigma, 2)));
                var deviation_y = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((y_arr[i] - y_avg), 2) / (2 * Math.Pow(sigma, 2)));
                var deviation_z = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((z_arr[i] - z_avg), 2) / (2 * Math.Pow(sigma, 2)));

                var da = new double[] { Math.Round(x_arr[i] + deviation_x, 2),
                                        Math.Round(y_arr[i] + deviation_y, 2),
                                        Math.Round(z_arr[i] + deviation_z, 2)
                };

                for (int xyz = 0; xyz < 3; xyz++)
                    if (da[xyz] > 1)
                        da[xyz] = 1;
                    else if (da[xyz] < -1)
                        da[xyz] = -1;

                coords.Add(da);

                this.points_view.Rows.Add(new[] { string.Join(", ", da) });
            }

            Show_Points();
        }


        private void Save_Points(object sender, EventArgs e)
        {
            string message = "";
            var backup = coords;
            
            coords.Clear();

            try
            {
                foreach (DataGridViewRow r in this.points_view.Rows)
                    if (r.Cells[0].Value != null)
                        coords.Add(r.Cells[0].Value.ToString().Split(new[] { ", " }, StringSplitOptions.None).Select(Double.Parse).ToArray());

                coords.ForEach(p => message += string.Join(", ", p) + "; ");

                using (StreamWriter writer = new StreamWriter("data.csv"))
                    writer.Write(message);
            }
            catch (FormatException)
            {
                MessageBox.Show("Parsing of a table has failed, check the inputs");
                coords = backup;
            }
        }


        private void Load_Points(object sender, EventArgs e)
        {
            var backup = coords;

            coords.Clear();
            this.points_view.Rows.Clear();

            try
            {
                using (StreamReader reader = new StreamReader("data.csv"))
                {
                    var message = reader.ReadToEnd();

                    var read_lines = message.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in read_lines)
                    {
                        var da = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                        coords.Add(da);
                        this.points_view.Rows.Add(new[] { String.Join(", ", da.Select(v => Math.Round(v, 2))) });
                    }
                }

                Show_Points();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Reader has done an oopsie!" + ex.Message);
                coords = backup;
            }
        }


        private void Protection_From_A_Fool(object sender, DataGridViewCellEventArgs e)
        {
            var backup = coords;

            if (this.points_view.CurrentCell.Value.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Any(xyz => xyz.ToString().Contains(',')))
            {
                MessageBox.Show("An input should not contain commas!");
                coords = backup;

                this.points_view.Rows.Clear();
                coords.ForEach(da => this.points_view.Rows.Add(new[] { string.Join(", ", da.Select(v => Math.Round(v, 2))) }));
            }
        }


        private void Show_Points()
        {
            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            if (frame_coords.Count != 0)
                frame_coords.Clear();

            foreach (var row in coords)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    int x = (int)((row[0] + 1) * frame.Width / 2),
                        y = (int)((row[1] + 1) * frame.Height / 2),
                        z = (int)((row[2] + 1) * 255/2);

                    frame_coords.Add(new[] { x, y, z });
                    
                    g.FillEllipse(new SolidBrush(Color.FromArgb(z, 255, 0, 255)), x, y, 7, 7);
                }
            }

            this.frame.Image = bmp;
        }


        private void PicImage_MouseDown(object sender, MouseEventArgs e)
        {
            initial_point = e.Location;

            initial_point.X = initial_point.X > 0 ? initial_point.X / 1873 : initial_point.X / 46;
            initial_point.Y = initial_point.Y > 0 ? initial_point.Y / 880 : initial_point.Y / 199;
        }


        private void PicImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                this.frame.Image == null)
                return;

            PointF final_point = e.Location;
            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            final_point.X /= Screen.PrimaryScreen.WorkingArea.Width;
            final_point.Y /= Screen.PrimaryScreen.WorkingArea.Height;

            foreach (var row in frame_coords)
            {
                row[0] += (int)(final_point.X - initial_point.X);
                row[1] += (int)(final_point.Y - initial_point.Y);

                using (Graphics g = Graphics.FromImage(bmp))
                    g.FillEllipse(new SolidBrush(Color.FromArgb(row[2], 255, 0, 255)), row[0], row[1], 7, 7);
            }

            initial_point = final_point;

            this.frame.Image = bmp;
        }


        private void PicImage_MouseWheel(object sender, MouseEventArgs e)
        {
            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            if (e.Delta > 0)
            {
                foreach (var row in frame_coords)
                {
                    row[0] = row[0] > e.Location.X ? row[0] + (int)(e.Delta / 12) : row[0] - (int)(e.Delta / 12);
                    row[1] = row[1] > e.Location.X ? row[1] + (int)(e.Delta / 12) : row[1] - (int)(e.Delta / 12);

                    using (Graphics g = Graphics.FromImage(bmp))
                        g.FillEllipse(new SolidBrush(Color.FromArgb(row[2], 255, 0, 255)), row[0], row[1], 7, 7);
                }

                this.frame.InitialImage = bmp;
            }
        }


        private void Button_Fashion_Down(object sender, MouseEventArgs e)
        {
            var btn = (Button)sender;
            btn.BackColor = Color.Fuchsia;
        }

        private void Button_Fashion_Up(object sender, MouseEventArgs e)
        {
            var btn = (Button)sender;
            btn.BackColor = Color.FromArgb(64, 64, 64);
        }


        private void Add_Or_Delete_Row(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !this.points_view.CurrentCell.IsInEditMode)
                this.points_view.Rows.Add(new object[] { });

            if (e.KeyCode == Keys.Delete &&
                this.points_view.Rows.Count != 0)
                this.points_view.Rows.RemoveAt(this.points_view.SelectedCells[0].RowIndex);

            coords.Clear();
            foreach (DataGridViewRow r in this.points_view.Rows)
                if (r.Cells[0].Value != null)
                    coords.Add(r.Cells[0].Value.ToString().Split(new[] { ", " }, StringSplitOptions.None).Select(Double.Parse).ToArray());

            Show_Points();
        }
    }
}
