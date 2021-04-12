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
        private List<PVector> coords, movable_coords;
        private readonly Random r;
        float distance;
        PointF initial_point;
        PointF delta;

        private Bitmap bmp;
        int size;


        public DVT_LR2_Form()
        {
            InitializeComponent();

            movable_coords = new List<PVector>();
            coords = new List<PVector>();
            delta = new Point(300, 300);
            r = new Random();
            distance = 4;
            size = 7;
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

            this.points_view.CellEndEdit += Edit_Row;
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
            movable_coords.Clear();
            this.points_view.Rows.Clear();
            delta = new Point(300, 300);

            for (int _ = 0; _ < num_rnd_count.Value; _++)
            {
                var v = new PVector(Math.Round(r.NextDouble(), 2),
                                     Math.Round(r.NextDouble(), 2),
                                     Math.Round(r.NextDouble(), 2));

                for (int i = 0; i < 3; i++)
                {
                    switch (r.Next(0, 3))
                    {
                        case 0:
                            v.x *= -1;
                            break;
                        case 1:
                            v.y *= -1;
                            break;
                        case 2:
                            v.z *= -1;
                            break;
                    }
                }

                coords.Add(v);

                this.points_view.Rows.Add(new object[] { $"{v.x}, {v.y}, {v.z}" });

                To_Pixels(v);
            }

            Show_Points();
        }


        private void Function_Generate(object sender, EventArgs e)
        {
            coords.Clear();
            movable_coords.Clear();
            this.points_view.Rows.Clear();
            delta = new Point(300, 300);

            var N = num_func_count.Value;
            var sigma = 1.1 - (double)this.num_func_deviation.Value;

            var x_arr = new List<double>();
            var y_arr = new List<double>();
            var z_arr = new List<double>();

            if (N == 0)
                return;

            for (int i = 0; i < N; i++)
            {
                x_arr.Add((0.7 * Math.Cos(6 * Math.PI * (i / (float)N))));
                y_arr.Add((0.5 * Math.Sin(4 * Math.PI * (i / (float)N))));
                z_arr.Add(-1 + 2 * (i / (float)N));

                var x_avg = x_arr.Sum() / x_arr.Count;
                var y_avg = y_arr.Sum() / y_arr.Count;
                var z_avg = z_arr.Sum() / z_arr.Count;

                var deviation_x = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((x_arr[i] - x_avg), 2) / (2 * Math.Pow(sigma, 2)));
                var deviation_y = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((y_arr[i] - y_avg), 2) / (2 * Math.Pow(sigma, 2)));
                var deviation_z = 1 / (Math.Sqrt(2 * Math.PI) * sigma) *
                                  Math.Exp(-Math.Pow((z_arr[i] - z_avg), 2) / (2 * Math.Pow(sigma, 2)));

                var v = new PVector(Math.Round(x_arr[i] + deviation_x, 2),
                                    Math.Round(y_arr[i] + deviation_y, 2),
                                    Math.Round(z_arr[i] + deviation_z, 2));

                v.x = v.x > 1 ? 1 : v.x < -1 ? -1 : v.x;
                v.y = v.y > 1 ? 1 : v.y < -1 ? -1 : v.y;
                v.z = v.z > 1 ? 1 : v.z < -1 ? -1 : v.z;

                coords.Add(v);

                this.points_view.Rows.Add(new[] { $"{v.x}, {v.y}, {v.z}" });

                To_Pixels(v);
            }

            Show_Points();
        }


        private void Save_Points(object sender, EventArgs e)
        {
            string message = "";

            try
            {
                coords.ForEach(v => message += $"{v.x}, {v.y}, {v.z}; ");

                using (StreamWriter writer = new StreamWriter("data.csv"))
                    writer.Write(message);
            }
            catch (FormatException)
            {
                MessageBox.Show("Parsing of a table has failed, check the inputs");
            }
        }


        private void Load_Points(object sender, EventArgs e)
        {
            var backup = coords;

            coords.Clear();
            movable_coords.Clear();
            this.points_view.Rows.Clear();
            delta = new Point(300, 300);

            try
            {
                using (StreamReader reader = new StreamReader("data.csv"))
                {
                    var message = reader.ReadToEnd();

                    var read_lines = message.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in read_lines)
                    {
                        var arr = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                        var v = new PVector(arr[0], arr[1], arr[2]); v.Round();

                        coords.Add(v);

                        this.points_view.Rows.Add(new[] { $"{v.x}, {v.y}, {v.z}" });

                        To_Pixels(v);
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


        private void Edit_Row(object sender, DataGridViewCellEventArgs e)
        {
            if (this.points_view.CurrentCell.Value == null)
                return;

            var backup = coords;

            if (this.points_view.CurrentCell.Value.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Any(xyz => xyz.ToString().Contains(',')))
            {
                MessageBox.Show("An input should not contain commas!");
                coords = backup;

                this.points_view.Rows.Clear();
                coords.ForEach(c => {
                    c.Round();
                    this.points_view.Rows.Add(new[] { $"{c.x}, {c.y}, {c.z}" });
                });

                return;
            }

            var arr = this.points_view.CurrentCell.Value.ToString().Split(new[] { ", " }, StringSplitOptions.None).Select(Double.Parse).ToArray();
            var v = new PVector(arr[0], arr[1], arr[2]);

            int index = this.points_view.CurrentCell.RowIndex;

            coords.RemoveAt(index);
            coords.Insert(index, v);

            movable_coords.RemoveAt(index);

            double z = 1 / (distance - v.z);

            double[][] projection =
            {
                        new[] { z, 0, 0 },
                        new[] { 0, z, 0 },
                    };

            PVector pd = MatMul(projection, v);

            pd.x = pd.x * 700 + this.frame.Width / 2;
            pd.y = pd.y * 700 + this.frame.Height / 2;
            pd.z = ((v.z + 1) * 255 / 2);
            pd.z = pd.z > 255 ? 255 : pd.z < 50 ? 50 : pd.z;

            pd.x += delta.X - 300;
            pd.y += delta.Y - 300;

            movable_coords.Insert(index, pd);

            Show_Points();
        }


        private void Show_Points()
        {
            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var v in movable_coords)
                    g.FillEllipse(new SolidBrush(Color.FromArgb((int)v.z, 255, 0, 255)), (float)v.x, (float)v.y, size, size);
            }

            this.frame.Image = bmp;
        }


        private void PicImage_MouseDown(object sender, MouseEventArgs e)
        {
            initial_point = e.Location;
        }


        private void PicImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                this.frame.Image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                PointF final_point = e.Location;
                bmp = new Bitmap(this.frame.Width, this.frame.Height);

                if (Control.ModifierKeys != Keys.Shift)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        foreach (var v in movable_coords)
                        {
                            v.x += (final_point.X - initial_point.X);
                            v.y += (final_point.Y - initial_point.Y);

                            Console.WriteLine($"{delta.X}, {delta.Y}");

                            g.FillEllipse(new SolidBrush(Color.FromArgb((int)v.z, 255, 0, 255)), (float)v.x, (float)v.y, size, size);
                        }
                    }

                    delta.X += (final_point.X - initial_point.X);
                    delta.Y += (final_point.Y - initial_point.Y);
                }
                else
                {

                }

                initial_point = final_point;

                this.frame.Image = bmp;
            }
        }


        private void PicImage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.frame.Image == null)
                return;

            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            // TODO: increment and decrement of the distance parameter
        }


        private void Add_Or_Delete_Row(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !this.points_view.CurrentCell.IsInEditMode)
            {
                this.points_view.Rows.Add(new object[] { });
                coords.Add(new PVector(0, 0, -1));
                movable_coords.Add(new PVector(0, 0, 0));
            }
            
            if (e.KeyCode == Keys.Delete &&
                this.points_view.Rows.Count != 0)
            {
                int index = this.points_view.SelectedCells[0].RowIndex;

                Console.WriteLine(index);

                this.points_view.Rows.RemoveAt(index);
                movable_coords.RemoveAt(index);
                coords.RemoveAt(index);
            }

            Show_Points();
        }



        private void Draw_Axises(Graphics g)
        {
            //var min_x = frame_coords.Min(r => r[0]);
            //var min_y = frame_coords.Min(r => r[1]);
            //var max_x = frame_coords.Max(r => r[0]);
            //var max_y = frame_coords.Max(r => r[1]);
            //var y_center = min_y + (max_y - min_y) / 2;
            //var x_center = min_x + (max_x - min_x) / 2;

            //g.DrawLine(new Pen(Color.Yellow, 3), min_x, max_y, min_x, y_center);
            //g.DrawLine(new Pen(Color.Yellow, 3), min_x, y_center, min_x - 5, y_center + 10);
            //g.DrawLine(new Pen(Color.Yellow, 3), min_x, y_center, min_x + 5, y_center + 10);
            //g.DrawString("Y", new Font("Gilroy Black", 16), Brushes.Yellow, min_x - 30, y_center);

            //g.DrawLine(new Pen(Color.Red, 3), min_x, max_y, x_center, max_y);
            //g.DrawLine(new Pen(Color.Red, 3), x_center, max_y, x_center - 10, max_y + 5);
            //g.DrawLine(new Pen(Color.Red, 3), x_center, max_y, x_center - 10, max_y - 5);
            //g.DrawString("X", new Font("Gilroy Black", 16), Brushes.Red, x_center - 5, max_y + 5);
        }


        private void To_Pixels(PVector v)
        {
            double z = 1 / (distance - v.z);

            double[][] projection =
            {
                        new[] { z, 0, 0 },
                        new[] { 0, z, 0 },
                    };

            PVector pd = MatMul(projection, v);

            pd.x = pd.x * 700 + this.frame.Width / 2;
            pd.y = pd.y * 700 + this.frame.Height / 2;
            z = ((v.z + 1) * 255 / 2);
            z = z > 255 ? 255 : z < 50 ? 50 : z;

            movable_coords.Add(new PVector(pd.x, pd.y, z));
        }


        private double[][] Vec_To_Mat(PVector v)
        {
            double[][] m = new double[3][];

            m[0] = new[] { v.x };
            m[1] = new[] { v.y };
            m[2] = new[] { v.z };

            return m;
        }


        private PVector Mat_To_vec(double[][] m)
        {
            PVector v = new PVector();
            v.x = m[0][0];
            v.y = m[1][0];

            if (m.Length > 2)
                v.z = m[2][0];

            return v;
        }


        private PVector MatMul(double[][] a, PVector b)
        {
            double[][] m = Vec_To_Mat(b);
            return Mat_To_vec(MatMul(a, m));
        }
        

        private double[][] MatMul(double[][] a, double[][] b)
        {
            int colsA = a[0].Length;
            int rowsA = a.Length;
            int colsB = b[0].Length;
            int rowsB = b.Length;

            if (colsA != rowsB)
            {
                Console.WriteLine("Columns of A must match rows of B");
                return null;
            }

            double[][] result = new double[rowsA][];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    result[i] = new double[colsB];

                    double sum = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += a[i][k] * b[k][j];
                    }
                    result[i][j] = sum;
                }
            }
            return result;
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
    }


    public partial class PVector
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public PVector(double x, double y, double z)
        {
            this.x = x; this.y = y; this.z = z;
        }


        public PVector() {
            this.x = 0; this.y = 0; this.z = 0;
        }


        public void mult(int d)
        {
            this.x *= d; this.y *= d; this.z *= d;
        }

        public void Round()
        {
            this.x = Math.Round(this.x, 2);
            this.y = Math.Round(this.y, 2);
            this.z = Math.Round(this.z, 2);
        }
    }
}
