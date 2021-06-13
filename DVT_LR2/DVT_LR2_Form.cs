using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Data;
using System.IO;
using System;

namespace DVT_LR2
{
    public partial class DVT_LR2_Form : Form
    {
        private double[][] rotationY, a;
        private readonly double[][] defaultMultipliers;
        private double distance, angleX;
        private readonly double angleDelta;
        private readonly PVector[] lineX, lineY, lineZ;
        private PointF initial_point, delta;
        private readonly int pointSize;
        private List<PVector> coords;
        private readonly Random r;
        private Bitmap bmp;


        public DVT_LR2_Form()
        {
            InitializeComponent();

            coords = new List<PVector>();
            lineX = new PVector[4];
            lineY = new PVector[4];
            lineZ = new PVector[4];
            angleDelta = 0.01;
            r = new Random();
            pointSize = 7;
            distance = 150;
            delta.X = 300;
            delta.Y = 300;
            angleX = 0;


            defaultMultipliers = a = new double[][] {
                                new double[] { 1, 0, -1 },
                                new double[] { 0, 1, 0 },
                                new double[] { 1, 0, 1 }
            };

            foreach (var row in defaultMultipliers)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(this.rotationY_grid);

                for (int i = 0; i < 3; i++)
                    r.Cells[i].Value = row[i];

                r.Height = 40;
                this.rotationY_grid.Rows.Add(r);
            }
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

            this.frame.MouseWheel += PicImage_MouseWheel;
            this.frame.MouseDown += PicImage_MouseDown;
            this.frame.MouseMove += PicImage_MouseMove;

            this.rotationY_grid.CellEndEdit += Edit_Cell;
        }


