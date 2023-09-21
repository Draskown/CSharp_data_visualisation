using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace CSharp_data_visualisation
{
    public partial class DVT_1_Form : Form
    {
        public List<double> X1 = new List<double>();
        public List<double> X2 = new List<double>();
        public List<double> Y1 = new List<double>();
        public List<double> Y2 = new List<double>();
        public List<double> x1_lst = new List<double>();
        public List<double> y1_lst = new List<double>();
        public List<double> x2_lst = new List<double>();
        public List<double> y2_lst = new List<double>();

        public TextBox[] X1_s, X2_s, Y1_s, Y2_s;

        int key = 0;

        private readonly int[] x_freq_count = new int[10];
        private readonly int[] y_freq_count = new int[10];


        public DVT_1_Form()
        {
            InitializeComponent();

            X1_s = new TextBox[] { X1_1, X1_2, X1_3, X1_4, X1_5 };
            X2_s = new TextBox[] { X2_1, X2_2, X2_3, X2_4, X2_5 };
            Y1_s = new TextBox[] { Y1_1, Y1_2, Y1_3, Y1_4, Y1_5 };
            Y2_s = new TextBox[] { Y2_1, Y2_2, Y2_3, Y2_4, Y2_5 };
        }


        private void Application_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                x1_lst.Add(0.0);
                x2_lst.Add(0.0);
                y1_lst.Add(0.0);
                y2_lst.Add(0.0);
            }
        }


        private void Application_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }


        private void Rb_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_X1Y1.Checked)
                key = 0;

            if (rb_X1X2.Checked)
                key = 1;

            if (rb_X2Y2.Checked)
                key = 2;

            if (rb_Y1Y2.Checked)
                key = 3;

            Draw_Plots();
        }


        private void KeyParameter_Protection(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            TextBox box = (TextBox)sender;

            if (box.Name.Contains('X'))
            {
                if (ch == 46 && box.Text.IndexOf('.') != -1)
                {
                    e.Handled = true;
                    return;
                }

                if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
                    e.Handled = true;
            }

            if (box.Name.Contains('Y'))
            {
                if ((ch == 46 || ch == 45) && (box.Text.IndexOf('.') != -1))
                {
                    e.Handled = true;
                    return;
                }

                if (!Char.IsDigit(ch) && ch != 8 && ch != 46 && ch != 45)
                    e.Handled = true;
            }
        }


        private void KeyParameter_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text != "" && box.Text != "-")
            {
                if (box.Name.Contains("X1"))
                    AscendingCheck(X1_s);
                if (box.Name.Contains("X2"))
                    AscendingCheck(X2_s);
            }
        }


        static void AscendingCheck(TextBox[] array)
        {
            for (int i = 1; i < 4; i++)
            {
                double value = Convert.ToDouble(array[i].Text);
                double next_value = Convert.ToDouble(array[i + 1].Text);
                double prev_value = Convert.ToDouble(array[i - 1].Text);

                if (value > next_value)
                    array[i].Text = array[i + 1].Text;
                if (value < prev_value)
                    array[i].Text = array[i - 1].Text;
            }
        }


        private void KeyParameter_Left(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text == "")
                box.Text = "0";

            if (Y1.Any() && Y2.Any())
            {
                Calculate_Y(X1, Y1, 1);
                Calculate_Y(X2, Y2, 2);

                Draw_Plots();
            }
        }


        private void Btn_save_Click(object sender, EventArgs e)
        {
            string msg = string.Format("{0}\n{1}\n{2}\n{3}", string.Join(" ", x1_lst),
                string.Join(" ", x2_lst), string.Join(" ", y1_lst), string.Join(" ", y2_lst));

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("settings.txt"))
                    file.Write(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application did an oopsie.. \n {ex}");
            }
        }


        private void Btn_load_Click(object sender, EventArgs e)
        {
            string msg = "";

            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader("settings.txt"))
                    msg = file.ReadToEnd();

                string[] line = msg.Split('\n');

                x1_lst = line[0].Split(' ').Select(x => double.Parse(x)).ToList();
                x2_lst = line[1].Split(' ').Select(x => double.Parse(x)).ToList();
                y1_lst = line[2].Split(' ').Select(x => double.Parse(x)).ToList();
                y2_lst = line[3].Split(' ').Select(x => double.Parse(x)).ToList();

                for (int i = 0; i < 5; i++)
                {
                    X1_s[i].Text = x1_lst[i].ToString();
                    X2_s[i].Text = x2_lst[i].ToString();
                    Y1_s[i].Text = y1_lst[i].ToString();
                    Y2_s[i].Text = y2_lst[i].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application did an oopsie.. \n {ex}");
            }
        }


        private void Btn_generate_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int count = (int)num_count.Value;
            X1.Clear();
            X2.Clear();

            for (int i = 0; i < count; i++)
            {
                X1.Add(Math.Round(rand.NextDouble(), 3));
                X2.Add(Math.Round(rand.NextDouble(), 3));
            }

            Calculate_Y(X1, Y1, 1);
            Calculate_Y(X2, Y2, 2);

            Draw_Plots();
        }


        private void Calculate_Y(List<double> x_arr, List<double> y_arr, int key)
        {
            y_arr.Clear();

            for (int i = 0; i < 5; i++)
            {
                if (key == 1)
                {
                    if (i == 0)
                    {
                        x1_lst.Clear();
                        y1_lst.Clear();
                    }

                    x1_lst.Add(Convert.ToDouble(X1_s[i].Text));
                    y1_lst.Add(Convert.ToDouble(Y1_s[i].Text));
                }
                else
                {
                    if (i == 0)
                    {
                        x2_lst.Clear();
                        y2_lst.Clear();
                    }

                    x2_lst.Add(Convert.ToDouble(X2_s[i].Text));
                    y2_lst.Add(Convert.ToDouble(Y2_s[i].Text));
                }
            }

            for (int i = 0; i < x_arr.Count; i++)
            {
                if (x_arr[i] >= x1_lst[0] && x_arr[i] <= x1_lst[1])
                    y_arr.Add(y1_lst[1] / x1_lst[1] * x_arr[i]);
                if (x_arr[i] > x1_lst[1] && x_arr[i] <= x1_lst[2])
                    y_arr.Add(y1_lst[2] / x1_lst[2] * x_arr[i]);
                if (x_arr[i] > x1_lst[2] && x_arr[i] <= x1_lst[3])
                    y_arr.Add(y1_lst[3] / x1_lst[3] * x_arr[i]);
                if (x_arr[i] > x1_lst[3] && x_arr[i] <= x1_lst[4])
                    y_arr.Add(y1_lst[4] / x1_lst[4] * x_arr[i]);
            }

            Maths(y_arr, key);
        }


        private void Maths(List<double> arr, int key)
        {
            double average = arr.Average(), dispersion, sum = 0, median = arr.Median();

            for (int i = 0; i < arr.Count; i++)
            {
                sum = Math.Pow(arr[i] - average, 2);
            }

            double mode = arr.GroupBy(v => v)
                            .OrderByDescending(g => g.Count())
                            .First()
                            .Key;

            dispersion = sum / arr.Count;

            if (key == 1)
            {
                avg1_box.Text = Math.Round(average, 2).ToString();
                dis1_box.Text = Math.Round(dispersion, 2).ToString();
                mode1_box.Text = Math.Round(mode, 2).ToString();
                med1_box.Text = Math.Round(median, 2).ToString();
            }
            else
            {
                avg2_box.Text = Math.Round(average, 2).ToString();
                dis2_box.Text = Math.Round(dispersion, 2).ToString();
                mode2_box.Text = Math.Round(mode, 2).ToString();
                med2_box.Text = Math.Round(median, 2).ToString();
            }
        }


        private void Draw_Plots()
        {
            List<double> _1 = new List<double>();
            List<double> _2 = new List<double>();
            double max_x = x1_lst[4], max_y = y1_lst[4];


            switch (key)
            {
                case 0:
                    correlation_plot.Series[0].Points.DataBindXY(x1_lst, y1_lst);
                    _1 = X1; _2 = Y1;
                    break;
                case 1:
                    correlation_plot.Series[0].Points.DataBindXY(x1_lst, x2_lst);
                    _1 = X1; _2 = X2; max_x = x1_lst[4]; max_y = x2_lst[4];
                    break;
                case 2:
                    correlation_plot.Series[0].Points.DataBindXY(x2_lst, y2_lst);
                    _1 = X2; _2 = Y2; max_x = x2_lst[4]; max_y = y2_lst[4];
                    break;
                case 3:
                    correlation_plot.Series[0].Points.DataBindXY(y1_lst, y2_lst);
                    _1 = Y1; _2 = Y2; max_x = y1_lst[4]; max_y = y2_lst[4];
                    break;
            }

            correlation_plot.Update();


            Bitmap scatter = new Bitmap(scatter_plot.Width, scatter_plot.Height);

            double x, y;
            int delta_x = scatter.Width / 10,
                delta_y = scatter.Height / 10;

            Array.Clear(x_freq_count, 0, 10);
            Array.Clear(y_freq_count, 0, 10);

            for (int i = 0; i < 10; i++)
            {
                x_freq_plot.Series[0].Points[i].SetValueXY(i, 0);
                y_freq_plot.Series[0].Points[i].SetValueXY(10 - i, 0);
            }

            for (int i = 0; i < X1.Count; i++)
            {
                x = scatter.Width / max_x * _1[i];
                y = scatter.Height / max_y * _2[i];

                Count_Frequency(x, delta_x, 0);
                Count_Frequency(y, delta_y, 1);

                int[] cell = new int[] { (int)(x / delta_x), (int)(y / delta_y) };

                cell[0] = cell[0] > 9 ? 9 : cell[0];
                cell[1] = cell[1] > 9 ? 9 : cell[1];

                int alpha = (int)((x_freq_count[cell[0]] + y_freq_count[cell[1]]) / 2.0 / (double)num_count.Value * 255);

                using (Graphics g = Graphics.FromImage(scatter))
                {
                    using (SolidBrush b = new SolidBrush(Color.Fuchsia))
                        g.FillEllipse(b, (int)x, (int)y, 7, 7);

                    using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, 255, 0, 255)))
                        g.FillRectangle(b, cell[0] * delta_x, cell[1] * delta_y, delta_x, delta_y);
                }
            }

            scatter_plot.BackgroundImage = scatter;
        }


        private void Count_Frequency(double value, double delta, int key)
        {
            for (int i = 0; i < 10; i++)
            {
                if ((int)(value / delta) == i)
                {
                    if (key == 0)
                    {
                        x_freq_count[i]++;
                        x_freq_plot.ResetAutoValues();
                        x_freq_plot.Series[0].Points[i].SetValueXY(i, x_freq_count[i]);
                    }
                    else
                    {
                        y_freq_count[i]++;
                        y_freq_plot.ResetAutoValues();
                        y_freq_plot.Series[0].Points[i].SetValueXY(10 - i, y_freq_count[i]);
                    }
                }
            }
        }
    }
}
