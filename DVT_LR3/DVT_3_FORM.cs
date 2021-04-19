using SharpGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DVT_LR3
{
    public partial class Application : Form
    {
        #region Initialization and timer conditions

        private readonly OpenGL scatterGL, histGL;
        private PointF initPoint, angle, delta;
        private bool pauseFlag, loadedFlag;
        private readonly float angleDelta;
        private readonly float cellDelta;
        private readonly int pointSize;
        private List<PointF3> points;
        private int[] xFreq, yFreq;
        private readonly Random r;
        private int actualAmount;
        private float distance;

        public Application()
        {
            InitializeComponent();

            this.scatterPlot.FrameRate = 60;
            this.histPlot.FrameRate = 60;
            this.timer1.Interval = 1500;

            points = new List<PointF3>();
            initPoint = new PointF();
            delta = new PointF(-0.5f, -0.5f);
            r = new Random();

            scatterGL = this.scatterPlot.OpenGL;
            histGL = this.histPlot.OpenGL;

            distance = (float)(this.scatterPlot.Width / 100);
            pauseFlag = loadedFlag = false;
            angleDelta = 0.2f;
            cellDelta = 0.2f;
            pointSize = 5;
        }


        private void Application_Load(object sender, EventArgs e)
        {
            this.thinning.Maximum = this.pointsAmount.Value;

            this.pointsAmount.ValueChanged += (ss, ee) =>
            {
                var nuds = (NumericUpDown)ss;
                this.thinning.Maximum = nuds.Value;
                loadedFlag = false;
            };

            this.Click += (ss, ee) => this.ActiveControl = null;

            this.scatterPlot.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.scatterPlot.MouseWheel += ChangeDistance;
            this.scatterPlot.MouseMove += ChangePosition;
            this.scatterPlot.OpenGLDraw += DrawPoints;

            this.btnStart.Click += Start;

            this.btnLoad.Click += LoadPoints;

            this.btnSave.Click += SavePlots;

            this.timer1.Tick += TimerTick;
        }


        private void Start(object sender, EventArgs e)
        {
            if (!pauseFlag && (points.Count == 0 || loadedFlag))
            {
                this.timer1.Start();
                this.btnStart.Text = "Pause";
                loadedFlag = false;
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

        #endregion



        #region Points generation

        private void TimerTick(object sender, EventArgs e)
        {
            xFreq = new int[10];
            yFreq = new int[10];

            var amount = this.pointsAmount.Value;

            if (amount == 0)
                return;

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
            float scale = 0.07f;

            scatterGL.Enable(OpenGL.GL_BLEND);
            scatterGL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            scatterGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            scatterGL.LoadIdentity();

            scatterGL.Translate(delta.X, delta.Y, -distance);
            scatterGL.Rotate(angle.Y, angle.X, 0.0f);

            scatterGL.PointSize(pointSize);
            scatterGL.Begin(OpenGL.GL_POINTS);
            scatterGL.Color(1.0f, 0.0f, 1.0f);

            xFreq = new int[10];
            yFreq = new int[10];

            actualAmount = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (i % this.thinning.Value == 0)
                {
                    var p = points[i];
                    CountPoint(p);
                    this.actualAmount++;
                    scatterGL.Vertex(p.X, p.Y, p.Z);
                }
            }
            scatterGL.End();

            scatterGL.LoadIdentity();
            scatterGL.Translate(delta.X, delta.Y, -distance);
            scatterGL.Rotate(angle.Y, angle.X, 0.0f);

            float xIndent = 1.2f;
            float yIndent = 1.2f;
            float zIndent = -1.0f;

            scatterGL.Begin(OpenGL.GL_QUADS);

            if (this.actualAmount != 0)
            {
                for (int i = -5; i < 5; i++)
                {
                    scatterGL.Color(1.0f, 0.0f, 0.0f, 0.7f);
                    scatterGL.Vertex(i * cellDelta, yIndent, zIndent);
                    scatterGL.Vertex(i * cellDelta, (float)xFreq[i + 5] * scale + yIndent, zIndent);
                    scatterGL.Vertex(i * cellDelta + cellDelta, (float)xFreq[i + 5] * scale + yIndent, zIndent);
                    scatterGL.Vertex(i * cellDelta + cellDelta, yIndent, zIndent);

                    scatterGL.Color(1.0f, 1.0f, 0.0f, 0.7f);
                    scatterGL.Vertex(xIndent, i*cellDelta, zIndent);
                    scatterGL.Vertex((float)yFreq[i + 5] * scale + xIndent, i * cellDelta , zIndent);
                    scatterGL.Vertex((float)yFreq[i + 5] * scale + xIndent, i * cellDelta + cellDelta, zIndent);
                    scatterGL.Vertex(xIndent, i * cellDelta + cellDelta, zIndent);
                }
            }
            scatterGL.End();

            scatterGL.Begin(OpenGL.GL_QUADS);

            scatterGL.Color(1.0f, 0.0f, 1.0f, 0.5f);
            for (int i = -5; i < 5; i++)
            {
                for (int j = -5; j < 5; j++)
                {
                    for (int k = -5; k < 5; k++)
                    {

                    }
                }
            }

            scatterGL.Flush();
        }

        #endregion


        #region Histograms calculation

        private void CountPoint(PointF3 p)
        {
            for (int i = -5; i < 10; i++)
            {
                if ((int)(p.X / cellDelta) == i)
                    xFreq[i + 5]++;
                if ((int)(p.Y / cellDelta) == i)
                    yFreq[i + 5]++;
            }
        }

        #endregion



        #region Mouse moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                            points.Count == 0)
                return;

            PointF finalPoint = e.Location;

            if (ModifierKeys == Keys.Shift)
            {
                angle.X += (finalPoint.X - initPoint.X)*angleDelta;
                angle.Y += (finalPoint.Y - initPoint.Y)*angleDelta;
            }
            else
            {
                delta.X += (finalPoint.X - initPoint.X) / 100;
                delta.Y -= (finalPoint.Y - initPoint.Y) / 100;
            }

            initPoint = finalPoint;
        }


        private void ChangeDistance(object sender, MouseEventArgs e)
        {
            distance -= e.Delta / 120;
        }

        #endregion



        #region Loading and saving

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

                loadedFlag = true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Reader has done an oopsie!" + ex.Message);
                points = backup;
            }
        }


        private void SavePlots(object sender, EventArgs e)
        {
            var lockMode = System.Drawing.Imaging.ImageLockMode.WriteOnly;
            var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            var width = this.scatterPlot.Width;
            var height = this.scatterPlot.Height;

            var bmp = new Bitmap(width, height, format);
            var bmpRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(bmpRect, lockMode, format);
            scatterGL.ReadPixels(0, 0, width, height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);

            bmp.UnlockBits(bmpData);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            bmp.Save("scatterPlot.bmp");
        }

        #endregion
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