        private void DVT_LR2_Form_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.rotationY_grid.ClearSelection();
        }


        private void Random_Generate(object sender, EventArgs e)
        {
            coords.Clear();

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
                            v.X *= -1;
                            break;
                        case 1:
                            v.Y *= -1;
                            break;
                        case 2:
                            v.Z *= -1;
                            break;
                    }
                }

                coords.Add(v);
            }

            Draw_Points();
        }


        private void Function_Generate(object sender, EventArgs e)
        {
            coords.Clear();

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

                v.X = v.X > 1 ? 1 : v.X < -1 ? -1 : v.X;
                v.Y = v.Y > 1 ? 1 : v.Y < -1 ? -1 : v.Y;
                v.Z = v.Z > 1 ? 1 : v.Z < -1 ? -1 : v.Z;

                coords.Add(v);
            }

            Draw_Points();
        }


        private void Save_Points(object sender, EventArgs e)
        {
            string message = "";

            try
            {
                coords.ForEach(v => message += $"{v.X}, {v.Y}, {v.Z}; ");

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

            try
            {
                using (StreamReader reader = new StreamReader("data.csv"))
                {
                    var message = reader.ReadToEnd();

                    var read_lines = message.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in read_lines)
                    {
                        var arr = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                        var v = new PVector(arr[0], arr[1], arr[2]);

                        coords.Add(v);
                    }
                }

                Draw_Points();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Reader has done an oopsie!" + ex.Message);
                coords = backup;
            }
        }


        private void Edit_Cell(object sender, DataGridViewCellEventArgs e)
        {
            if (this.rotationY_grid.CurrentCell.Value == null)
                return;

            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string separator = nfi.CurrencyDecimalSeparator;

            string input = this.rotationY_grid.CurrentCell.Value.ToString();

            string pattern = @"^-?\d+(\" + separator + @"\d{1,2})?$";

            Match m = Regex.Match(input, pattern);

            if (!m.Success)
            {
                MessageBox.Show("Please make sure that inputs are following the pattern \'X" + separator + "XX\' or \'XX\'");

                this.rotationY_grid.CurrentCell.Value = 0;

                return;
            }

            a = new double[3][];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = new double[3];

                for (int j = 0; j < a[i].Length; j++)
                    a[i][j] = Convert.ToDouble(this.rotationY_grid.Rows[i].Cells[j].Value);
            }

            Draw_Points();
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

                if (ModifierKeys == Keys.Shift)
                    angleX -= (final_point.X - initial_point.X) * angleDelta;
                else
                {
                    delta.X += final_point.X - initial_point.X;
                    delta.Y += final_point.Y - initial_point.Y;
                }

                Draw_Points();

                initial_point = final_point;
            }
        }


        private void Draw_Points()
        {
            bmp = new Bitmap(this.frame.Width, this.frame.Height);

            lineY[0] = new PVector(-1, 1);
            lineY[1] = new PVector(-1, 0);
            lineY[2] = new PVector(-0.96, 0.1);
            lineY[3] = new PVector(-1.04, 0.1);
            var y_string = new PVector(-1.2, 0.1);

            lineX[0] = new PVector(-1, 1);
            lineX[1] = new PVector(0, 1);
            lineX[2] = new PVector(-0.1, 0.96);
            lineX[3] = new PVector(-0.1, 1.04);
            var x_string = new PVector(-0.2, 1.04);

            lineZ[0] = new PVector(-1, 1, 0);
            lineZ[1] = new PVector(-1, 1, 1);
            lineZ[2] = new PVector(-1.04, 1, 0.96);
            lineZ[3] = new PVector(-0.96, 1, 1.04);
            var z_string = new PVector(-1.04, 1, 1.04);

            rotationY = new double[][] {
                        new double[] { Math.Cos(angleX) * a[0][0], 0* a[0][1], Math.Sin(angleX)*a[0][2] },
                        new double[] { 0*a[1][0], 1* a[1][1], 0*a[1][2] },
                        new double[] { Math.Sin(angleX)* a[2][0], 0* a[2][1], Math.Cos(angleX)* a[2][2] }
            };

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var v in coords)
                {
                    PVector rotated = MatMul(rotationY, v);

                    var x_string_rot = MatMul(rotationY, x_string);
                    var y_string_rot = MatMul(rotationY, y_string);
                    var z_string_rot = MatMul(rotationY, z_string);

                    var rotatedX = new PVector[lineX.Length];
                    var rotatedY = new PVector[lineY.Length];
                    var rotatedZ = new PVector[lineZ.Length];

                    for (int i = 0; i < lineX.Length; i++)
                    {
                        rotatedX[i] = MatMul(rotationY, lineX[i]);
                        rotatedY[i] = MatMul(rotationY, lineY[i]);
                        rotatedZ[i] = MatMul(rotationY, lineZ[i]);
                    }

                    double[][] projection =
                    {
                        new double[] { 1, 0, 0 },
                        new double[] { 0, 1, 0 }
                    };

                    PVector projected2d = MatMul(projection, rotated);
                    projected2d.Mult(distance);
                    projected2d.Move(delta);
                    int alpha = (int)((rotated.Z + 0.86) * 255 / 0.85 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);

                    var x_string_proj = MatMul(projection, x_string_rot);
                    x_string_proj.Mult(distance);
                    x_string_proj.Move(delta);

                    var y_string_proj = MatMul(projection, y_string_rot);
                    y_string_proj.Mult(distance);
                    y_string_proj.Move(delta);

                    var z_string_proj = MatMul(projection, z_string_rot);
                    z_string_proj.Mult(distance);
                    z_string_proj.Move(delta);

                    g.DrawString("X", new Font("Gilroy Black", 14), Brushes.Red, (float)x_string_proj.X, (float)x_string_proj.Y);
                    g.DrawString("Y", new Font("Gilroy Black", 14), Brushes.Yellow, (float)y_string_proj.X, (float)y_string_proj.Y);
                    g.DrawString("Z", new Font("Gilroy Black", 14), Brushes.Blue, (float)z_string_proj.X, (float)z_string_proj.Y);

                    var projectedX = new PVector[lineX.Length];
                    var projectedY = new PVector[lineY.Length];
                    var projectedZ = new PVector[lineZ.Length];
                    for (int i = 0; i < lineX.Length; i++)
                    {
                        projectedX[i] = MatMul(projection, rotatedX[i]);
                        projectedX[i].Mult(distance);
                        projectedX[i].Move(delta);

                        projectedY[i] = MatMul(projection, rotatedY[i]);
                        projectedY[i].Mult(distance);
                        projectedY[i].Move(delta);

                        projectedZ[i] = MatMul(projection, rotatedZ[i]);
                        projectedZ[i].Mult(distance);
                        projectedZ[i].Move(delta);

                        if (i != 0)
                        {
                            g.DrawLine(new Pen(Color.Red, 3), (float)projectedX[i / 2].X, (float)projectedX[i / 2].Y, (float)projectedX[i % 4].X, (float)projectedX[i % 4].Y);
                            g.DrawLine(new Pen(Color.Yellow, 3), (float)projectedY[i / 2].X, (float)projectedY[i / 2].Y, (float)projectedY[i % 4].X, (float)projectedY[i % 4].Y);
                            g.DrawLine(new Pen(Color.Blue, 3), (float)projectedZ[i / 2].X, (float)projectedZ[i / 2].Y, (float)projectedZ[i % 4].X, (float)projectedZ[i % 4].Y);
                        }
                    }
                }
            }

            this.frame.Image = bmp;
        }


        private void PicImage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.frame.Image == null)
                return;

            distance += e.Delta / 10;
            distance = distance < 0 ? 0 : distance;

            Draw_Points();
        }


        private double[][] Vec_To_Mat(PVector v)
        {
            double[][] m = new double[3][];

            m[0] = new[] { v.X };
            m[1] = new[] { v.Y };
            m[2] = new[] { v.Z };

            return m;
        }


        private PVector Mat_To_vec(double[][] m)
        {
            PVector v = new PVector
            {
                X = m[0][0],
                Y = m[1][0]
            };

            if (m.Length > 2)
                v.Z = m[2][0];

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
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public PVector(double x, double y, double z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }

        public PVector()
        {
            this.X = 0; this.Y = 0; this.Z = 0;
        }

        public PVector(double x, double y)
        {
            this.X = x; this.Y = y; this.Z = 0;
        }

        public void Mult(double d)
        {
            this.X *= d; this.Y *= d; this.Z *= d;
        }

        public void Move(PointF p)
        {
            this.X += p.X; this.Y += p.Y;
        }
    }
}
