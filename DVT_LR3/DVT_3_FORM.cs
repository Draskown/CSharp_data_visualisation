using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.IO;
using SharpGL;
using System;
using System.Drawing;

namespace DVT_LR3
{
    public partial class Application : Form
    {
        private List<PointF3> points;
        OpenGL scatterGL, histGL;
        bool pauseFlag;
        int pointSize;
        Random r;

        public Application()
        {
            InitializeComponent();

            this.scatterPlot.FrameRate = 60;
            this.histPlot.FrameRate = 60;
            this.timer1.Interval = 1500;

            scatterGL = this.scatterPlot.OpenGL;
            histGL = this.histPlot.OpenGL;
            points = new List<PointF3>();
            pauseFlag = false;
            r = new Random();
            pointSize = 3;
        }

        private void Application_Load(object sender, EventArgs e)
        {
            this.thinning.Maximum = this.pointsAmount.Value;

            this.pointsAmount.ValueChanged += (ss, ee) =>
            {
                var nuds = (NumericUpDown)ss;
                this.thinning.Maximum = nuds.Value;
            };

            this.Click += (ss, ee) => this.ActiveControl = null;

            this.scatterPlot.OpenGLDraw += DrawPoints;

            this.timer1.Tick += TimerTick;

            this.btnStart.Click += Start;

            this.btnLoad.Click += LoadPoints;

            this.btnSave.Click += SavePlots;
        }


        private void SavePlots(object sender, EventArgs e)
        {
            var lockMode = System.Drawing.Imaging.ImageLockMode.WriteOnly;
            var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            var width = this.scatterPlot.Width;
            var height = this.scatterPlot.Height;

            var bitmap = new Bitmap(width, height, format);
            var bitmapRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(bitmapRectangle, lockMode, format); 
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    scatterGL.ReadPixels(i, j, width, height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);  
                }
            }
            bitmap.UnlockBits(bmpData);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            bitmap.Save("scatterImage.bmp");

            this.Close();

            //var width = this.scatterPlot.Width;
            //var height = this.scatterPlot.Height;

            //Bitmap bmp = new Bitmap(width, height);

            //byte[] scatterImage = new byte[3*width*height];

            //scatterGL.ReadPixels(0, 0, width, height, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, scatterImage);

            //Console.WriteLine(string.Join(", ", scatterImage));

            //for (int i = 0; i < width; ++i)
            //{
            //    for (int j = 0; j < height; ++j)
            //    {
            //        bmp.SetPixel(i, j, Color.FromArgb(scatterImage[i*j % 3], scatterImage[(i*j + 1) % 3], scatterImage[(i*j + 2) % 3]));
            //    }
            //}

            //bmp.Save("scatterPlot.bmp");
        }


        private void Start(object sender, EventArgs e)
        {
            if (!pauseFlag && points.Count == 0)
            {
                this.timer1.Start();
                this.btnStart.Text = "Pause";
                return;
            }

            if (pauseFlag)
            {
                this.timer1.Start();
                this.btnStart.Text = "Pause";
            }
            else
            {
                this.timer1.Stop();
                this.btnStart.Text = "Resume";
            }

            pauseFlag = !pauseFlag;
        }


        #region Points generation

        private void TimerTick(object sender, EventArgs e)
        {
            var amount = this.pointsAmount.Value;

            if (points.Count == 0 || amount > points.Count)
            {
                for (int i = 0; i < amount; i++)
                    Generate_Point();
            }
            else if (amount == points.Count)
            {
                points.RemoveAt(0);
                Generate_Point();
            }
            else
            {
                points.RemoveRange(0, (int)(points.Count - amount));
            }
        }


        private void Generate_Point()
        {
            var v = new PointF3((float)Math.Round(r.NextDouble(), 2),
                                (float)Math.Round(r.NextDouble(), 2),
                                (float)Math.Round(r.NextDouble(), 2));

            for (int _ = 0; _ < 3; _++)
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

            points.Add(v);
        }
        #endregion


        #region Drawing
        private void DrawPoints(object sender, SharpGL.RenderEventArgs args)
        {
            scatterGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            scatterGL.LoadIdentity();

            scatterGL.Translate(0.0f, 0.0f, -4.0f);

            scatterGL.PointSize(pointSize);
            scatterGL.Begin(OpenGL.GL_POINTS);
            scatterGL.Color(1.0f, 0.0f, 1.0f);

            for (int i = 0; i < points.Count; i++)
            {
                if (i % this.thinning.Value == 0)
                {
                    var p = points[i];
                    scatterGL.Vertex(p.X, p.Y, p.Z);
                }
            }

            scatterGL.End();
            scatterGL.Flush();
        } 
        #endregion


        private void LoadPoints(object sender, EventArgs e)
        {
            var backup = points;

            points.Clear();

            try
            {
                using (StreamReader reader = new StreamReader("data.csv"))
                {
                    var message = reader.ReadToEnd();

                    var read_lines = message.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in read_lines)
                    {
                        var arr = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
                        var v = new PointF3(arr[0], arr[1], arr[2]);

                        points.Add(v);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Reader has done an oopsie!" + ex.Message);
                points = backup;
            }
        }


        
    }


    class PointF3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public PointF3(float x, float y, float z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }
    }
}
