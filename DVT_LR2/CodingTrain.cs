using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVT_LR2
{
    public partial class CodingTrain : Form
    {
        private readonly PVector[] points = new PVector[8];
        private readonly System.Threading.Timer t;
        private readonly int size;
        private double angle;


        public CodingTrain()
        {
            points[0] = new PVector(-0.5, -0.5, -0.5);
            points[1] = new PVector(0.5, -0.5, -0.5);
            points[2] = new PVector(0.5, 0.5, -0.5);
            points[3] = new PVector(-0.5, 0.5, -0.5);
            points[4] = new PVector(-0.5, -0.5, 0.5);
            points[5] = new PVector(0.5, -0.5, 0.5);
            points[6] = new PVector(0.5, 0.5, 0.5);
            points[7] = new PVector(-0.5, 0.5, 0.5);
            size = 10;
            angle = 0.01;
            t = new System.Threading.Timer(new TimerCallback(Draw),
                          0, 0, 1);

            InitializeComponent();
        }


        private void Draw(object obj)
        {
            Bitmap bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);

                double[][] rotationZ = {
                    new double[] {Math.Cos(angle), -Math.Sin(angle), 0},
                    new double[] {Math.Sin(angle), Math.Cos(angle), 0},
                    new double[] { 0, 0, 1 }
                };

                double[][] rotationX = {
                    new double[] { 1, 0, 0 },
                    new double[] {0, (double)Math.Cos(angle), (double)-Math.Sin(angle)},
                    new double[] {0, (double)Math.Sin(angle), (double)Math.Cos(angle)},
                };

                double[][] rotationY = {
                    new double[] {(double)Math.Cos(angle), 0, (double)-Math.Sin(angle)},
                    new double[] { 0, 1, 0 },
                    new double[] {(double)Math.Sin(angle), 0, (double)Math.Cos(angle)}
                };

                foreach (var v in points)
                {
                    PVector rotated = matmul(rotationY, v);
                    rotated = matmul(rotationX, rotated);
                    rotated = matmul(rotationZ, rotated);

                    double distance = 2;
                    double z = 1 / (distance - rotated.z);

                    double[][] projection =
                    {
                        new double[] { z, 0, 0 },
                        new double[] { 0, z, 0 }
                    };

                    PVector projected2d = matmul(projection, rotated);

                    projected2d.mult(300);

                    projected2d.x += this.pictureBox1.Width / 2;
                    projected2d.y += this.pictureBox1.Height / 2;

                    int alpha = (int)((rotated.z + 0.86) * 255 / 0.85 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    Console.WriteLine(alpha);

                    g.FillEllipse(new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)), (float)projected2d.x, (float)projected2d.y, size, size);
                }
            }

            angle += 0.01;

            this.pictureBox1.Image = bmp;
        }


        private double[][] vecToMat(PVector v)
        {
            double[][] m = new double[3][];

            m[0] = new[] { v.x };
            m[1] = new[] { v.y };
            m[2] = new[] { v.z };

            return m;
        }

        private PVector matrixToVec(double[][] m)
        {
            PVector v = new PVector();
            v.x = m[0][0];
            v.y = m[1][0];

            if (m.Length > 2)
                v.z = m[2][0];

            return v;
        }

        private void logMatrix(double[][] m)
        {
            int cols = m[0].Length;
            int rows = m.Length;

            Console.WriteLine($"{rows} x {cols}");
            Console.WriteLine("----------------");

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Console.WriteLine($"{m[i][j]} ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private PVector matmul(double[][] a, PVector b)
        {
            double[][] m = vecToMat(b);
            return matrixToVec(matmul(a, m));
        }

        private double[][] matmul(double[][] a, double[][] b)
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
    }


    public partial class PVector
    {
        public double x { get; set; } = 0;
        public double y { get; set; } = 0;
        public double z { get; set; } = 0;

        public PVector(double x, double y, double z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public PVector() { }

        public void mult (int d)
        {
            this.x *= d; this.y *= d; this.z *= d;
        }
    }
}
