using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPDT_LR1
{
    public partial class Bitmapshit : Form
    {
        List<Point> test = new List<Point> { new Point(20, 80),
                                             new Point(20, 40),
                                             new Point(12, 50),
                                             new Point(1, 90)
        };

        Point[] line1 = new Point[] {   new Point(35, 76),
                                        new Point(44, 66),
                                        new Point(57, 51),
                                        new Point(72, 34),
                                        new Point(85, 20)
        };

        Point[] line2 = new Point[] {   new Point(158, 70),
                                        new Point(268, 70),
                                        new Point(200, 70),
                                        new Point(220, 70),
                                        new Point(180, 70)
        };

        Point[] line3 = new Point[] {   new Point(101, 97),
                                        new Point(101, 120),
                                        new Point(101, 297),
                                        new Point(101, 200),
                                        new Point(101, 160)
        };

        Point[] line4 = new Point[] {   new Point(152, 96),
                                        new Point(169, 115),
                                        new Point(196, 144),
                                        new Point(214, 165),
                                        new Point(227, 180)
        };

        Point[] line5 = new Point[] {   new Point(168, 331),
                                        new Point(183, 228),
                                        new Point(177, 257),
                                        new Point(172, 300),
                                        new Point(176, 272)
        };

        Point[] scatter = new Point[] {   new Point(469, 81),
                                        new Point(480, 107),
                                        new Point(501, 89),
                                        new Point(498, 57),
                                        new Point(521, 119)
        };

        Point[][] objects = new Point[6][];

        List<Point> points = new List<Point>();

        public Bitmapshit()
        {
            InitializeComponent();

            for (int i = 0; i < objects.GetLength(0); i++)
            {
                objects[0] = line1;
                objects[1] = line2;
                objects[2] = line3;
                objects[3] = line4;
                objects[4] = line5;
                objects[5] = scatter;
            }
        }

        private void Bitmapshit_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            Random r = new Random();

            var temp = line1;

            temp = line2;

            Console.WriteLine(string.Join(" ", line1));
            this.Close();

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var obj in objects)
                {
                    foreach (Point p in obj)
                    {
                        g.FillEllipse(
                            Brushes.Fuchsia,
                            p.X,
                            p.Y,
                            3,
                            3
                        );
                    }

                    int min_x = obj.Min(p => p.X),
                        min_y = obj.Min(p => p.Y),
                        max_x = obj.Max(p => p.X),
                        max_y = obj.Max(p => p.Y);

                    Point centroid = new Point((min_x + max_x) / 2, (min_y + max_y) / 2);

                    Point endpoint = obj.Last(p => p.X == min_x),
                          startpoint = obj.Last(p => p.X == max_x);

                    if (min_x == max_x)
                    {
                        startpoint = obj.Last(p => p.Y == min_y);
                        endpoint = obj.Last(p => p.Y == max_y);
                        g.DrawLine(Pens.Blue, startpoint, endpoint);
                    }
                    else
                    {
                        double slope = (startpoint.Y - endpoint.Y) / (startpoint.X - endpoint.X);
                        int y_intersect = (int)(-slope * startpoint.X + startpoint.Y);

                        if (obj.Length - obj.Count(p => Math.Abs(p.Y - slope * p.X - y_intersect) <= 10) < 3)
                            g.DrawLine(Pens.Blue, startpoint, endpoint);
                        else
                            g.DrawRectangle(Pens.Fuchsia, min_x, min_y, max_x - min_x, max_y - min_y);
                    }

                    g.FillRectangle(
                        Brushes.Red,
                        centroid.X,
                        centroid.Y,
                        5,
                        5
                        );
                }
            }

            this.pictureBox1.Image = bmp;
        }
    }
}
